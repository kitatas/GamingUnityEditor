using System;
using System.Collections.Generic;
using UnityEngine;

namespace GamingUnityEditor
{
    internal static class Extension
    {
        private const float MAX = 255.0f;

        public static Color ToGamingColor(this ref int self)
        {
            self = ++self <= MAX ? self : 0;
            return Color.HSVToRGB(self / MAX, 1.0f, 1.0f);
        }

        public static IDisposable Subscribe<T>(this IObservable<T> self, Action<T> onNext)
        {
            return self.Subscribe(new MiniObserver<T>(onNext));
        }

        public static IObservable<T> Where<T>(this IObservable<T> self, Func<T, bool> predicate)
        {
            return new MiniObservable<T>(observer =>
                self.Subscribe(x =>
                {
                    var filter = predicate?.Invoke(x);
                    if (filter.HasValue && filter.Value)
                    {
                        observer?.OnNext(x);
                    }
                })
            );
        }

        public static void AddTo(this IDisposable self, List<IDisposable> disposables)
        {
            disposables.Add(self);
        }
    }

    #region Observer Pattern

    internal sealed class MiniObserver<T> : IObserver<T>
    {
        private readonly Action<T> _onNext;

        public MiniObserver(Action<T> onNext)
        {
            _onNext = onNext;
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(T value)
        {
            _onNext?.Invoke(value);
        }
    }

    internal sealed class MiniObservable<T> : IObservable<T>, IDisposable
    {
        private readonly Action<IObserver<T>> _observer;

        public MiniObservable(Action<IObserver<T>> observer)
        {
            _observer = observer;
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            _observer?.Invoke(observer);
            return this;
        }

        public void Dispose()
        {
        }
    }

    internal sealed class MiniRx<T> : IObservable<T>, IObserver<T>, IDisposable
    {
        private T _value;
        private readonly List<IObserver<T>> _observers = new();

        public IDisposable Subscribe(IObserver<T> observer)
        {
            _observers.Add(observer);
            return this;
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(T value)
        {
            if (Equals(_value, value)) return;
            _value = value;
            foreach (var observer in _observers)
            {
                observer?.OnNext(_value);
            }
        }

        public void Dispose()
        {
            _observers.Clear();
        }
    }
    
    #endregion
}