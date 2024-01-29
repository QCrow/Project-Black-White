public interface ICommand
{
  void Execute();
  void Undo();
}

public class PlacePieceCommand : ICommand
{
  private Piece _piece;
  private Cell _cell;
  private bool _previousCellState;

  public PlacePieceCommand(Piece piece, Cell cell)
  {
    _piece = piece;
    _cell = cell;
    _previousCellState = _cell.IsShadowed;
  }

  public void Execute()
  {
    _piece.CellUnderPiece = _cell;
    CombatManager.Instance.PlayerOnBoardPieces.Add(_piece);
  }

  public void Undo()
  {
    _piece.CellUnderPiece = BoardManager.Graveyard;
    CombatManager.Instance.PlayerOnBoardPieces.Remove(_piece);

    _cell.IsShadowed = _previousCellState;
  }
}