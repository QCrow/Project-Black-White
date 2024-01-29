using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class Piece : MonoBehaviour
{
  private int _moveRange;
  public CharacterDataSO data;
  protected int _maxHitPoints;
  protected int _currentHitPoints;

  protected int _maxActionPoints;
  protected int _currentActionPoints;

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

  public bool HasMoved { get; set; }
  public bool HasAttacked { get; set; }
  public bool IsExhausted
  {
    get
    {
      return HasMoved && HasAttacked;
    }
  }

  protected virtual void Initialize()
  {
  }

  protected virtual void OnPieceMoved(Cell oldCell, Cell newCell)
  {
    if (oldCell != null)
    {
      oldCell.PieceOnCell = null;
    }
    Debug.Log(newCell);
    newCell.PieceOnCell = this;
    BoardManager.Instance.MovePieceToCell(this.gameObject, newCell.gameObject);
  }

  public virtual void RefreshActions()
  {
    this._currentActionPoints = this._maxActionPoints;
  }

  public virtual void ExhaustActions()
  {
    this._currentActionPoints = 0;
  }

  public virtual void UseActions(int usedActionPoints)
  {
    this._currentActionPoints -= usedActionPoints;
  }
}