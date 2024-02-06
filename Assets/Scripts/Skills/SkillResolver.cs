using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SkillResolver : MonoBehaviour
{
  private static SkillResolver instance;

  public static SkillResolver Instance
  {
    get
    {
      if (instance == null)
      {
        instance = FindObjectOfType<SkillResolver>();
        if (instance == null)
        {
          GameObject singletonObject = new GameObject();
          instance = singletonObject.AddComponent<SkillResolver>();
          singletonObject.name = "Skill Resolver";
        }
      }
      return instance;
    }
  }

  public List<Cell> GetAvailableMoves(Piece piece)
  {
    List<Cell> availableMoves = new List<Cell>();

    // BFS to find available moves
    Queue<Cell> queue = new Queue<Cell>();
    queue.Enqueue(piece.CellUnderPiece);

    // If the cell under the piece is not the same color as piece, then the piece movement is limited
    // TODO: Instead of 1, implement this to be different for each character
    int moveRange = piece.CellUnderPiece.IsShadow == piece.IsShadow ? piece.MoveRange : 1;

    Dictionary<Cell, int> distances = new Dictionary<Cell, int>();
    distances[piece.CellUnderPiece] = 0;

    // Perform breadth-first search
    while (queue.Count > 0)
    {
      Cell currentCell = queue.Dequeue();
      int currentDistance = distances[currentCell];

      if (currentDistance < moveRange)
      {

        foreach (Cell neighbor in currentCell.GetNeighbors())
        {
          if (!distances.ContainsKey(neighbor) && neighbor.IsPassable())
          {
            queue.Enqueue(neighbor);
            distances[neighbor] = currentDistance + 1;
            availableMoves.Add(neighbor);
          }
        }
      }
    }
    return availableMoves;
  }

  //? Do we need to take into account the skill's minimum targets here?
  //? Possibly ruling out the cells that are not legal targets (like empty cells?)
  //? Currently, just returning all the cells in the target range
  public List<Cell> GetAvailableTargets(Piece piece, SkillSO skill)
  {
    List<Cell> availableTargets = new List<Cell>();

    skill.TargetRange.ForEach(range =>
    {
      Vector2Int targetPosition = new Vector2Int(piece.CellUnderPiece.IndexPosition.x + range.x, piece.CellUnderPiece.IndexPosition.y + range.y);
      if (BoardManager.Instance.IsWithinBoard(targetPosition.x, targetPosition.y))
      {
        Cell targetCell = BoardManager.Instance.CurrentBoard[targetPosition.x][targetPosition.y];
        availableTargets.Add(targetCell);
      }
    });

    return availableTargets;
  }


  public void ComputeSkillEffects(SkillSO skill, Piece caster, Cell targetCell)
  {
    if (skill.IsProjectile)
    {
      ComputeProjectileSkillEffects(skill, caster, targetCell);
    }
    // else
    // {
    //   ComputeNonProjectileSkillEffects(skill, caster, targetCell);
    // }
  }

  private void ComputeProjectileSkillEffects(SkillSO skill, Piece caster, Cell targetCell)
  {
    Vector2Int sourcePosition = caster.CellUnderPiece.IndexPosition;
    Vector2Int targetPosition = targetCell.IndexPosition;

    List<Cell> path = GetPath(sourcePosition, targetPosition);
  }

  private List<Cell> GetPath(Vector2Int sourceCell, Vector2Int targetCell)
  {
    List<Cell> path = new List<Cell>();

    Vector2Int direction = targetCell - sourceCell;
    Vector2Int absDirection = new Vector2Int(Mathf.Abs(direction.x), Mathf.Abs(direction.y));

    int error = absDirection.x - absDirection.y;
    int x = sourceCell.x;
    int y = sourceCell.y;

    int xIncrement = direction.x > 0 ? 1 : -1;
    int yIncrement = direction.y > 0 ? 1 : -1;

    while (x != targetCell.x || y != targetCell.y)
    {
      path.Add(BoardManager.Instance.CurrentBoard[x][y]);
      int doubleError = error * 2;
      if (doubleError > -absDirection.y)
      {
        error -= absDirection.y;
        x += xIncrement;
      }
      if (doubleError < absDirection.x)
      {
        error += absDirection.x;
        y += yIncrement;
      }
    }
    return path;
  }


  public void ResolveSkill(SkillSO skill, Piece caster, Cell targetCell)
  {
  }




  //! We might still refer to this in the future, but for now, DEPRECATED
  // public Dictionary<Cell, List<Cell>> GetAvailableMovesWithPaths(Piece piece)
  // {
  //   Dictionary<Cell, List<Cell>> availableMovesWithPaths = new Dictionary<Cell, List<Cell>>();

  //   // Check if the piece has a cell assigned
  //   if (piece.CellUnderPiece != null)
  //   {
  //     // Create a queue to perform breadth-first search
  //     Queue<Cell> queue = new Queue<Cell>();
  //     queue.Enqueue(piece.CellUnderPiece);

  //     int moveRange = piece.CellUnderPiece.IsShadow == piece.IsShadow ? piece.MoveRange : 1;

  //     // Create a dictionary to keep track of visited cells and their distances
  //     Dictionary<Cell, int> distances = new Dictionary<Cell, int>();
  //     distances[piece.CellUnderPiece] = 0;

  //     // Create a dictionary to keep track of paths to cells
  //     Dictionary<Cell, List<Cell>> paths = new Dictionary<Cell, List<Cell>>();
  //     paths[piece.CellUnderPiece] = new List<Cell>();

  //     // Perform breadth-first search
  //     while (queue.Count > 0)
  //     {
  //       Cell currentCell = queue.Dequeue();
  //       int currentDistance = distances[currentCell];
  //       List<Cell> currentPath = paths[currentCell];

  //       // Check if the current distance is within the piece's movement range
  //       if (currentDistance <= moveRange)
  //       {
  //         // Add the current cell and its path to the dictionary of available moves with paths
  //         availableMovesWithPaths[currentCell] = currentPath;

  //         // Explore the neighboring cells
  //         foreach (Cell neighbor in currentCell.GetNeighbors())
  //         {
  //           // Check if the neighbor is not already visited and is passable
  //           if (!distances.ContainsKey(neighbor) && neighbor.IsPassable())
  //           {
  //             queue.Enqueue(neighbor);
  //             distances[neighbor] = currentDistance + 1;

  //             // Create a new path by copying the current path and adding the neighbor cell
  //             List<Cell> newPath = new List<Cell>(currentPath);
  //             newPath.Add(neighbor);
  //             paths[neighbor] = newPath;
  //           }
  //         }
  //       }
  //     }
  //   }
  //   availableMovesWithPaths.Remove(piece.CellUnderPiece);
  //   return availableMovesWithPaths;
  // }

  //! Same as above, DEPRECATED
  // public Dictionary<Cell, List<Cell>> GetAvailableTargets(Piece piece, SkillSO skill)
  // {
  //   Dictionary<Cell, List<Cell>> availableTargets = new Dictionary<Cell, List<Cell>>();
  //   if (!skill.IsProjectile) // If the skill is not a projectile (cannot be blocked)
  //   {
  //     skill.TargetRange.ForEach(range =>
  //     {
  //       Vector2Int targetPosition = new Vector2Int(piece.CellUnderPiece.IndexPosition.x + range.x, piece.CellUnderPiece.IndexPosition.y + range.y);
  //       if (BoardManager.Instance.IsWithinBoard(targetPosition.x, targetPosition.y))
  //       {
  //         Cell targetCell = BoardManager.Instance.CurrentBoard[targetPosition.x][targetPosition.y];
  //         availableTargets[targetCell] = new List<Cell>();
  //         foreach (var effectRange in skill.TargetArea)
  //         {
  //           Vector2Int effectPosition = new Vector2Int(targetPosition.x + effectRange.x, targetPosition.y + effectRange.y);
  //           if (BoardManager.Instance.IsWithinBoard(effectPosition.x, effectPosition.y))
  //           {
  //             availableTargets[targetCell].Add(BoardManager.Instance.CurrentBoard[effectPosition.x][effectPosition.y]);
  //           }
  //         }
  //       }
  //     });
  //   }
  //   else // If the skill is a projectile
  //   {
  //     List<Cell> targetCells = new List<Cell>();


  //     foreach (var range in skill.TargetRange)
  //     {
  //       Vector2Int targetPosition = new Vector2Int(piece.CellUnderPiece.IndexPosition.x + range.x, piece.CellUnderPiece.IndexPosition.y + range.y);
  //       if (BoardManager.Instance.IsWithinBoard(targetPosition.x, targetPosition.y))
  //       {
  //         Cell targetCell = BoardManager.Instance.CurrentBoard[targetPosition.x][targetPosition.y];
  //         targetCells.Add(targetCell);
  //       }
  //     }

  //     Queue<Cell> queue = new Queue<Cell>();

  //     Dictionary<Cell, List<Cell>> paths = new Dictionary<Cell, List<Cell>>();
  //     paths[piece.CellUnderPiece] = new List<Cell>();

  //     Dictionary<Cell, int> numberOfHit = new Dictionary<Cell, int>();
  //     numberOfHit[piece.CellUnderPiece] = 0;

  //     queue.Enqueue(piece.CellUnderPiece);

  //     while (queue.Count > 0)
  //     {
  //       Cell currentCell = queue.Dequeue();
  //       int currentNumberOfHit = numberOfHit[currentCell];
  //       List<Cell> currentPath = paths[currentCell];

  //       foreach (Cell neighbor in currentCell.GetNeighbors())
  //       {
  //         if (availableTargets.ContainsKey(neighbor)) continue; // If the neighbor is already a target, skip it

  //         if (targetCells.Contains(neighbor))
  //         {
  //           if (!neighbor.IsEmpty) currentNumberOfHit++;
  //           if (neighbor.GetTerrain().BlocksProjectile) currentNumberOfHit = int.MaxValue;
  //           if (currentNumberOfHit < skill.MaximumTargets)
  //           {
  //             queue.Enqueue(neighbor);
  //           }
  //           numberOfHit[neighbor] = currentNumberOfHit;
  //           availableTargets[neighbor] = currentPath;

  //           List<Cell> newPath = new List<Cell>(currentPath);
  //           newPath.Add(neighbor);
  //           paths[neighbor] = newPath;

  //           foreach (var cell in newPath)
  //           {
  //             availableTargets[cell] = newPath;
  //           }
  //         }
  //       }
  //     }
  //   }
  //   return availableTargets;
  // }
}