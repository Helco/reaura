using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Aura.Script;
using System;

namespace Aura
{
    public class AuraInventory : MonoBehaviour, IVariableSet
    {
        [SerializeField]
        private string scriptVariableSetName = "Predmet";
        [SerializeField]
        private AuraSOItemSet itemSet = null;

        private HashSet<string> _allItems = null;
        private HashSet<string> AllItems => _allItems ?? (_allItems = new HashSet<string>(itemSet.Items.Select(i => i.name)));
        private HashSet<string> items = new HashSet<string>();

        public int this[string name]
        {
            get
            {
                if (!AllItems.Contains(name))
                    throw new ArgumentOutOfRangeException($"Unknown item name {name}");
                return items.Contains(name) ? 1 : 0;
            }
            set
            {
                if (!AllItems.Contains(name))
                    throw new ArgumentOutOfRangeException($"Unknown item name {name}");
                if (value == 0)
                    items.Remove(name);
                else
                    items.Add(name);
            }
        }

        private void OnEnable()
        {
            GetComponentInParent<AuraScriptExecution>()
                .Interpreter.RegisterVariableSet(scriptVariableSetName, this);
        }
    }
}
