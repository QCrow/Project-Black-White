public abstract class Effect
{
    public EffectType Type;
    public EffectTarget Target;
    public EffectDirection Direction;

    public int Value;
    public int Duration;

    public Effect(EffectType type)
    {
        Type = type;
    }

    public Effect(EffectType type, int value, int duration)
    {
        Type = type;
        Value = value;
        Duration = duration;
    }

    public Effect(EffectType type, EffectTarget target, EffectDirection direction, int value, int duration)
    {
        Type = type;
        Target = target;
        Direction = direction;
        Value = value;
        Duration = duration;
    }


    public abstract void Apply(Piece target);
}

public enum EffectType
{
    Displace,

    Grant
}

public enum EffectTarget
{
    Self,
    Target,
    Both,
}

public enum EffectDirection
{
    Forward,
    Backward
}