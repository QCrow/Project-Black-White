using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum TargetType
{
    Self,
    Ally,
    Enemy,
    NonEmpty,
    Empty,
    Any
}

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

    public Target(Cell targetCell)
    {
        TargetCell = targetCell;
        TargetPiece = targetCell.PieceOnCell;
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

public static class TargetingHelper
{
    public static bool IsWithinBoard(int x, int y)
    {
        return x >= 0 && x < BoardManager.Instance.Size && y >= 0 && y < BoardManager.Instance.Size;
    }

    public static List<Cell> GetCellsWithRelativeRange(Cell origin, List<Vector2Int> relativeRange)
    {
        List<Cell> cells = new List<Cell>();
        foreach (Vector2Int relativePosition in relativeRange)
        {
            int x = origin.IndexPosition.x + relativePosition.x;
            int y = origin.IndexPosition.y + relativePosition.y;
            if (IsWithinBoard(x, y))
            {
                cells.Add(BoardManager.Instance.CurrentBoard[x][y]);
            }
        }
        return cells;
    }

    public static List<Cell> GetProjectileRange(Cell origin, List<Vector2Int> projectileDirection, int projectileRange, int piercingPower = 0)
    {
        List<Cell> cells = new List<Cell>();
        foreach (Vector2Int direction in projectileDirection)
        {
            int piercingCounter = 0; // This counter is used to keep track of how many pieces the projectile has pierced
            for (int i = 1; i <= projectileRange; i++)
            {
                int x = origin.IndexPosition.x + direction.x * i;
                int y = origin.IndexPosition.y + direction.y * i;
                if (IsWithinBoard(x, y))
                {
                    Cell cell = BoardManager.Instance.CurrentBoard[x][y];
                    cells.Add(cell); //? Right now, the range also include the hit cell, but it might be changed in the future
                    if (!cell.IsEmpty)
                    {
                        piercingCounter++;
                        if (piercingCounter > piercingPower)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    break;
                }
            }
        }
        return cells;
    }
}

