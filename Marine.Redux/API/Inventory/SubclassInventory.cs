using Marine.Redux.API.Interfaces;
using System;
using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace Marine.Redux.API.Inventory
{
    public class SubclassInventory : IHasRandom
    {
        [YamlMember(Alias = "slots")]
        private List<Slot> _slots;

        [YamlMember(Alias = "default_items")]
        private List<ItemType> _items;

        public SubclassInventory()
        {
            _slots = new(8);
        }

        public SubclassInventory(IEnumerable<Slot> slots) : this()
        {
            _slots.AddRange(slots);
        }

        public SubclassInventory(IEnumerable<Slot> slots, IEnumerable<ItemType> items, bool isRandomable) : this(slots)
        {
            _items = new(8);

            _items.AddRange(items);

            IsRandomable = isRandomable;
        }

        public bool IsRandomable { get; set; }

        [YamlIgnore]
        public IReadOnlyCollection<ItemType> Items => _items.AsReadOnly();

        public void Randomize()
        {
            if (!IsRandomable)
            {
                return;
            }

            _items.Clear();

            foreach (var slot in _slots)
            {
                slot.Randomize();

                _items.Add(slot.Item);
            }
        }
    }
}
