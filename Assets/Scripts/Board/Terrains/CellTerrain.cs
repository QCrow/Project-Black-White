using UnityEngine;

public abstract class CellTerrain : MonoBehaviour
{
  public virtual bool IsPassable { get; set; } = false;

}