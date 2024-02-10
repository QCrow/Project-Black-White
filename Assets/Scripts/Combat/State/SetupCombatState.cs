using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class SetupCombatState : ICombatState
{
    private List<ICommand> _commands;
    public bool ConfirmedForNextState;

    public void EnterState()
    {
        ConfirmedForNextState = false;

        _commands = new List<ICommand>();

        InputManager.OnCellSelected += HandleCellSelected;
    }

    public void ExitState()
    {
        InputManager.OnCellSelected -= HandleCellSelected;
    }

    public void UpdateState()
    {
        // Enable or disable the confirm button
        UIManager.Instance.SetConfirmButtonInteractable(CombatManager.Instance.PlayerOffBoardPieces.Count == 0);
        if (ConfirmedForNextState)
        {
            UIManager.Instance.SetConfirmButtonActive(false);
            CombatManager.Instance.ChangeState(new PlayerCombatState());
        }
    }

    private void HandleCellSelected(Cell cell)
    {
        ICommand command;
        Ally playerPieceOnCell = null;
        if (cell.PieceOnCell != null)
        {
            if (!CombatManager.Instance.PlayerOnBoardPieces.Contains(cell.PieceOnCell)) return;

            playerPieceOnCell = (Ally)cell.PieceOnCell;

            int foundIndex = CombatManager.Instance.PlayerOnBoardPieces.IndexOf(playerPieceOnCell);
            if (foundIndex >= 0)
            {
                // Remove the piece from the player pieces list
                command = _commands[foundIndex];
                CombatManager.Instance.Invoker.SetCommand(command);
                CombatManager.Instance.Invoker.UndoCommand();
                _commands.RemoveAt(foundIndex);

                CombatManager.Instance.PlayerOffBoardPieces.Add(playerPieceOnCell);
            }
        }
        if (CombatManager.Instance.PlayerOffBoardPieces.Count == 0) return;

        Ally playerPieceToPlace = CombatManager.Instance.PlayerOffBoardPieces[0];
        if (playerPieceToPlace == playerPieceOnCell) return; // If the place to place is the same piece that we just removed, do nothing
        if (playerPieceToPlace.IsShadow != cell.IsShadow) return;

        command = new DeployCommand(playerPieceToPlace, cell);
        CombatManager.Instance.Invoker.SetCommand(command);
        CombatManager.Instance.Invoker.ExecuteCommand();

        _commands.Add(command);
        CombatManager.Instance.PlayerOffBoardPieces.Remove(playerPieceToPlace);
    }
}