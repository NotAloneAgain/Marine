﻿using Marine.Redux.API.Interfaces;
using UnityEngine;
using YamlDotNet.Serialization;

namespace Marine.Redux.API.Inventory
{
    public class Slot : IHasRandom
    {
        [YamlMember(Alias = "item")]
        private ItemType _item;

        public Slot() => _item = ItemType.None;

        public Slot(ItemChances items, bool isRandomable) : this()
        {
            Items = items;
            IsRandomable = isRandomable;
        }

        [YamlIgnore]
        public ItemType Item => _item;

        public bool IsRandomable { get; set; }

        [YamlMember(Alias = "possible_items")]
        public ItemChances Items { get; set; }

        public void Randomize()
        {
            if (!IsRandomable && _item != ItemType.None)
            {
                return;
            }

            _item = ItemType.None;

            foreach (System.Collections.Generic.KeyValuePair<ItemType, int> chances in Items)
            {
                if (Random.Range(0, 101) >= 100 - chances.Value)
                {
                    _item = chances.Key;

                    return;
                }
            }
        }
    }
}
