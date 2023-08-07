using Marine.Commands.API.Abstract;
using Marine.Commands.Commands;
using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace Marine.Commands.Models
{
    public class Commands
    {
        [YamlIgnore]
        public List<CommandBase> All
        {
            get
            {
                var list = new List<CommandBase>()
                {
                    Force,
                    Steal,
                    Level,
                    Item,
                    Teleport,
                    Heal,
                    Upgrade,
                    Hide,
                    Size,
                    DropItem,
                    DropRagdoll,
                    Grenade
                };

                return list;
            }
        }

        public Force Force { get; set; } = new();

        public Steal Steal { get; set; } = new();

        public Level Level { get; set; } = new();

        public Item Item { get; set; } = new();

        public Teleport Teleport { get; set; } = new();

        public Heal Heal { get; set; } = new();

        public Upgrade Upgrade { get; set; } = new();

        public Size Size { get; set; } = new();

        public Hide Hide { get; set; } = new();

        public DropItem DropItem { get; set; } = new();

        public DropRagdoll DropRagdoll { get; set; } = new();

        public Grenade Grenade { get; set; } = new();
    }
}
