namespace ChoreHelper.Editor.Model;

public sealed class Transition
{
    public Transition(DanceFigure source, DanceFigure target, byte distance)
    {
        Source = source;
        Target = target;
        Distance = distance;
    }

    public DanceFigure Source { get; }
    public DanceFigure Target { get; }
    public byte Distance { get; }
}