using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;

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
  private List<Cell> _highlightedCells = new List<Cell>();
  public List<Cell> HighlightedCells
  {
    get => _highlightedCells;
    set
    {
      if (_highlightedCells != value)
      {
        _highlightedCells = value;
        foreach (var cell in _highlightedCells)
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
        _actionSelectionState = value;
        switch (_actionSelectionState)
        {
          case ActionSelectionState.None:
            foreach (var cell in HighlightedCells)
            {
              cell.RemoveHighlight();
            }
            HighlightedCells.Clear();
            break;
          case ActionSelectionState.Move:
            HighlightedCells = new List<Cell>(_selectedPiece.GetAvailableMovesWithPaths().Keys);
            break;
          case ActionSelectionState.Attack:
            // HighlightedCells = _selectedPiece.GetAvailableAttackCells();
            break;
          case ActionSelectionState.Skill:
            // HighlightedCells = _selectedPiece.GetAvailableSkillCells();
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
  }

  public void UpdateState()
  {

  }

  public void ExitState()
  {
    InputManager.OnCellSelected -= HandleCellSelected;
  }

  private void HandleCellSelected(Cell cell)
  {
    if (cell.PieceOnCell != null)
    {
      _selectedPiece = cell.PieceOnCell;
      if (_selectedPiece.IsMovable)
      {
        ActionSelectionState = ActionSelectionState.Move;
      }
      else
      {
        // TODO: Show other actions than move
        // HighlightedCells = _selectedPiece.GetAvailableActions();
      }
    }
    else
    {
      if (HighlightedCells.Contains(cell)) // We are clicking a highlighted cell, do the action that is currently selected
      {
        switch (ActionSelectionState)
        {
          case ActionSelectionState.Move:
            MoveCommand moveCommand = new MoveCommand(_selectedPiece, cell);
            _moveCommandPerPiece.Add(_selectedPiece, moveCommand);
            _allCommands.Add(moveCommand);
            CombatManager.Instance.Invoker.SetCommand(moveCommand);
            CombatManager.Instance.Invoker.ExecuteCommand();
            break;
          case ActionSelectionState.Attack:
            // _selectedPiece.AttackCell(cell);
            break;
          case ActionSelectionState.Skill:
            // _selectedPiece.UseSkillOnCell(cell);
            break;
        }
        _selectedPiece = null;
        _actionSelectionState = ActionSelectionState.None;
      }
      else // We are clicking a cell that is not highlighted, cancel the selection
      {
        _selectedPiece = null;
        _actionSelectionState = ActionSelectionState.None;
      }
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