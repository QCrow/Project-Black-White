using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "Scriptable Object/Character Data", order = 0)]
public class CharacterDataSO : ScriptableObject
{
  public string Name;
  public PieceType PieceType;

  public int HitPoints;
  public int ActionPointsPerTurn;
  public int MoveRange;

  public List<Skill> AllSkills = new List<Skill>();
  public List<Skill> AvailableSkills = new List<Skill>();
  public List<Skill> SelectedSkills = new List<Skill>();
}
