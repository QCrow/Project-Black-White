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
    private Ally _selectedAlly;

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
                        _selectedAlly = null;
                        // If the state is set to None, clear the lists
                        _cellsInRange = null;
                        _selectedTargets = null;
                        break;

                    case ActionSelectionState.Move:
                        _cellsInRange = _selectedAlly.GetPossibleMoves();
                        break;
                    case ActionSelectionState.Attack:
                        _cellsInRange = _selectedAlly.BaseAttack.SelectTarget(_selectedAlly, new()); //? Could there be a case where the initial _cellsInRange is emtpy?
                        break;
                    case ActionSelectionState.Skill:
                        _cellsInRange = _selectedAlly.ActiveSkill.SelectTarget(_selectedAlly, new()); //? Could there be a case where the initial _cellsInRange is emtpy?
                        break;
                }
            }
        }
    }

    // These lists are used to for target selection
    private List<Cell> _cellsInRange;
    private List<Target> _selectedTargets;


    public void EnterState()
    {
        RefreshPieces();
        DecreaseRedeployTimers();

        // Add the event listeners
        InputManager.OnCellSelected += HandleCellSelected;
        InputManager.OnCellHovered += HandleCellHovered;
    }

    public void UpdateState()
    {

    }

    public void ExitState()
    {
        // Remove the event listeners
        InputManager.OnCellSelected -= HandleCellSelected;
        InputManager.OnCellHovered -= HandleCellHovered;
    }

    private void HandleCellSelected(Cell cell)
    {
        if (_cellsInRange.Contains(cell)) // We are clicking a highlighted cell, do the action that is currently selected
        {
            switch (ActionSelectionState)
            {
                case ActionSelectionState.Move:
                    MoveCommand moveCommand = new MoveCommand(_selectedAlly, cell);
                    moveCommand.Execute(); //? We might want to use the invoker and store the commands to be able to undo them
                    ActionSelectionState = ActionSelectionState.None;
                    break;
                case ActionSelectionState.Attack:
                    _cellsInRange = _selectedAlly.BaseAttack.SelectTarget(cell.PieceOnCell, _selectedTargets);
                    if (_cellsInRange.Count > 0)
                    {
                        ActionSelectionState = ActionSelectionState.Attack;
                    }
                    else
                    {
                        ActionSelectionState = ActionSelectionState.None;
                    }
                    break;
                case ActionSelectionState.Skill:
                    _cellsInRange = _selectedAlly.ActiveSkill.SelectTarget(cell.PieceOnCell, _selectedTargets);
                    if (_cellsInRange.Count > 0)
                    {
                        ActionSelectionState = ActionSelectionState.Skill;
                    }
                    else
                    {
                        ActionSelectionState = ActionSelectionState.None;
                    }
                    break;
                default:
                    throw new Exception("Unexpected ActionSelectionState");
            }
        }
        else
        {
            if (!cell.IsEmpty)
            {
                ActionSelectionState = ActionSelectionState.None;

                if (cell.PieceOnCell is not Ally) return; // If the piece is not an ally, do nothing
                _selectedAlly = (Ally)cell.PieceOnCell;

                UIManager.Instance.SetAttackButtonInteractable(_selectedAlly.CanAttack);
                UIManager.Instance.SetSkillButtonInteractable(_selectedAlly.CanUseSkill);

                ActionSelectionState = ActionSelectionState.Move;
            }
            else
            {
                ActionSelectionState = ActionSelectionState.None;
            }
        }
    }

    private void HandleCellHovered(Cell cell)
    {
    }

    private void RefreshPieces()
    {
        foreach (var piece in CombatManager.Instance.PlayerOnBoardPieces)
        {
            if (piece.IsShadow == piece.CellUnderPiece.IsShadow) //? We might want to implement a compute field called IsVeiled instead of comparing the shadow status
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