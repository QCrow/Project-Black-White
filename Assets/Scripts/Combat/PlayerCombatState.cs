using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;

public enum ActionSelectionState
{
  None,
  Move,
  Attack,
  Skill
}

public class PlayerCombatState : ICombatState
{
  private Piece _selectedPiece;
  public Piece SelectedPiece
  {
    get => _selectedPiece;
    set
    {
      if (_selectedPiece != value)
      {
        _selectedPiece = value;
        UIManager.Instance.SetActionButtonsInteractable(_selectedPiece != null);
      }
    }
  }

  private Dictionary<Cell, List<Cell>> _possibleTargetCellsWithPaths = new Dictionary<Cell, List<Cell>>();
  public Dictionary<Cell, List<Cell>> PossibleTargetCellsWithPaths
  {
    get => _possibleTargetCellsWithPaths;
    set
    {
      if (_possibleTargetCellsWithPaths != value)
      {
        _possibleTargetCellsWithPaths = value;
        foreach (var cell in _possibleTargetCellsWithPaths.Keys)
        {
          switch (ActionSelectionState)
          {
            case ActionSelectionState.Move:
              cell.SetHighlight(Color.green);
              break;
            case ActionSelectionState.Attack:
              cell.SetHighlight(Color.red);
              break;
            case ActionSelectionState.Skill:
              cell.SetHighlight(Color.red);
              break;
          }
        }
      }
    }
  }

  private ActionSelectionState _actionSelectionState;

  public ActionSelectionState ActionSelectionState
  {
    get => _actionSelectionState;
    set
    {
      if (_actionSelectionState != value)
      {
        foreach (var cell in PossibleTargetCellsWithPaths.Keys)
        {
          cell.RemoveHighlight();
        }
        PossibleTargetCellsWithPaths.Clear();

        _actionSelectionState = value;
        switch (_actionSelectionState)
        {
          case ActionSelectionState.Move:
            PossibleTargetCellsWithPaths = SelectedPiece.GetAvailableMovesWithPaths();
            break;
          case ActionSelectionState.Attack:
            PossibleTargetCellsWithPaths = SelectedPiece.GetAvailableTargets(SelectedPiece.data.BaseAttack);
            break;
          case ActionSelectionState.Skill:
            PossibleTargetCellsWithPaths = SelectedPiece.GetAvailableTargets(SelectedPiece.data.ActiveSkill);
            break;
        }
      }
    }
  }

  private List<ICommand> _allCommands = new List<ICommand>();
  private Dictionary<Piece, MoveCommand> _moveCommandPerPiece = new Dictionary<Piece, MoveCommand>();

  public void EnterState()
  {
    RefreshPieces();
    DecreaseRedeployTimers();
    InputManager.OnCellSelected += HandleCellSelected;
    InputManager.OnCellHovered += HandleCellHovered;
  }

  public void UpdateState()
  {

  }

  public void ExitState()
  {
    InputManager.OnCellSelected -= HandleCellSelected;
    InputManager.OnCellHovered -= HandleCellHovered;
  }

  private void HandleCellSelected(Cell cell)
  {
    if (cell.PieceOnCell != null)
    {
      SelectedPiece = cell.PieceOnCell;
      UIManager.Instance.SetAttackButtonInteractable(SelectedPiece.CanAttack);
      UIManager.Instance.SetSkillButtonInteractable(SelectedPiece.CanUseSkill);
      if (SelectedPiece.CanMove)
      {
        ActionSelectionState = ActionSelectionState.Move;
      }
    }
    else
    {
      if (PossibleTargetCellsWithPaths.ContainsKey(cell)) // We are clicking a highlighted cell, do the action that is currently selected
      {
        switch (ActionSelectionState)
        {
          case ActionSelectionState.Move:
            MoveCommand moveCommand = new MoveCommand(SelectedPiece, cell);
            // _moveCommandPerPiece.Add(SelectedPiece, moveCommand);
            // _allCommands.Add(moveCommand);
            CombatManager.Instance.Invoker.SetCommand(moveCommand);
            CombatManager.Instance.Invoker.ExecuteCommand();
            break;
          case ActionSelectionState.Attack:
            // SelectedPiece.AttackCell(cell);
            break;
          case ActionSelectionState.Skill:
            // SelectedPiece.UseSkillOnCell(cell);
            break;
        }
        SelectedPiece = null;
        ActionSelectionState = ActionSelectionState.None;
      }
      else // We are clicking a cell that is not highlighted, cancel the selection
      {
        SelectedPiece = null;
        ActionSelectionState = ActionSelectionState.None;
      }
    }
  }

  private void HandleCellHovered(Cell cell)
  {
    cell.ToggleHovered();
  }

  private void RefreshPieces()
  {
    foreach (var piece in CombatManager.Instance.PlayerOnBoardPieces)
    {
      if (piece.IsShadowed == piece.CellUnderPiece.IsShadowed)
      {
        piece.VeiledRefreshActions();
      }
      else
      {
        piece.UnveiledRefreshActions();
      }
    }
  }

  private void DecreaseRedeployTimers()
  {
    foreach (var piece in CombatManager.Instance.PlayerOffBoardPieces)
    {
      if (piece.RedeployTimer > 0)
      {
        piece.RedeployTimer--;
      }
    }
  }
}