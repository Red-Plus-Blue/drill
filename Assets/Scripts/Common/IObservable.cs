using System;

public interface IObservable<T>
{
    void Subscribe(Action<T> onChange);

    void UnSubscribe(Action<T> onChange);

    void RemoveAllListeners();

}
