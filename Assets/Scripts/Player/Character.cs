using System.Collections.Generic;
using UnityEngine;
public class Character
{
  public string Name;

  public AllyType Type;
  public int MaxHitPoints;
  public int MaxActionPoints;

  public int UnveiledActionPointRestoration;
  public int MoveRange;

  public int TurnsToRedeploy;

  public SkillSO BaseAttack;

  public List<SkillSO> AllSkills = new List<SkillSO>();


  public List<SkillSO> UnlockedSkills = new List<SkillSO>();

  public SkillSO ActiveSkill;


  public Character(CharacterBaseDataSO characterBaseData)
  {
    Name = characterBaseData.Name;
    Type = characterBaseData.Type;
    MaxHitPoints = characterBaseData.MaxHitPoints;
    MaxActionPoints = characterBaseData.MaxActionPoints;
    UnveiledActionPointRestoration = characterBaseData.UnveiledActionPointRestoration;
    MoveRange = characterBaseData.MoveRange;
    TurnsToRedeploy = characterBaseData.turnsToRedeploy;
    AllSkills = characterBaseData.AllSkills;
    BaseAttack = characterBaseData.BaseAttack;

    //! This is for testing purposes only for now, eventually this will be handled by UI selection
    UnlockedSkills.Add(characterBaseData.AllSkills[0]);
    ActiveSkill = UnlockedSkills[0];
  }
}