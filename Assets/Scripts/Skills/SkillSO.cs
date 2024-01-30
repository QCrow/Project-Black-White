using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill", menuName = "Scriptable Object/Skill", order = 0)]
public class SkillSO : ScriptableObject
{
  [Header("General")]
  public string SkillName;
  public int ActionPointCost;
  public int Cooldown = 0;

  [Header("Targeting")]
  public bool IsDirectional;
  public List<Vector2Int> TargetRange;
  public List<Vector2Int> TargetArea;
  public int MaximumTargets;

  [Header("Type")]
  public bool IsPassive;
  public bool TargetRequired;
  public bool IsProjectile;
  public bool LeavesTrail;


  [Header("Effect")]
  public int Damage;
  public List<EffectType> Effects;
  public List<int> EffectValues;

  [Header("Upgrade")]
  public List<SkillSO> SkillUpgrades;
}