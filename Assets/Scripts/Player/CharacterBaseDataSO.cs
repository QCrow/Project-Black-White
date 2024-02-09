using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character Base Data", menuName = "Scriptable Object/Character Base Data", order = 0)]
public class CharacterBaseDataSO : ScriptableObject
{
    public string Name;
    public AllyType Type;

    public int MaxHitPoints;
    public int MaxActionPoints;
    public int UnveiledActionPointRestoration;

    public int turnsToRedeploy;

    public int MoveRange;

    public SkillType BaseAttackType;
    public List<SkillType> AllSkillTypes = new();
}
