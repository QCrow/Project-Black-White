using UnityEngine;

public class PieceFactory : MonoBehaviour
{
    public static PieceFactory Instance { get; private set; }

    [SerializeField] GameObject _enforcerPrefab;

    [SerializeField] GameObject _sharpshooterPrefab;

    [SerializeField] GameObject _artilleryPrefab;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;
    }

    private GameObject GetPrefabFromType(AllyType pieceType)
    {
        switch (pieceType)
        {
            case AllyType.Enforcer:
                return _enforcerPrefab;
            case AllyType.Sharpshooter:
                return _sharpshooterPrefab;
            case AllyType.Artillery:
                return _artilleryPrefab;
            default:
                return null;
        }
    }

    public Ally CreatePiece(Character character)
    {
        GameObject prefab = GetPrefabFromType(character.Type);
        Ally allyPiece = Instantiate(prefab).GetComponent<Ally>();
        allyPiece.data = character;
        allyPiece.Initialize();
        return allyPiece;
    }
}