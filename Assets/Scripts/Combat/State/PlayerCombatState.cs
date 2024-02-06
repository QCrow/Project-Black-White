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
  private Ally _selectedPiece;
  public Ally SelectedPiece
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

  private List<Cell> _possibleTargets = new List<Cell>();
  public List<Cell> PossibleTargets
  {
    get => _possibleTargets;
    set
    {
      if (_possibleTargets != value)
      {
        _possibleTargets = value;
        foreach (var cell in _possibleTargets)
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
        foreach (var cell in PossibleTargets)
        {
          cell.RemoveHighlight();
        }
        PossibleTargets.Clear();

        _actionSelectionState = value;
        switch (_actionSelectionState)
        {
          case ActionSelectionState.Move:
            PossibleTargets = SkillResolver.Instance.GetAvailableMoves(SelectedPiece);
            break;
          case ActionSelectionState.Attack:
            PossibleTargets = SkillResolver.Instance.GetAvailableTargets(SelectedPiece, SelectedPiece.data.BaseAttack);
            break;
          case ActionSelectionState.Skill:
            PossibleTargets = SkillResolver.Instance.GetAvailableTargets(SelectedPiece, SelectedPiece.data.ActiveSkill);
            break;
        }
      }
    }
  }

  private List<ICommand> _allCommands = new List<ICommand>();
  private Dictionary<Ally, MoveCommand> _moveCommandPerPiece = new Dictionary<Ally, MoveCommand>();

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
      if (cell.PieceOnCell is not Ally) return;
      SelectedPiece = (Ally)cell.PieceOnCell;
      UIManager.Instance.SetAttackButtonInteractable(SelectedPiece.CanAttack);
      UIManager.Instance.SetSkillButtonInteractable(SelectedPiece.CanUseSkill);
      if (SelectedPiece.CanMove)
      {
        Debug.Log("Can move");
        ActionSelectionState = ActionSelectionState.Move;
      }
    }
    else
    {
      if (PossibleTargets.Contains(cell)) // We are clicking a highlighted cell, do the action that is currently selected
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
        if (PossibleTargets.Contains(cell))
        {
          HandleTargetCellHovered(cell, SelectedPiece.data.BaseAttack);
        }
        break;
      case ActionSelectionState.Skill:
        if (PossibleTargets.Contains(cell))
        {
          HandleTargetCellHovered(cell, SelectedPiece.data.ActiveSkill);
        }
        break;
    }
  }

  private void HandleTargetCellHovered(Cell cell, SkillSO skill)
  {
  }

  private void RefreshPieces()
  {
    foreach (var piece in CombatManager.Instance.PlayerOnBoardPieces)
    {
      if (piece.IsShadow == piece.CellUnderPiece.IsShadow)
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