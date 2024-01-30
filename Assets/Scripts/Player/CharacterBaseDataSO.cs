using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character Base Data", menuName = "Scriptable Object/Character Base Data", order = 0)]
public class CharacterBaseDataSO : ScriptableObject
{
  public string Name;
  public PieceType PieceType;

  public int MaxHitPoints;
  public int MaxActionPoints;
  public int unveiledActionPointRestoration;

  public int turnsToRedeploy;

  public int MoveRange;

  public List<SkillSO> AllSkills = new List<SkillSO>();
}
