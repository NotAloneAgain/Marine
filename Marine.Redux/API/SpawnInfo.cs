using Marine.Redux.API.Inventory;
using UnityEngine;
using YamlDotNet.Serialization;

namespace Marine.Redux.API
{
    public class SpawnInfo
    {
        public SpawnInfo()
        {

        }

        [YamlMember(Alias = "health")]
        public virtual float Health { get; set; } = 100;

        [YamlMember(Alias = "size")]
        public virtual Vector3 Size { get; set; } = Vector3.one;

        [YamlMember(Alias = "shield")]
        public virtual Shield Shield { get; set; } = new (0, 75, 1.2f, 0.7f, 0, false);

        [YamlMember(Alias = "inventory")]
        public virtual SubclassInventory Inventory { get; set; } = new SubclassInventory();

        [YamlMember(Alias = "show_info")]
        public virtual bool ShowInfo { get; set; } = false;

        [YamlMember(Alias = "message")]
        public virtual Message Message { get; set; } = new Message();
    }
}
