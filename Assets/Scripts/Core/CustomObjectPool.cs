using System;
using System.Collections.Generic;

namespace Asteroids.Core
{
    public class CustomObjectPool<T> where T : class
    {
        private readonly Stack<T> _pool;
        private readonly Func<T> _createFunc; 
        private readonly Action<T> _actionOnGet; 
        private readonly Action<T> _actionOnRelease; 

        public CustomObjectPool(Func<T> createFunc, Action<T> actionOnGet, Action<T> actionOnRelease, int initialCapacity = 10)
        {
            _createFunc = createFunc ?? throw new ArgumentNullException(nameof(createFunc));
            _actionOnGet = actionOnGet;
            _actionOnRelease = actionOnRelease;
            
            _pool = new Stack<T>(initialCapacity);
        }

        public T Get()
        {
            T item = _pool.Count > 0 ? _pool.Pop() : _createFunc();
            _actionOnGet?.Invoke(item);
            
            return item;
        }

        public void Release(T item)
        {
            if (item == null) return;

            _actionOnRelease?.Invoke(item);
            
            _pool.Push(item);
        }
    }
}