using System.Collections.Generic;
using UnityEngine;

public abstract class Skill
{
    //TODO: Localization might be needed eventually
    public readonly string Name;
    public readonly string Description;
    public readonly int Cost;
    public readonly int Cooldown;
    public readonly List<TargetType> ValidTargetType;

    public Skill(string name, string description, int cost, int cooldown, List<TargetType> validTargetType)
    {
        Name = name;
        Description = description;
        Cost = cost;
        Cooldown = cooldown;
        ValidTargetType = validTargetType;
    }

    /// <summary>
    /// Process skill targeting logic based on currently selected targets and update the list of cells in range.
    /// </summary>
    /// <param name="caster">The piece that is casting the skill.</param>
    /// <param name="currentlySelectedTargets">The currently selected targets by the user.</param>
    /// <returns>The list of cells that are in the range for next selection. Returns empty if the skill is resolved or if the target leads to deselection of the skill.</returns>
    public abstract List<Cell> SelectTarget(Piece caster, List<Target> currentlySelectedTargets);
}