using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public abstract class Piece : MonoBehaviour
{
    public bool IsShadow { get; set; }

    protected int _moveRange;

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

    public int x => CellUnderPiece.IndexPosition.x;
    public int y => CellUnderPiece.IndexPosition.y;

    protected int _maxHitPoints = 100;
    [SerializeField] protected int _currentHitPoints;

    public void TakeDamage(int damage)
    {
        _currentHitPoints -= damage;
        if (_currentHitPoints <= 0)
        {
            //TODO: Die
            // Die();
        }
    }

    public void Heal(int heal)
    {
        _currentHitPoints += heal;
        if (_currentHitPoints > _maxHitPoints)
        {
            _currentHitPoints = _maxHitPoints;
        }
    }

    public void ResetHealth()
    {
        _currentHitPoints = _maxHitPoints;
    }

    public virtual void Initialize()
    {
        ResetHealth();
    }

    public virtual List<Cell> GetPossibleMoves()
    {
        List<Cell> availableMoves = new List<Cell>();

        // BFS to find available moves
        Queue<Cell> queue = new Queue<Cell>();
        queue.Enqueue(CellUnderPiece);

        // If the cell under the piece is not the same color as piece, then the piece movement is limited
        // TODO: Instead of 1, implement this to be different for each character
        int moveRange = CellUnderPiece.IsShadow == IsShadow ? _moveRange : 1;

        Dictionary<Cell, int> distances = new Dictionary<Cell, int>();
        distances[CellUnderPiece] = 0;

        // Perform breadth-first search
        while (queue.Count > 0)
        {
            Cell currentCell = queue.Dequeue();
            int currentDistance = distances[currentCell];

            if (currentDistance < moveRange)
            {

                foreach (Cell neighbor in currentCell.GetNeighbors())
                {
                    if (!distances.ContainsKey(neighbor) && neighbor.IsPassable)
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
}