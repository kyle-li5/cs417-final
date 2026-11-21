using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rowland
{
    [Serializable]
    public struct ItemEntry
    {
        public string name;
        public bool unlocked;
        public bool persistant;
    }

    public class SpellDictionary : MonoBehaviour
    {
        private Dictionary<string, (bool unlocked, bool persistant)> spells;
        [SerializeField] private List<ItemEntry> entries = new List<ItemEntry>();

        private void Awake()
        {
            spells = new Dictionary<string, (bool unlocked, bool persistant)>();
            foreach (ItemEntry entry in entries)
            {
                spells[entry.name] = (entry.unlocked, entry.persistant);
            }
        }

        public bool isUnlocked(string name)
        {
            return spells.TryGetValue(name, out var spell) && spell.unlocked;
        }

        public void setUnlock(string name, bool _unlocked)
        {
            if (spells.TryGetValue(name, out var spell) && !spell.persistant)
            {
                spells[name] = (_unlocked, spell.persistant);
            }
        }

        public void setAll(bool _unlocked)
        {
            foreach (string key in new List<string>(spells.Keys))
            {
                var spell = spells[key];
                if (!spell.persistant)
                    spells[key] = (_unlocked, spell.persistant);
            }
        }
    }
}
