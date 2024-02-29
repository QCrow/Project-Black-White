
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnforcerUnyieldingAssault : Skill
{
    private static string _name = "Unyielding Assault";
    private static string _description = "A charge attack that deals 2 damage to the target and 1 damage to the caster. Knocks back the target 1 cell.";
    private static int _cost = 2;
    private static int _cooldown = 3;
    private static List<TargetType> _validTargetType = new List<TargetType>(
        new TargetType[] // The valid target types are only non-empty cells
        {
            TargetType.Any
        }
    );

    private static List<Vector2Int> _projectileDirection = new List<Vector2Int>
    {
        new Vector2Int(1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(-1, 0),
        new Vector2Int(0, -1)
    };
    private static int _projectileRange = 100; // A very large number to make the move stop only when it hits a wall or a piece

    public EnforcerUnyieldingAssault() : base(_name, _description, _cost, _cooldown, _validTargetType)
    {
    }

    public override List<Cell> SelectTarget(Piece caster, List<Target> currentlySelectedTargets)
    {
        if (currentlySelectedTargets.Count == 0)
        {
            return TargetingHelper.GetProjectileRange(caster.CellUnderPiece, _projectileDirection, _projectileRange);
        }
        ResolveSkill(caster, currentlySelectedTargets[0]);
        return new();
    }

    private void ResolveSkill(Piece caster, Target target)
    {
        //? The following code might be refactored into a helper method in the future
        // Compute the direction of the charge
        Vector2Int direction = target.x > caster.x ? new Vector2Int(1, 0) : target.x < caster.x ? new Vector2Int(-1, 0) : target.y > caster.y ? new Vector2Int(0, 1) : new Vector2Int(0, -1);

        List<Cell> cellsOnPath = TargetingHelper.GetProjectileRange(caster.CellUnderPiece, new List<Vector2Int>(new Vector2Int[] { direction }), _projectileRange);

        // Change cells color
        foreach (Cell cell in cellsOnPath)
        {
            cell.IsShadow = true;
        }
        caster.CellUnderPiece.IsShadow = true;

        // Displace the caster
        Cell stoppingCell = cellsOnPath.Last();
        if (stoppingCell.PieceOnCell == null)
        {
            caster.CellUnderPiece = stoppingCell;
        }
        else
        {
            caster.CellUnderPiece = BoardManager.Instance.GetCell(stoppingCell.x - direction.x, stoppingCell.y - direction.y);
            caster.TakeDamage(1);
            stoppingCell.PieceOnCell.TakeDamage(2);
        }

        //? Might include a knockback?

        ((Ally)caster).CurrentActionPoints -= Cost;
    }
}