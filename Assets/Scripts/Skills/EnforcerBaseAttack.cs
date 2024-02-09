using System.Collections.Generic;
using UnityEngine;

public class EnforcerBaseAttack : Skill
{
    public EnforcerBaseAttack() : base("Enforcer Base Attack", "Description", new List<Vector2Int>(), new List<Vector2Int>(), 0, new List<Effect>(), 0, 0, 0, new List<TargetType>())
    {
    }

    public override List<Cell> SelectTarget(Piece caster, List<Target> currentlySelectedTargets)
    {
        throw new System.NotImplementedException();
    }
}