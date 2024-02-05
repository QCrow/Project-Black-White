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

  private Dictionary<Cell, List<Cell>> _possibleTargetsWithEffectRange = new Dictionary<Cell, List<Cell>>();
  public Dictionary<Cell, List<Cell>> PossibleTargetsWithEffectRange
  {
    get => _possibleTargetsWithEffectRange;
    set
    {
      if (_possibleTargetsWithEffectRange != value)
      {
        _possibleTargetsWithEffectRange = value;
        foreach (var cell in _possibleTargetsWithEffectRange.Keys)
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
        foreach (var cell in PossibleTargetsWithEffectRange.Keys)
        {
          cell.RemoveHighlight();
        }
        PossibleTargetsWithEffectRange.Clear();

        _actionSelectionState = value;
        switch (_actionSelectionState)
        {
          case ActionSelectionState.Move:
            PossibleTargetsWithEffectRange = SkillResolver.Instance.GetAvailableMovesWithPaths(SelectedPiece);
            break;
          case ActionSelectionState.Attack:
            PossibleTargetsWithEffectRange = SkillResolver.Instance.GetAvailableTargets(SelectedPiece, SelectedPiece.data.BaseAttack);
            break;
          case ActionSelectionState.Skill:
            PossibleTargetsWithEffectRange = SkillResolver.Instance.GetAvailableTargets(SelectedPiece, SelectedPiece.data.ActiveSkill);
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
      if (PossibleTargetsWithEffectRange.ContainsKey(cell)) // We are clicking a highlighted cell, do the action that is currently selected
      {
        switch (ActionSelectionState)
        {
          case ActionSelectionState.Move:
            MoveCommand moveCommand = new MoveCommand(SelectedPiece, cell);
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
    switch (ActionSelectionState)
    {
      case ActionSelectionState.Move:
        cell.ToggleHovered();
        break;
      case ActionSelectionState.Attack:
        if (PossibleTargetsWithEffectRange.ContainsKey(cell))
        {
          HandleTargetCellHovered(cell, SelectedPiece.data.BaseAttack);
        }
        break;
      case ActionSelectionState.Skill:
        if (PossibleTargetsWithEffectRange.ContainsKey(cell))
        {
          HandleTargetCellHovered(cell, SelectedPiece.data.ActiveSkill);
        }
        break;
    }
  }

  private void HandleTargetCellHovered(Cell cell, SkillSO skill)
  {
    foreach (var pathCell in PossibleTargetsWithEffectRange[cell])
    {
      pathCell.ToggleTargetedHovered();
    }
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