using System;
using System.Collections.Generic;

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

    public Ally SelectedAlly
    {
        get => _selectedAlly;
        set
        {
            if (_selectedAlly != value)
            {
                _selectedAlly = value;
                if (_selectedAlly != null)
                {
                    UIManager.Instance.SetAttackButtonInteractable(_selectedAlly.CanAttack);
                    UIManager.Instance.SetSkillButtonInteractable(_selectedAlly.CanUseSkill);
                }
                else
                {
                    UIManager.Instance.SetAttackButtonInteractable(false);
                    UIManager.Instance.SetSkillButtonInteractable(false);
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
                        SelectedAlly = null;
                        // If the state is set to None, clear the lists
                        CellsInRange = new();
                        _selectedTargets = new();
                        break;

                    case ActionSelectionState.Move:
                        CellsInRange = SelectedAlly.GetPossibleMoves();
                        break;
                    case ActionSelectionState.Attack:
                        CellsInRange = SelectedAlly.BaseAttack.SelectTarget(SelectedAlly, new()); //? Could there be a case where the initial CellsInRange is emtpy?
                        break;
                    case ActionSelectionState.Skill:
                        CellsInRange = SelectedAlly.ActiveSkill.SelectTarget(SelectedAlly, new()); //? Could there be a case where the initial CellsInRange is emtpy?
                        break;
                }
            }
        }
    }

    private TargetType ValidTargetType()
    {
        if (ActionSelectionState == ActionSelectionState.Move)
        {
            return TargetType.Empty;
        }
        else if (ActionSelectionState == ActionSelectionState.Attack)
        {
            return SelectedAlly.BaseAttack.ValidTargetType[SelectedTargets.Count];
        }
        else if (ActionSelectionState == ActionSelectionState.Skill)
        {
            return SelectedAlly.ActiveSkill.ValidTargetType[SelectedTargets.Count];
        }
        else
        {
            throw new Exception("Unexpected ActionSelectionState");
        }
    }

    // These lists are used to for target selection
    private List<Cell> _cellsInRange = new();
    public List<Cell> CellsInRange
    {
        get => _cellsInRange;
        set
        {
            foreach (var cell in _cellsInRange)
            {
                cell.State = CellState.Normal;
            }
            _cellsInRange = value;
            foreach (var cell in _cellsInRange)
            {
                cell.State = CellState.InRange;
            }
        }
    }

    private List<Target> _selectedTargets = new();
    public List<Target> SelectedTargets
    {
        get => _selectedTargets;
        set
        {
            if (_selectedTargets.Count > 0)
                _selectedTargets[-1].TargetCell.State = CellState.Normal;
            _selectedTargets = value;
            if (_selectedTargets.Count > 0)
                _selectedTargets[-1].TargetCell.State = CellState.Selected;
        }
    }

    public void EnterState()
    {
        RefreshPieces();
        DecreaseRedeployTimers();
        ActionSelectionState = ActionSelectionState.None;

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
                    MoveCommand moveCommand = new MoveCommand(SelectedAlly, cell);
                    moveCommand.Execute(); //? We might want to use the invoker and store the commands to be able to undo them
                    Ally currentAlly = SelectedAlly;
                    ActionSelectionState = ActionSelectionState.None;
                    SelectedAlly = currentAlly;
                    ActionSelectionState = ActionSelectionState.Move;
                    break;
                case ActionSelectionState.Attack:
                    SelectedTargets.Add(new Target(cell, cell.PieceOnCell));
                    CellsInRange = SelectedAlly.BaseAttack.SelectTarget(SelectedAlly, SelectedTargets);
                    if (CellsInRange.Count > 0)
                    {
                        ActionSelectionState = ActionSelectionState.Attack;
                    }
                    else
                    {
                        ActionSelectionState = ActionSelectionState.None;
                    }
                    break;
                case ActionSelectionState.Skill:
                    SelectedTargets.Add(new Target(cell, cell.PieceOnCell));
                    CellsInRange = SelectedAlly.ActiveSkill.SelectTarget(SelectedAlly, SelectedTargets);
                    if (CellsInRange.Count > 0)
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
                SelectedAlly = (Ally)cell.PieceOnCell;

                UIManager.Instance.SetAttackButtonInteractable(SelectedAlly.CanAttack);
                UIManager.Instance.SetSkillButtonInteractable(SelectedAlly.CanUseSkill);

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
        if (CellsInRange.Contains(cell))
        {
            if (new Target(cell).IsValidTarget(SelectedAlly, ValidTargetType()))
                cell.Hovered();
        }
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