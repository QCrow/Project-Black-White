using UnityEngine;

public class Cell : MonoBehaviour
{
  private bool _isShadowed = false;
  public bool IsShadowed
  {
    get => _isShadowed;
    set
    {
      if (_isShadowed != value)
      {
        _isShadowed = value;
        UpdateCellDisplay();
        BoardManager.Instance.UpdateShadowedCellsCount(_isShadowed);
      }
    }
  }

  [SerializeField] private Color color = Color.white;

  public Vector2Int IndexPosition = new(0, 0);
  public Piece PieceOnCell;

  private Renderer _renderer;
  private MaterialPropertyBlock _propBlock;

  public override string ToString()
  {
    return $"Cell [{IndexPosition.x}, {IndexPosition.y}]";
  }

  private void Awake()
  {
    _renderer = GetComponent<Renderer>();
    _propBlock = new MaterialPropertyBlock();
  }

  public void Initialize(int x, int y)
  {
    SetIndexPosition(x, y);
    UpdateCellDisplay();
  }

  public void SetIndexPosition(int x, int y)
  {
    IndexPosition.Set(x, y);
  }

  public void UpdateCellDisplay()
  {
    _renderer.GetPropertyBlock(_propBlock);
    _propBlock.SetFloat("_IsInverted", IsShadowed ? 1.0f : 0.0f); // Assuming _IsInverted is your boolean property in Shader Graph
    _propBlock.SetColor("_Color", color);
    _renderer.SetPropertyBlock(_propBlock);
  }
}
