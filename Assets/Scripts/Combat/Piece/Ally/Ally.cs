using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ally : Piece
{
    public Character data;

    [SerializeField] private int _currentActionPoints;
    public int CurrentActionPoints { get => _currentActionPoints; set => _currentActionPoints = value; }

    public int RedeployTimer { get; set; }

    public bool CanMove
    {
        get => CurrentActionPoints - MovementCost >= 0;
    }

    public bool CanAttack
    {
        get => CurrentActionPoints - BaseAttack.Cost >= 0;
    }

    public bool CanUseSkill
    {
        get => CurrentActionPoints - ActiveSkill.Cost >= 0;
    }

    public int MovementCost
    {
        get => 1; //TODO: To be changed for logic processing
    }

    public Skill BaseAttack { get; set; }
    public Skill ActiveSkill { get; set; }

    public override void Initialize()
    {
        _maxHitPoints = data.MaxHitPoints;
        _moveRange = data.MoveRange;
        CurrentActionPoints = data.MaxActionPoints;
        RedeployTimer = 0;
        BaseAttack = data.BaseAttack;
        ActiveSkill = data.ActiveSkill;

        IsShadow = true;

        base.Initialize();
    }

    public void VeiledRefreshActions()
    {
        CurrentActionPoints = data.MaxActionPoints;
    }

    public void UnveiledRefreshActions()
    {
        CurrentActionPoints = Mathf.Clamp(CurrentActionPoints + data.MaxActionPoints / 2, 0, data.MaxActionPoints);
    }

    public void ExhaustActions()
    {
        CurrentActionPoints = 0;
    }

    /// <summary>
    /// Retrieves a list of possible moves for the ally based on its current state.
    /// </summary>
    /// <remarks>
    /// This method overrides the base class implementation to introduce a condition that checks the ally's ability to move.
    /// If the ally cannot move, an empty list is returned.
    /// </remarks>
    /// <returns>A list of <see cref="Cell"/> objects representing possible moves. If the ally cannot move, the list is empty.</returns>
    public override List<Cell> GetPossibleMoves()
    {
        if (CanMove)
        {
            return base.GetPossibleMoves();
        }
        // Return an empty list if the ally cannot move
        return new();
    }
}