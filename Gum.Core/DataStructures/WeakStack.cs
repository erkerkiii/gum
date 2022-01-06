using System;
using Gum.Core.Assert;

namespace Gum.Core.DataStructures
{
    public class WeakStack<T>
    {
        private WeakReference[] _collection;

        private int _index;

        public int Count => _index;
        
        public WeakStack()
        {
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

            T poppedInstance = (T)_collection[--_index].Target;
            if (poppedInstance == null)
            {
                poppedInstance = Activator.CreateInstance<T>();
            }

            return poppedInstance;
        }
    }
}