using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnforcerBaseAttack : Skill
{
    private static string _name = "Base Attack";
    private static string _description = "A quick hit to an adjacent non-empty cell.";
    private static int _cost = 1;
    private static int _cooldown = 0;
    private static List<TargetType> _validTargetType = new List<TargetType>(
        new TargetType[] // The valid target types are only non-empty cells
        {
            TargetType.NonEmpty
        }
    );

    private static List<Vector2Int> _range = new List<Vector2Int>(
        new Vector2Int[] // The range is the 4 cells around the caster
        {
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, -1)
        }
    );
    private static List<Vector2Int> _area = new List<Vector2Int>(
        new Vector2Int[] // The area is only the target cell (single target)
        {
            new Vector2Int(0, 0)
        }
    );



    public EnforcerBaseAttack() : base(_name, _description, _cost, _cooldown, _validTargetType)
    {
    }

    public override List<Cell> SelectTarget(Piece caster, List<Target> currentlySelectedTargets)
    {
        if (currentlySelectedTargets.Count == 0)
        {
            return TargetingHelper.GetCellsWithRelativeRange(caster.CellUnderPiece, _range);
        }
        ResolveSkill(caster as Ally, currentlySelectedTargets[0]);
        return new();
    }

    private void ResolveSkill(Ally caster, Target target)
    {
        target.TargetPiece.TakeDamage(1);
        target.TargetPiece.CellUnderPiece.IsShadow = true;

        caster.CurrentActionPoints -= Cost;
    }
}