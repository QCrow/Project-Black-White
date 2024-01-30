public interface ICommand
{
  void Execute();
  void Undo();
}

public class DeployCommand : ICommand
{
  private Piece _piece;
  private Cell _cell;
  private bool _previousCellState;

  public DeployCommand(Piece piece, Cell cell)
  {
    _piece = piece;
    _cell = cell;
    _previousCellState = _cell.IsShadowed;
  }

  public void Execute()
  {
    _piece.CellUnderPiece = _cell;
    _piece.RedeployTimer = -1;

    CombatManager.Instance.PlayerOnBoardPieces.Add(_piece);
    CombatManager.Instance.PlayerOffBoardPieces.Remove(_piece);
  }

  public void Undo()
  {
    _piece.CellUnderPiece = BoardManager.Graveyard;
    _piece.RedeployTimer = 0;

    CombatManager.Instance.PlayerOnBoardPieces.Remove(_piece);
    CombatManager.Instance.PlayerOffBoardPieces.Add(_piece);

    _cell.IsShadowed = _previousCellState;
  }
}