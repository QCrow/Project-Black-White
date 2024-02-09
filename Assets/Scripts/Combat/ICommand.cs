public interface ICommand
{
    void Execute();
    void Undo();
}

public class DeployCommand : ICommand
{
    private Ally _piece;
    private Cell _cell;
    private bool _previousCellState;

    public DeployCommand(Ally piece, Cell cell)
    {
        _piece = piece;
        _cell = cell;
        _previousCellState = _cell.IsShadow;
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

        _cell.IsShadow = _previousCellState;
    }
}

public class MoveCommand : ICommand
{
    private Ally _piece;
    private Cell _oldCell;
    private Cell _newCell;
    private int _previousActionPoints;

    public MoveCommand(Ally piece, Cell newCell)
    {
        _piece = piece;
        _oldCell = piece.CellUnderPiece;
        _newCell = newCell;
    }

    public void Execute()
    {
        _piece.CellUnderPiece = _newCell;
        _piece.CurrentActionPoints -= _piece.MovementCost;
    }

    public void Undo()
    {
        _piece.CellUnderPiece = _oldCell;
        _piece.CurrentActionPoints = _previousActionPoints;
    }
}