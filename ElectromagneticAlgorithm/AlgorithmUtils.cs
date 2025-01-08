using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectromagneticAlgorithm
{
    internal static class AlgorithmUtils
    {
        public class Map<T1, T2> // Źródło: https://stackoverflow.com/questions/10966331/two-way-bidirectional-dictionary-in-c
        {
            private Dictionary<T1, T2> _forward = new Dictionary<T1, T2>();
            private Dictionary<T2, T1> _reverse = new Dictionary<T2, T1>();

            public Map()
            {
                this.Forward = new Indexer<T1, T2>(_forward);
                this.Reverse = new Indexer<T2, T1>(_reverse);
            }

            public class Indexer<T3, T4>
            {
                private Dictionary<T3, T4> _dictionary;
                public Indexer(Dictionary<T3, T4> dictionary)
                {
                    _dictionary = dictionary;
                }
                public T4 this[T3 index]
                {
                    get { return _dictionary[index]; }
                    set { _dictionary[index] = value; }
                }

                // My addition
                public List<T3> GetKeys()
                {
                    return new List<T3>(_dictionary.Keys);
                }
            }

            public void Add(T1 t1, T2 t2)
            {
                _forward.Add(t1, t2);
                _reverse.Add(t2, t1);
            }

            public Indexer<T1, T2> Forward { get; private set; }
            public Indexer<T2, T1> Reverse { get; private set; }
        }

    }
}
