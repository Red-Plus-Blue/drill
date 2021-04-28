
public interface IRange<T>
{
    IObservable<T> Value { get; }
    IObservable<(T, T)> Bounds { get; }

    T Minimum { get; }
    T Maximum { get; }
    T Current { get; }
}
