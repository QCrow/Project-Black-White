using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SetupState : ICombatState
{
  private Queue<Piece> _piecesToPlace;

  // TODO: Data structure to hold previous commands to undo
  private List<ICommand> _commands;
  public void EnterState(CombatManager manager)
  {

    // Set the pieces to place
    _piecesToPlace = new Queue<Piece>();
    foreach (Piece piece in CombatManager.Instance.PlayerPieces)
    {
      _piecesToPlace.Enqueue(piece);
    }

    _commands = new List<ICommand>();

    InputManager.OnCellSelected += HandleCellSelected;
  }

  public void ExitState(CombatManager manager)
  {
    InputManager.OnCellSelected -= HandleCellSelected;
  }

  public void UpdateState(CombatManager manager)
  {
  }

  private void HandleCellSelected(Cell cell)
  {
    ICommand command;
    if (cell.PieceOnCell == null)
    {
      if (_piecesToPlace.Count == 0) return;
      Piece pieceToPlace = _piecesToPlace.Dequeue();
      // Debug.Log(pieceToPlace);
      command = new PlacePieceCommand(pieceToPlace, cell);
      CombatManager.Instance.Invoker.SetCommand(command);
      CombatManager.Instance.Invoker.ExecuteCommand();

      _commands.Add(command);
    }
    else
    {
      Piece piece = cell.PieceOnCell;
      int foundIndex = CombatManager.Instance.AlivePieces.IndexOf(piece);
      if (foundIndex >= 0)
      {
        // Remove the piece from the player pieces list
        command = _commands[foundIndex];
        CombatManager.Instance.Invoker.SetCommand(command);
        CombatManager.Instance.Invoker.UndoCommand();
        _commands.RemoveAt(foundIndex);

        _piecesToPlace.Enqueue(piece);
      }
    }
  }
}