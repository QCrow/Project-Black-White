using System.Collections.Generic;
using UnityEngine;

public abstract class Ally : Piece
{
  public Character data;

  public int CurrentActionPoints { get; set; }

  public int RedeployTimer { get; set; }

  public bool CanMove
  {
    get => CurrentActionPoints - MovementCost >= 0 && !ActiveEffects.ContainsKey(EffectType.Stun) && !ActiveEffects.ContainsKey(EffectType.Root);
  }

  public bool CanAttack
  {
    get => CurrentActionPoints - _baseAttack.ActionPointCost >= 0 && !ActiveEffects.ContainsKey(EffectType.Stun);
  }

  public bool CanUseSkill
  {
    get => CurrentActionPoints - _activeSkill.ActionPointCost >= 0 && !ActiveEffects.ContainsKey(EffectType.Stun);
  }

  public int MovementCost
  {
    get => 1; //TODO: To be changed for logic processing
  }

  private SkillSO _baseAttack;
  private SkillSO _activeSkill;

  public override void Initialize()
  {
    _maxHitPoints = data.MaxHitPoints;
    MoveRange = data.MoveRange;
    CurrentActionPoints = data.MaxActionPoints;
    RedeployTimer = 0;
    _baseAttack = data.BaseAttack;
    _activeSkill = data.ActiveSkill;

    IsShadow = true;

    base.Initialize();
  }
  public void VeiledRefreshActions()
  {
    CurrentActionPoints = data.MaxActionPoints;
  }

  public void UnveiledRefreshActions()
  {
    CurrentActionPoints = Mathf.Clamp(CurrentActionPoints + data.UnveiledActionPointRestoration, 0, data.MaxActionPoints);
  }

  public void ExhaustActions()
  {
    CurrentActionPoints = 0;
  }
}