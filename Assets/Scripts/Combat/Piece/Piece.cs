using System.Collections.Generic;
using UnityEngine;

public abstract class Piece : MonoBehaviour
{
  public bool IsShadow { get; set; }

  public int MoveRange { get; set; }

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


  protected int _maxHitPoints = 100;
  protected int _currentHitPoints;

  public void takeDamage(int damage)
  {
    _currentHitPoints -= damage;
    if (_currentHitPoints <= 0)
    {
      //TODO: Die
      // Die();
    }
  }

  public void heal(int heal)
  {
    _currentHitPoints += heal;
    if (_currentHitPoints > _maxHitPoints)
    {
      _currentHitPoints = _maxHitPoints;
    }
  }

  public void resetHealth()
  {
    _currentHitPoints = _maxHitPoints;
  }

  protected Dictionary<EffectType, int> ActiveEffects = new Dictionary<EffectType, int>();


  public virtual void Initialize()
  {
    resetHealth();
  }
}