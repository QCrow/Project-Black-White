using System.Collections.Generic;

public class Character
{
    public string Name;

    public AllyType Type;
    public int MaxHitPoints;
    public int MaxActionPoints;

    public int UnveiledActionPointRestoration;
    public int MoveRange;

    public int TurnsToRedeploy;

    public Skill BaseAttack;

    public List<SkillType> AllSkillTypes = new();


    public List<SkillType> UnlockedSkillTypes = new();

    public Skill ActiveSkill;


    public Character(CharacterBaseDataSO characterBaseData)
    {
        Name = characterBaseData.Name;
        Type = characterBaseData.Type;
        MaxHitPoints = characterBaseData.MaxHitPoints;
        MaxActionPoints = characterBaseData.MaxActionPoints;
        UnveiledActionPointRestoration = characterBaseData.UnveiledActionPointRestoration;
        MoveRange = characterBaseData.MoveRange;
        TurnsToRedeploy = characterBaseData.turnsToRedeploy;
        AllSkillTypes = characterBaseData.AllSkillTypes;
        BaseAttack = SkillFactory.CreateSkill(characterBaseData.BaseAttackType);

        //! This is for testing purposes only for now, eventually this will be handled by UI selection
        UnlockedSkillTypes.Add(characterBaseData.AllSkillTypes[0]);
        ActiveSkill = SkillFactory.CreateSkill(UnlockedSkillTypes[0]);
    }
}