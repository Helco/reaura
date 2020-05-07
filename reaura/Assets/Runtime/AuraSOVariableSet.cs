using System;
using System.Collections.Generic;
using System.Linq;
using Aura.Script;
using UnityEngine;

namespace Aura
{
    public class AuraSOVariableSet : ScriptableObject, IVariableSet
    {
        // More hacks because of unity...
        [Serializable]
        public struct Variable
        {
            public string Name;
            public int Value;

            public Variable(string k, int v)
            {
                Name = k;
                Value = v;
            }
        }

        [SerializeField]
        private List<Variable> values = new List<Variable>();
        private Dictionary<string, int> _valueIndices;
        private Dictionary<string, int> ValueIndices => _valueIndices ??
            (_valueIndices = values
                .Select((p, i) => (name: p.Name, index: i))
                .ToDictionary(p => p.name, p => p.index));

        public IEnumerable<Variable> AsEnumerable => values;

        public int this[string name]
        {
            get
            {
                if (!ValueIndices.TryGetValue(name, out int index))
                    throw new ArgumentOutOfRangeException($"Unknown variable {name} in set {this.name}");
                return values[index].Value;
            }
            set
            {
                if (ValueIndices.TryGetValue(name, out int index))
                    values[index] = new Variable(name, value);
                else
                {
                    ValueIndices[name] = values.Count;
                    values.Add(new Variable(name, value));
                }
            }
        }
    }
}
