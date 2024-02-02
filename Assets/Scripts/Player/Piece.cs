using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public abstract class Piece : MonoBehaviour
{
  public Character data;

  //? These stats are copied from the character data and could be modified during the combat
  //? If we don't need them, then we can remove them after
  [SerializeField] private int _maxHitPoints;
  [SerializeField] private int _maxActionPoints;
  [SerializeField] private int _unveiledActionPointRestoration;
  [SerializeField] private int _moveRange;

  [SerializeField] private int _turnsToRedeploy;




  public int CurrentHitPoints;
  public int CurrentActionPoints;

  public int RedeployTimer;
  public bool IsShadowed;
  private Cell _cellUnderPiece;
  public Cell CellUnderPiece
  {
    get => _cellUnderPiece;
    set
    {
      if (_cellUnderPiece != value)
      {
        Cell oldCell = _cellUnderPiece;
        _cellUnderPiece = value;
        OnPieceMoved(oldCell, value);
      }
    }
  }

  protected virtual void OnPieceMoved(Cell oldCell, Cell newCell)
  {
    if (oldCell != null)
    {
      oldCell.PieceOnCell = null;
    }

    newCell.PieceOnCell = this;
    BoardManager.Instance.MovePieceToCell(this.gameObject, newCell.gameObject);
  }

  public bool CanMove
  {
    get => CurrentActionPoints - MovementCost >= 0 && !ActiveEffects.ContainsKey(EffectType.Stun) && !ActiveEffects.ContainsKey(EffectType.Root);
  }

  public bool CanAttack
  {
    get => CurrentActionPoints - _baseAttack.ActionPointCost >= 0 && !ActiveEffects.ContainsKey(EffectType.Stun);
  }

  public bool CanUseSkill
  {
    get => CurrentActionPoints - _activeSkill.ActionPointCost >= 0 && !ActiveEffects.ContainsKey(EffectType.Stun);
  }

  public int MovementCost
  {
    get => 1; //TODO: To be changed for logic processing
  }

  public Dictionary<EffectType, int> ActiveEffects = new Dictionary<EffectType, int>();

  private SkillSO _baseAttack;
  private SkillSO _activeSkill;








  public virtual void Initialize()
  {
    // Assign the character's data to the piece in case they need to be modified during the game
    _maxHitPoints = data.MaxHitPoints;
    _maxActionPoints = data.MaxActionPoints;
    _unveiledActionPointRestoration = data.unveiledActionPointRestoration;
    _moveRange = data.MoveRange;
    _turnsToRedeploy = data.TurnsToRedeploy;

    CurrentHitPoints = _maxHitPoints;
    CurrentActionPoints = _maxActionPoints;

    _baseAttack = data.BaseAttack;
    _activeSkill = data.ActiveSkill;

    RedeployTimer = 0;
  }

  #region Update Piece Stats

  public void VeiledRefreshActions()
  {
    CurrentActionPoints = _maxActionPoints;
  }

  public void UnveiledRefreshActions()
  {
    CurrentActionPoints = Mathf.Clamp(CurrentActionPoints + _unveiledActionPointRestoration, 0, _maxActionPoints);
  }

  public void ExhaustActions()
  {
    CurrentActionPoints = 0;
  }
  #endregion

  public virtual Dictionary<Cell, List<Cell>> GetAvailableMovesWithPaths()
  {
    Dictionary<Cell, List<Cell>> availableMovesWithPaths = new Dictionary<Cell, List<Cell>>();

    // Check if the piece has a cell assigned
    if (CellUnderPiece != null)
    {
      // Create a queue to perform breadth-first search
      Queue<Cell> queue = new Queue<Cell>();
      queue.Enqueue(CellUnderPiece);

      int moveRange = CellUnderPiece.IsShadowed == IsShadowed ? _moveRange : 1;

      // Create a dictionary to keep track of visited cells and their distances
      Dictionary<Cell, int> distances = new Dictionary<Cell, int>();
      distances[CellUnderPiece] = 0;

      // Create a dictionary to keep track of paths to cells
      Dictionary<Cell, List<Cell>> paths = new Dictionary<Cell, List<Cell>>();
      paths[CellUnderPiece] = new List<Cell>();

      // Perform breadth-first search
      while (queue.Count > 0)
      {
        Cell currentCell = queue.Dequeue();
        int currentDistance = distances[currentCell];
        List<Cell> currentPath = paths[currentCell];

        // Check if the current distance is within the piece's movement range
        if (currentDistance <= moveRange)
        {
          // Add the current cell and its path to the dictionary of available moves with paths
          availableMovesWithPaths[currentCell] = currentPath;

          // Explore the neighboring cells
          foreach (Cell neighbor in currentCell.GetNeighbors())
          {
            // Check if the neighbor is not already visited and is passable
            if (!distances.ContainsKey(neighbor) && neighbor.IsPassable())
            {
              queue.Enqueue(neighbor);
              distances[neighbor] = currentDistance + 1;

              // Create a new path by copying the current path and adding the neighbor cell
              List<Cell> newPath = new List<Cell>(currentPath);
              newPath.Add(neighbor);
              paths[neighbor] = newPath;
            }
          }
        }
      }
    }
    availableMovesWithPaths.Remove(CellUnderPiece);
    return availableMovesWithPaths;
  }

  //TODO: This method might need to be moved to a different class
  //TODO: Diagonal projectiles and directional input still need to be handled
  public virtual Dictionary<Cell, List<Cell>> GetAvailableTargets(SkillSO skill)
  {
    Dictionary<Cell, List<Cell>> availableTargets = new Dictionary<Cell, List<Cell>>();
    if (!skill.IsProjectile) // If the skill is not a projectile (cannot be blocked)
    {
      skill.TargetRange.ForEach(range =>
      {
        Vector2Int targetPosition = new Vector2Int(CellUnderPiece.IndexPosition.x + range.x, CellUnderPiece.IndexPosition.y + range.y);
        if (BoardManager.Instance.IsWithinBoard(targetPosition.x, targetPosition.y))
        {
          Cell targetCell = BoardManager.Instance.CurrentBoard[targetPosition.x][targetPosition.y];
          availableTargets[targetCell] = new List<Cell>();
        }
      });
    }
    else // If the skill is a projectile
    {
      List<Cell> targetCells = new List<Cell>();


      foreach (var range in skill.TargetRange)
      {
        Vector2Int targetPosition = new Vector2Int(CellUnderPiece.IndexPosition.x + range.x, CellUnderPiece.IndexPosition.y + range.y);
        if (BoardManager.Instance.IsWithinBoard(targetPosition.x, targetPosition.y))
        {
          Cell targetCell = BoardManager.Instance.CurrentBoard[targetPosition.x][targetPosition.y];
          targetCells.Add(targetCell);
        }
      }

      Queue<Cell> queue = new Queue<Cell>();

      Dictionary<Cell, List<Cell>> paths = new Dictionary<Cell, List<Cell>>();
      paths[CellUnderPiece] = new List<Cell>();

      Dictionary<Cell, int> numberOfHit = new Dictionary<Cell, int>();
      numberOfHit[CellUnderPiece] = 0;

      queue.Enqueue(CellUnderPiece);

      while (queue.Count > 0)
      {
        Cell currentCell = queue.Dequeue();
        int currentNumberOfHit = numberOfHit[currentCell];
        List<Cell> currentPath = paths[currentCell];

        foreach (Cell neighbor in currentCell.GetNeighbors())
        {
          if (availableTargets.ContainsKey(neighbor)) continue; // If the neighbor is already a target, skip it

          if (targetCells.Contains(neighbor))
          {
            if (!neighbor.IsEmpty) currentNumberOfHit++;
            if (neighbor.GetTerrain().BlocksProjectile) currentNumberOfHit = int.MaxValue;
            if (currentNumberOfHit < skill.MaximumTargets)
            {
              queue.Enqueue(neighbor);
            }
            numberOfHit[neighbor] = currentNumberOfHit;
            availableTargets[neighbor] = currentPath;
            List<Cell> newPath = new List<Cell>(currentPath);
            newPath.Add(neighbor);
            paths[neighbor] = newPath;
          }
        }
      }
    }
    return availableTargets;
  }
}