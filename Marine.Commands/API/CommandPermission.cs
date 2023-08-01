using Exiled.API.Features;
using System;
using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace Marine.Commands.API
{
    public class CommandPermission
    {
        public bool IsLimited { get; set; } = false;

        public List<string> Groups { get; set; } = new List<string>();

        public List<string> Users { get; set; } = new List<string>();

        [YamlIgnore]
        public Func<Player, bool> Custom { get; set; }
    }
}
