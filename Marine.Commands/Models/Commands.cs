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
                    Teleport
                };

                return list;
            }
        }

        public Force Force { get; set; } = new();

        public Steal Steal { get; set; } = new();

        public Level Level { get; set; } = new();

        public Item Item { get; set; } = new();

        public Teleport Teleport { get; set; } = new();
    }
}
