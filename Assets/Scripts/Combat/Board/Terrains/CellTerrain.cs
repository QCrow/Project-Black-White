using UnityEngine;

public abstract class CellTerrain : MonoBehaviour
{
    public virtual bool IsPassable { get; set; } = false;
    public virtual bool BlocksProjectile { get; set; } = false;

}