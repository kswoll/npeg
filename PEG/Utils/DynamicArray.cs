using System;
using System.Collections;
using System.Collections.Generic;

namespace PEG.Utils
{
    public class DynamicArray<T> : IEnumerable<T>
    {
        private List<T> storage = new List<T>();
        private T defaultValue;

        public DynamicArray()
        {
        }

        public DynamicArray(T defaultValue)
        {
            this.defaultValue = defaultValue;
        }

        public void SetMaxLength(int length)
        {
            if (storage.Count > length)
                storage.RemoveRange(length, storage.Count - length);
        }

        public virtual T this[int index]
        {
            get
            {
                if (index >= storage.Count)
                {
                    return defaultValue;
                }
                else
                {
                    return storage[index];
                }
            }
            set
            {
                while (index >= storage.Count)
                    storage.Add(defaultValue);
                storage[index] = value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return storage.GetEnumerator();
        }

        public DynamicArray<T> Copy()
        {
            DynamicArray<T> copy = new DynamicArray<T>(defaultValue);
            copy.storage.AddRange(this);
            return copy;
        }
    }
}