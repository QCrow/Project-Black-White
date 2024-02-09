public class SkillFactory
{
    public static Skill CreateSkill(SkillType type)
    {
        switch (type)
        {
            case SkillType.EnforcerBaseAttack:
                return new EnforcerBaseAttack();
            case SkillType.EnforcerUnyieldingAssault:
                return new EnforcerUnyieldingAssault();
            default:
                return null;
        }
    }
}

