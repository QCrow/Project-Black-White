using UnityEngine;

public class Enemy : MonoBehaviour
{
  public int CurrentHitPoints = 100;

  private Cell _cellUnderEnemy;
  public Cell CellUnderEnemy
  {
    get => _cellUnderEnemy;
    set
    {
      if (_cellUnderEnemy != value && value.PieceOnCell == null)
      {
        Cell oldCell = _cellUnderEnemy;
        _cellUnderEnemy = value;
        OnEnemyMoved(oldCell, value);
      }
      {
        Cell oldCell = _cellUnderEnemy;
        _cellUnderEnemy = value;
        OnEnemyMoved(oldCell, value);
      }
    }
  }
  protected virtual void OnEnemyMoved(Cell oldCell, Cell newCell)
  {
    if (oldCell != null)
    {
      oldCell.EnemyOnCell = null;
    }

    newCell.EnemyOnCell = this;
    BoardManager.Instance.MovePieceToCell(this.gameObject, newCell.gameObject);
  }
}