using System.Collections.Generic;
using UnityEngine;
public class Character
{
  public string Name;

  public PieceType PieceType;
  public int MaxHitPoints;
  public int MaxActionPoints;

  public int unveiledActionPointRestoration;
  public int MoveRange;

  public int TurnsToRedeploy;

  public List<SkillSO> AllSkills = new List<SkillSO>();


  public List<SkillSO> UnlockedSkills = new List<SkillSO>();
  public List<SkillSO> ActiveSkills = new List<SkillSO>();


  public Character(CharacterBaseDataSO characterBaseData)
  {
    Name = characterBaseData.Name;
    PieceType = characterBaseData.PieceType;
    MaxHitPoints = characterBaseData.MaxHitPoints;
    MaxActionPoints = characterBaseData.MaxActionPoints;
    unveiledActionPointRestoration = characterBaseData.unveiledActionPointRestoration;
    MoveRange = characterBaseData.MoveRange;
    TurnsToRedeploy = characterBaseData.turnsToRedeploy;
  }
}