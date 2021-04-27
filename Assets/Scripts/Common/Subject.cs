using System;

public class Subject<T> : IObservable<T>
{
    protected T _value;
    protected Action<T> _onChange;

    public Subject(T initialValue)
    {
        _value = initialValue;
    }

    public void Subscribe(Action<T> onChange)
    {
        _onChange += onChange;
        onChange?.Invoke(_value);
    }

    public void UnSubscribe(Action<T> onChange)
    {
        _onChange -= onChange;
    }

    public void Set(T value)
    {
        _value = value;
        _onChange?.Invoke(_value);
    }

    public T Get()
    {
        return _value;
    }
}
