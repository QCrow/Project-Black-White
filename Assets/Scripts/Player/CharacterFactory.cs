using UnityEngine;

public class CharacterFactory : MonoBehaviour
{
  public static CharacterFactory Instance { get; private set; }
  [SerializeField] GameObject _enforcerPrefab;

  [SerializeField] GameObject _sharpshooterPrefab;

  [SerializeField] GameObject _artilleryPrefab;

  private void Awake()
  {
    if (Instance == null)
    {
      Instance = this;
    }
    else if (Instance != this)
    {
      Destroy(gameObject); // Destroy any duplicate instances.
    }
  }

  private GameObject getPrefabFromType(PieceType pieceType)
  {
    switch (pieceType)
    {
      case PieceType.Enforcer:
        return _enforcerPrefab;
      case PieceType.Sharpshooter:
        return _sharpshooterPrefab;
      case PieceType.Artillery:
        return _artilleryPrefab;
      default:
        return null;
    }
  }

  public Piece CreatePiece(PieceType pieceType)
  {
    GameObject prefab = getPrefabFromType(pieceType);
    Piece piece = Instantiate(prefab).GetComponent<Piece>();
    return piece;
  }
}