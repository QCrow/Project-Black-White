using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Board_Setup", menuName = "Scriptable Object/Board Setup", order = 1)]
public class BoardSetupSO : ScriptableObject
{
  public int BoardWidth;
  public int BoardHeight;

  public List<List<bool>> CellColors = new List<List<bool>>(); // TODO: Add a visualizer in editor interface to customize this
}
