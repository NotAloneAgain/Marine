﻿using Marine.Commands.API.Abstract;
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
                    Grenade,
                    Zombie,
                    Sus,
                    Ball,
                    Knock,
                    Subclass,
                    Clothes,
                    Scp372Command,
                    Suicide
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

        public Zombie Zombie { get; set; } = new();

        public Sus Sus { get; set; } = new();

        public Ball Ball { get; set; } = new();

        public Knock Knock { get; set; } = new();

        public GiveSubclass Subclass { get; set; } = new();

        public Suicide Suicide { get; set; } = new();

        public Scp372Command Scp372Command { get; set; } = new();

        public Clothes Clothes { get; set; } = new();
    }
}
