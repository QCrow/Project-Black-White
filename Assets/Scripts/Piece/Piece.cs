using System.Collections.Generic;
using UnityEngine;

public abstract class Piece : MonoBehaviour
{
  [SerializeField] public RangeSO PossibleMoves;
  [SerializeField] public List<Skill> Skills;
  [SerializeField] protected int _hitPoints;


  public bool IsShadowed;
  private Cell _cellUnderPiece;
  protected int _currentHitPoints;

  public bool HasMoved { get; set; }
  public bool HasAttacked { get; set; }
  public bool IsExhausted
  {
    get
    {
      return HasMoved && HasAttacked;
    }
  }

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

  private void Start()
  {
    Initialize();
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

    newCell.PieceOnCell = this;
    BoardManager.Instance.MovePieceToCell(this.gameObject, newCell.gameObject);
  }

  public virtual void RefreshActions()
  {
    this.HasMoved = false;
    this.HasAttacked = false;
  }

  public virtual void ExhaustActions()
  {
    this.HasMoved = true;
    this.HasAttacked = true;
  }
}