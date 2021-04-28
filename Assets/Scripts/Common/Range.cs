using System;

public class Range<T> : IRange<T> where T : IComparable<T>
{
    public IObservable<T> Value => _value;
    public IObservable<(T, T)> Bounds => _bounds;

    public T Minimum => _bounds.Get().Item1;
    public T Current => _value.Get();
    public T Maximum => _bounds.Get().Item2;

    protected Subject<T> _value = new Subject<T>(default);
    protected Subject<(T, T)> _bounds = new Subject<(T, T)>((default, default));

    public Range(T initialValue, T minimum, T maximum)
    {
        _value.Set(initialValue);
        _bounds.Set((minimum, maximum));
    }

    public void Set(T value)
    {
        if (0 > value.CompareTo(Minimum))
        {
            value = Minimum;
        }

        if (0 < value.CompareTo(Maximum))
        {
            value = Maximum;
        }

        _value.Set(value);
    }

    public void SetBounds(T minimum, T maximum)
    {
        _bounds.Set((minimum, maximum));
    }

    public void RemoveAllListeners()
    {
        _bounds.RemoveAllListeners();
        _bounds = null;
        _value.RemoveAllListeners();
        _value = null;
    }
}
