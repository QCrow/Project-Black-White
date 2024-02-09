using System.Collections.Generic;
using UnityEngine;

public abstract class Skill
{

    public readonly string Name;
    public readonly string Description;

    public readonly List<Vector2Int> Range;

    public readonly List<Vector2Int> Area;

    public readonly int Cooldown;

    public readonly List<Effect> Effects;

    public readonly int Cost;

    public readonly int minimumPossibleTargets;
    public readonly int maximumPossibleTargets;
    protected readonly List<TargetType> TargetTypes;

    public Skill(string name, string description, List<Vector2Int> range, List<Vector2Int> area, int cooldown, List<Effect> effects, int cost, int minimumPossibleTargets, int maximumPossibleTargets, List<TargetType> targetTypes)
    {
        Name = name;
        Description = description;
        Range = range;
        Area = area;
        Cooldown = cooldown;
        Effects = effects;
        Cost = cost;
        this.minimumPossibleTargets = minimumPossibleTargets;
        this.maximumPossibleTargets = maximumPossibleTargets;
        TargetTypes = targetTypes;
    }

    /// <summary>
    /// Process skill targeting logic based on currently selected targets and update the list of cells in range.
    /// </summary>
    /// <param name="caster">The piece that is casting the skill.</param>
    /// <param name="currentlySelectedTargets">The currently selected targets by the user.</param>
    /// <returns>The list of cells that are in the range for next selection. Returns empty if the skill is resolved or if the target leads to deselection of the skill.</returns>
    public abstract List<Cell> SelectTarget(Piece caster, List<Target> currentlySelectedTargets);
}