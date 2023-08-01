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
        public float Health { get; } = 100;

        [YamlMember(Alias = "size")]
        public Vector3 Size { get; } = Vector3.one;

        [YamlMember(Alias = "shield")]
        public virtual (float Amount, float Limit, float Decay, float Efficacy, float sus, bool Persistent) Shield { get; } = (0, 75, 1.2f, 0.7f, 0, false);

        [YamlMember(Alias = "inventory")]
        public SubclassInventory Inventory { get; } = new SubclassInventory();

        [YamlMember(Alias = "show_info")]
        public bool ShowInfo { get; set; } = false;

        [YamlMember(Alias = "message")]
        public Message Message { get; set; } = new Message();
    }
}
