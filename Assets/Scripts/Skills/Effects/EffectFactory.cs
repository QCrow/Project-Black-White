public class EffectFactory
{
    public static Effect CreateEffect(EffectType type)
    {
        return new PushEffect();
    }

    public static Effect CreateEffect(EffectType type, int value, int duration)
    {
        Effect effect = CreateEffect(type);
        effect.Value = value;
        effect.Duration = duration;
        return effect;
    }

}