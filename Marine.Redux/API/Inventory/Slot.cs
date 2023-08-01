using Marine.Redux.API.Interfaces;
using UnityEngine;
using YamlDotNet.Serialization;

namespace Marine.Redux.API.Inventory
{
    public class Slot : IHasRandom
    {
        [YamlMember(Alias = "item")]
        private ItemType _item;

        [YamlMember(Alias = "possible_items")]
        private ItemChances _possibleItems;

        public Slot()
        {
            _item = ItemType.None;
        }

        public Slot(ItemChances items, bool isRandomable) : this()
        {
            _possibleItems = items;
            IsRandomable = isRandomable;
        }

        [YamlIgnore]
        public ItemType Item => _item;

        public bool IsRandomable { get; set; }

        public void Randomize()
        {
            if (!IsRandomable)
            {
                return;
            }

            _item = ItemType.None;

            foreach (var chances in _possibleItems)
            {
                if (Random.Range(0, 101) <= chances.Value)
                {
                    _item = chances.Key;

                    return;
                }
            }
        }
    }
}
