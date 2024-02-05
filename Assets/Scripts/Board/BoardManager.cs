using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
  public static BoardManager Instance { get; private set; }
  public static Cell Graveyard;
  public GameObject CellPrefab;

  public BoardSetupSO BoardSetup;

  private List<List<bool>> _cellColors = new List<List<bool>>(); // TODO: This is just a temporary solution to make sure that the board is initialized correctly, this should be placed in BoardSetupSO eventually

  public int Size = 8;
  public List<List<Cell>> CurrentBoard;

  public int ShadowedCellsCount;

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

    // Instantiate faraway graveyard cell
    GameObject graveyardObject = Instantiate(CellPrefab, new Vector3(1000f, 1000f, 1000f), Quaternion.identity);
    Graveyard = graveyardObject.GetComponent<Cell>();
    Graveyard.Initialize(-1, -1, TerrainType.Base, true);
    graveyardObject.name = "Graveyard";

    // TODO: This entire region's utility should be implemented into a BoardSetupSO data structure eventually
    #region TO BE CHANGED
    for (int i = 0; i < Size; i++)
    {
      List<bool> row = new List<bool>(new bool[Size]);
      for (int j = Size / 2; j < Size; j++)
      {
        row[j] = true;
      }
      _cellColors.Add(row);
    }
    // Now we are just setting up a mock cellColors List for testing purposes and mimicking the BoardSetupSO
    #endregion
  }

  public void InitializeBoard()
  {
    ShadowedCellsCount = 0;

    // Create the board structure
    CurrentBoard = new List<List<Cell>>();

    // Calculate the center offset
    float offset = Size / 2;
    bool isSizeOdd = Size % 2 == 1;

    // Populate the board with cells
    for (int i = 0; i < Size; i++)
    {
      List<Cell> row = new List<Cell>();
      for (int j = 0; j < Size; j++)
      {
        // Calculate the position of the cell in world space
        Vector3 position = isSizeOdd
            ? new Vector3(-offset + i, 0, offset - j)
            : new Vector3(-offset + 0.5f + i, 0, offset - 0.5f - j);

        // Instantiate the cell in game coordinates
        GameObject cell = Instantiate(CellPrefab, position, Quaternion.identity);
        Cell cellScript = cell.GetComponent<Cell>();

        // Initialize the cell
        cellScript.Initialize(i, j, TerrainType.Base, _cellColors[i][j]);
        cell.transform.SetParent(this.transform); //? This might not be necessary (putting it in the parent), but only needed for organization purposes
        cell.name = "Cell_" + i + "_" + j;

        // Add the cell to the row
        row.Add(cellScript);
      }
      // Add the row to the board
      CurrentBoard.Add(row);
    }

  }

  public void MovePieceToCell(GameObject piece, GameObject newCell)
  {

    Vector3 piecePosition = CalculatePiecePosition(piece, newCell);
    piece.transform.position = piecePosition;
  }

  private Vector3 CalculatePiecePosition(GameObject piece, GameObject cell)
  {
    Vector3 cellPosition = cell.transform.position;
    float cellHeight = cell.transform.localScale.y;
    float pieceHeight = piece.transform.localScale.y;
    return new Vector3(cellPosition.x, cellPosition.y + cellHeight / 2 + pieceHeight / 2, cellPosition.z);
  }

  public void UpdateShadowedCellsCount(bool isShadowed)
  {
    if (isShadowed)
    {
      ShadowedCellsCount++;
    }
    else
    {
      ShadowedCellsCount--;
    }
  }

  public bool IsWithinBoard(int x, int y)
  {
    return x >= 0 && x < Size && y >= 0 && y < Size;
  }

  public static List<Vector2Int> RelativeToAbsolutePosition(List<Vector2Int> relativePositions, Vector2Int reference)
  {
    return relativePositions.Select(relativePosition => relativePosition + reference).ToList();
  }
}
