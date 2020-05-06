﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Aura
{
    public class AuraSOItemList : ScriptableObject
    {
        [SerializeField]
        private AuraSOItem[] items;

        public IEnumerable<AuraSOItem> Items
        {
            get => items;
            set => items = value.ToArray();
        }

    }
}
