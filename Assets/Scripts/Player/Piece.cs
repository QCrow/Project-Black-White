using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

public abstract class Piece : MonoBehaviour
{
  public Character data;

  //? These stats are copied from the character data and could be modified during the combat
  //? If we don't need them, then we can remove them after
  [SerializeField] private int _maxHitPoints;
  [SerializeField] private int _maxActionPoints;
  [SerializeField] private int _unveiledActionPointRestoration;
  public int MoveRange;

  [SerializeField] private int _turnsToRedeploy;




  public int CurrentHitPoints;
  public int CurrentActionPoints;

  public int RedeployTimer;
  public bool IsShadowed;
  private Cell _cellUnderPiece;
  public Cell CellUnderPiece
  {
    get => _cellUnderPiece;
    set
    {
      if (_cellUnderPiece != value)
      {
        Cell oldCell = _cellUnderPiece;
        _cellUnderPiece = value;
        OnPieceMoved(oldCell, value);
      }
    }
  }

  protected virtual void OnPieceMoved(Cell oldCell, Cell newCell)
  {
    if (oldCell != null)
    {
      oldCell.PieceOnCell = null;
    }

    newCell.PieceOnCell = this;
    BoardManager.Instance.MovePieceToCell(this.gameObject, newCell.gameObject);
  }

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

  public Dictionary<EffectType, int> ActiveEffects = new Dictionary<EffectType, int>();

  private SkillSO _baseAttack;
  private SkillSO _activeSkill;








  public virtual void Initialize()
  {
    // Assign the character's data to the piece in case they need to be modified during the game
    _maxHitPoints = data.MaxHitPoints;
    _maxActionPoints = data.MaxActionPoints;
    _unveiledActionPointRestoration = data.unveiledActionPointRestoration;
    MoveRange = data.MoveRange;
    _turnsToRedeploy = data.TurnsToRedeploy;

    CurrentHitPoints = _maxHitPoints;
    CurrentActionPoints = _maxActionPoints;

    _baseAttack = data.BaseAttack;
    _activeSkill = data.ActiveSkill;

    RedeployTimer = 0;
  }

  #region Update Piece Stats

  public void VeiledRefreshActions()
  {
    CurrentActionPoints = _maxActionPoints;
  }

  public void UnveiledRefreshActions()
  {
    CurrentActionPoints = Mathf.Clamp(CurrentActionPoints + _unveiledActionPointRestoration, 0, _maxActionPoints);
  }

  public void ExhaustActions()
  {
    CurrentActionPoints = 0;
  }
  #endregion


}