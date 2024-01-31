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
      CombatManager.Instance.ChangeState(new PlayerCombatState());
    }
  }

  private void HandleCellSelected(Cell cell)
  {
    ICommand command;

    Piece pieceOnCell = null; // Existing piece on the cell
    if (cell.PieceOnCell != null)
    {
      pieceOnCell = cell.PieceOnCell;
      int foundIndex = CombatManager.Instance.PlayerOnBoardPieces.IndexOf(pieceOnCell);
      if (foundIndex >= 0)
      {
        // Remove the piece from the player pieces list
        command = _commands[foundIndex];
        CombatManager.Instance.Invoker.SetCommand(command);
        CombatManager.Instance.Invoker.UndoCommand();
        _commands.RemoveAt(foundIndex);

        CombatManager.Instance.PlayerOffBoardPieces.Add(pieceOnCell);
      }
    }
    if (CombatManager.Instance.PlayerOffBoardPieces.Count == 0) return;

    Piece pieceToPlace = CombatManager.Instance.PlayerOffBoardPieces[0];
    if (pieceToPlace == pieceOnCell) return; // If the place to place is the same piece that we just removed, do nothing
    if (pieceToPlace.IsShadowed != cell.IsShadowed) return;

    command = new DeployCommand(pieceToPlace, cell);
    CombatManager.Instance.Invoker.SetCommand(command);
    CombatManager.Instance.Invoker.ExecuteCommand();

    _commands.Add(command);
    CombatManager.Instance.PlayerOffBoardPieces.Remove(pieceToPlace);

  }
}