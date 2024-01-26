using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player_Setup", menuName = "Scriptable Object/Player Setup", order = 0)]
public class PlayerSetup : ScriptableObject
{
  public List<GameObject> Pieces;
}