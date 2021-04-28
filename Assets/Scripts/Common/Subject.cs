using System;
using System.Collections.Generic;

public class Subject<T> : IObservable<T>
{
    protected List<Action<T>> _listeners;
    protected T _value;
    protected Action<T> _onChange;

    public Subject(T initialValue)
    {
        _listeners = new List<Action<T>>();
        _value = initialValue;
    }

    public void Subscribe(Action<T> onChange)
    {
        _listeners.Add(onChange);
        _onChange += onChange;
        onChange?.Invoke(_value);
    }

    public void UnSubscribe(Action<T> onChange)
    {
        _listeners.Remove(onChange);
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

    public void RemoveAllListeners()
    {
        _listeners.ForEach(listener => _onChange -= listener);
        _listeners.Clear();
    }
}
