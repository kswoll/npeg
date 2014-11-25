using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PEG.Utils
{
    public class DictionaryList<TKey, TValue> : IEnumerable<TKey>
    {
        private Dictionary<TKey, List<TValue>> dictionary = new Dictionary<TKey, List<TValue>>();

        public int GetCount(TKey key)
        {
            List<TValue> list = this[key];
            return list != null ? list.Count : 0;
        }

        public List<TValue> this[TKey key]
        {
            get
            {
                List<TValue> list;
                if (!dictionary.TryGetValue(key, out list) || list == null)
                {
                    list = new List<TValue>();
                    dictionary[key] = list;
                }
                return list;
            }
            set
            {
                dictionary[key] = value;
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

        public void AddIfUnique(TKey key, TValue value)
        {
            List<TValue> list;
            if (!dictionary.TryGetValue(key, out list))
            {
                list = new List<TValue>();
                dictionary[key] = list;
            }
            if (!list.Contains(value))
                list.Add(value);
        }

        public void Add(TKey key, TValue value)
        {
            List<TValue> list;
            if (!dictionary.TryGetValue(key, out list))
            {
                list = new List<TValue>();
                dictionary[key] = list;
            }
            list.Add(value);
        }

        public IEnumerable<TValue> GetValues(TKey key)
        {
            List<TValue> list;
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