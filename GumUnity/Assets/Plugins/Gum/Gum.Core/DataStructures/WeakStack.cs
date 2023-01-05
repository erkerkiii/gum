using System;
using Gum.Core.Assert;

namespace Gum.Core.DataStructures
{
    public class WeakStack<T> where T : class
    {
        private WeakReference[] _collection;

        private int _index;

        public int Count => _index;

        private readonly Func<T> _instanceProviderCallback;

        public WeakStack()
        {
            _collection = new WeakReference[4];
        }
        
        public WeakStack(Func<T> instanceProviderCallback)
        {
            _instanceProviderCallback = instanceProviderCallback;
            _collection = new WeakReference[4];
        }
        
        public void Push(T value)
        {
            EnsureCapacity();
            
            _collection[_index++] = new WeakReference(value);
        }

        private void EnsureCapacity()
        {
            if (_collection.Length <= _index)
            {
                Array.Resize(ref _collection, _collection.Length * 2);
            }
        }

        public T Pop()
        {
            GumAssert.GreaterThanZero(_index);

            T poppedInstance = (T)_collection[--_index].Target ?? (_instanceProviderCallback == null
                ? Activator.CreateInstance<T>()
                : _instanceProviderCallback.Invoke());

            return poppedInstance;
        }
    }
}