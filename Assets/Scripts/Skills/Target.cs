public class Target
{
    public Cell TargetCell;
    public Piece TargetPiece;

    public int x => TargetCell.IndexPosition.x;
    public int y => TargetCell.IndexPosition.y;

    public Target(Cell targetCell, Piece targetPiece)
    {
        TargetCell = targetCell;
        TargetPiece = targetPiece;
    }

    public bool IsValidTarget(Piece caster, TargetType targetType)
    {
        switch (targetType)
        {
            case TargetType.Ally:
                return TargetPiece is Ally;
            case TargetType.Enemy:
                return TargetPiece is Enemy;
            case TargetType.Self:
                return TargetPiece == caster;
            case TargetType.NonEmpty:
                return TargetPiece != null;
            case TargetType.Empty:
                return TargetPiece == null;
            case TargetType.Any:
                return true;
            default:
                throw new System.Exception("Unexpected TargetType");
        }
    }
}