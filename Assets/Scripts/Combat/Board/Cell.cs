using System;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
  private bool _isShadow = false;
  public bool IsShadow
  {
    get => _isShadow;
    set
    {
      if (_isShadow != value)
      {
        _isShadow = value;
        UpdateCellDisplay();
        BoardManager.Instance.UpdateShadowedCellsCount(_isShadow);
      }
    }
  }

  private Color color = Color.white;

  public Vector2Int IndexPosition = new(0, 0);
  public Piece PieceOnCell;

  public bool IsEmpty => PieceOnCell == null;

  private Renderer _renderer;
  private MaterialPropertyBlock _propBlock;

  private bool _isHovered = false;
  public bool IsHovered
  {
    get => _isHovered;
    set
    {
      if (_isHovered != value)
      {
        _isHovered = value;
      }
    }
  }
  private bool _isHighlighted = false;
  private Color _currentHighlightColor;

  public override string ToString()
  {
    return $"Cell [{IndexPosition.x}, {IndexPosition.y}]";
  }

  private void Awake()
  {
    _renderer = GetComponent<Renderer>();
    _propBlock = new MaterialPropertyBlock();
  }

  //? This might be changed in the future, since different terrains might need different prefabs, so the logic should be handled at instantiation
  public void Initialize(int x, int y, TerrainType terrainType, bool isShadowed)
  {
    SetIndexPosition(x, y);
    this._isShadow = isShadowed;
    SetTerrain(terrainType);
    if (isShadowed)
    {
      BoardManager.Instance.UpdateShadowedCellsCount(true);
    }
    UpdateCellDisplay();
  }

  private void SetTerrain(TerrainType terrainType)
  {
    if (GetComponent<Terrain>() != null)
    {
      Destroy(GetComponent<Terrain>());
    }


    switch (terrainType)
    {
      case TerrainType.Base:
        this.gameObject.AddComponent<BaseCellTerrain>();
        break;
    }
  }

  public CellTerrain GetTerrain()
  {
    return GetComponent<CellTerrain>();
  }

  public void SetIndexPosition(int x, int y)
  {
    IndexPosition.Set(x, y);
  }

  public void UpdateCellDisplay()
  {
    _renderer.GetPropertyBlock(_propBlock);
    _propBlock.SetFloat("_IsInverted", IsShadow ? 1.0f : 0.0f); // Assuming _IsInverted is your boolean property in Shader Graph
    _propBlock.SetColor("_Color", color);
    _renderer.SetPropertyBlock(_propBlock);
  }

  public List<Cell> GetNeighbors()
  {
    List<Cell> neighbors = new List<Cell>();
    if (IndexPosition.x > 0)
    {
      neighbors.Add(BoardManager.Instance.CurrentBoard[IndexPosition.x - 1][IndexPosition.y]);
    }
    if (IndexPosition.x < BoardManager.Instance.Size - 1)
    {
      neighbors.Add(BoardManager.Instance.CurrentBoard[IndexPosition.x + 1][IndexPosition.y]);
    }
    if (IndexPosition.y > 0)
    {
      neighbors.Add(BoardManager.Instance.CurrentBoard[IndexPosition.x][IndexPosition.y - 1]);
    }
    if (IndexPosition.y < BoardManager.Instance.Size - 1)
    {
      neighbors.Add(BoardManager.Instance.CurrentBoard[IndexPosition.x][IndexPosition.y + 1]);
    }
    return neighbors;
  }

  public bool IsPassable()
  {
    return IsEmpty && GetComponent<CellTerrain>().IsPassable;
  }

  public bool BlocksProjectile()
  {
    return !IsEmpty || GetComponent<CellTerrain>().BlocksProjectile;
  }

  public void SetHighlight(Color color)
  {
    Transform highlight = transform.Find("Highlight");
    if (highlight != null)
    {
      Renderer highlightRenderer = highlight.GetComponent<Renderer>();
      if (highlightRenderer != null)
      {
        MaterialPropertyBlock highlightPropBlock = new MaterialPropertyBlock();
        highlightRenderer.GetPropertyBlock(highlightPropBlock);
        highlightPropBlock.SetColor("_Color", color);
        highlightRenderer.SetPropertyBlock(highlightPropBlock);
      }
      highlight.gameObject.SetActive(true);
      _isHighlighted = true;
      _currentHighlightColor = color;
    }
  }

  public void RemoveHighlight()
  {
    Transform highlight = transform.Find("Highlight");
    if (highlight != null)
    {
      highlight.gameObject.SetActive(false);
    }
    _isHighlighted = false;
  }


  public void ToggleHovered()
  {
    _isHovered = !_isHovered;
  }

  public void ToggleTargetedHovered()
  {
    _isHovered = !_isHovered;
    if (_isHovered)
    {
      SetHighlight(Color.yellow);

    }
    else
    {
      if (_isHighlighted)
      {
        SetHighlight(Color.red);
      }
      else
      {
        RemoveHighlight();
      }
    }
  }
}
