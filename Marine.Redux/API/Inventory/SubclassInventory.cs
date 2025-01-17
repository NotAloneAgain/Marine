﻿using Marine.Redux.API.Interfaces;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Serialization;

namespace Marine.Redux.API.Inventory
{
    public class SubclassInventory : IHasRandom
    {
        private readonly List<ItemType> _items;

        public SubclassInventory()
        {
            _items = new(8);

            Slots = new(8);
        }

        public SubclassInventory(IEnumerable<Slot> slots) : this() => Slots.AddRange(slots);

        public SubclassInventory(IEnumerable<Slot> slots, IEnumerable<ItemType> items, bool isRandomable) : this(slots)
        {
            _items.AddRange(items);

            IsRandomable = isRandomable;
        }

        public bool IsRandomable { get; set; }

        [YamlMember(Alias = "slots")]
        public List<Slot> Slots { get; set; }

        [YamlIgnore]
        public IReadOnlyCollection<ItemType> Items => _items.AsReadOnly();

        public void Randomize()
        {
            if (!IsRandomable && _items.Any())
            {
                return;
            }

            _items.Clear();

            foreach (Slot slot in Slots)
            {
                slot.Randomize();

                _items.Add(slot.Item);
            }
        }
    }
}
