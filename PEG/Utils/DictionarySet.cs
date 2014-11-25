using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PEG.Utils
{
    public class DictionarySet<TKey, TValue> : IEnumerable<TKey>
    {
        private Dictionary<TKey, HashSet<TValue>> dictionary = new Dictionary<TKey, HashSet<TValue>>();

        public int GetCount(TKey key)
        {
            HashSet<TValue> list = this[key];
            return list != null ? list.Count : 0;
        }

        public HashSet<TValue> this[TKey key]
        {
            get
            {
                HashSet<TValue> list;
                if (!dictionary.TryGetValue(key, out list))
                {
                    list = new HashSet<TValue>();
                    dictionary[key] = list;
                }
                return list;
            }
        }

        #region IEnumerable<TKey> Members

        public IEnumerator<TKey> GetEnumerator()
        {
            return dictionary.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        public void Clear()
        {
            dictionary.Clear();
        }

        public void AddIfUnique(TKey key, TValue value)
        {
            HashSet<TValue> list;
            if (!dictionary.TryGetValue(key, out list))
            {
                list = new HashSet<TValue>();
                dictionary[key] = list;
            }
            if (!list.Contains(value))
                list.Add(value);
        }

        public void Add(TKey key, TValue value)
        {
            HashSet<TValue> list;
            if (!dictionary.TryGetValue(key, out list))
            {
                list = new HashSet<TValue>();
                dictionary[key] = list;
            }
            list.Add(value);
        }

        public IEnumerable<TValue> GetValues(TKey key)
        {
            HashSet<TValue> list;
            if (dictionary.TryGetValue(key, out list))
            {
                return list;
            }
            else
            {
                return Enumerable.Empty<TValue>();
            }
        }

        public TValue[] ToArray()
        {
            var result = new List<TValue>();
            foreach (var list in dictionary.Values)
                foreach (TValue value in list)
                    result.Add(value);
            return result.ToArray();
        }

        public bool ContainsKey(TKey key)
        {
            return dictionary.ContainsKey(key);
        }

        public int GetValuesCount(TKey key)
        {
            return dictionary.ContainsKey(key) ? dictionary[key].Count : 0;
        }

        public IEnumerable<TValue> Values
        {
            get
            {
                foreach (var value in dictionary.Values)
                {
                    foreach (var current in value)
                    {
                        yield return current;
                    }
                }
            }
        }
    }
}