using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public abstract class Piece : MonoBehaviour
{
  public Character data;
  protected int _currentHitPoints;

  protected int _currentActionPoints;
  public int CurrentActionPoints
  {
    get => _currentActionPoints;
    set
    {
      if (_currentActionPoints != value)
      {
        _currentActionPoints = value;
        UpdateAvailableActions();
      }
    }
  }

  private void UpdateAvailableActions()
  {

  }

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

  public bool IsMovable
  {
    get => _currentActionPoints - MovementCost >= 0 && !ActiveEffects.ContainsKey(EffectType.Stun) && !ActiveEffects.ContainsKey(EffectType.Root);
  }

  private int MovementCost
  {
    get => 1; //TODO: To be changed for logic processing
  }

  public Dictionary<EffectType, int> ActiveEffects = new Dictionary<EffectType, int>();
  protected void Start()
  {
    Initialize();
  }

  protected virtual void Initialize()
  {
    _currentHitPoints = data.MaxHitPoints;
    CurrentActionPoints = data.MaxActionPoints;

    RedeployTimer = 0;
  }

  protected virtual void OnPieceMoved(Cell oldCell, Cell newCell)
  {
    if (oldCell != null)
    {
      oldCell.PieceOnCell = null;
    }
    Debug.Log(newCell);
    newCell.PieceOnCell = this;
    BoardManager.Instance.MovePieceToCell(this.gameObject, newCell.gameObject);
  }

  #region Update Piece Stats
  public void VeiledRefreshActions()
  {
    CurrentActionPoints = data.MaxActionPoints;
  }

  public void UnveiledRefreshActions()
  {
    CurrentActionPoints = Mathf.Max(data.MaxActionPoints, CurrentActionPoints + data.unveiledActionPointRestoration);
  }

  public void ExhaustActions()
  {
    CurrentActionPoints = 0;
  }

  public void DecreaseAP(int usedActionPoints)
  {
    CurrentActionPoints -= usedActionPoints;
  }

  public void IncreaseAP(int gainedActionPoints)
  {
    CurrentActionPoints += gainedActionPoints;
  }

  public void DecreaseHP(int damage)
  {
    _currentHitPoints -= damage;
  }

  public void IncreaseHP(int healing)
  {
    _currentHitPoints += healing;
  }
  #endregion
}