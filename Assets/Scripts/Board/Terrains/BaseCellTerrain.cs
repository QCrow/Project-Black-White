public class BaseCellTerrain : CellTerrain
{
  public override bool IsPassable { get; set; } = true;
  public override bool BlocksProjectile { get; set; } = false;

}