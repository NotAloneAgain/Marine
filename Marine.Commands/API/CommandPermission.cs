using Exiled.API.Features;
using System;
using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace Marine.Commands.API
{
    public class CommandPermission
    {
        public bool IsLimited { get; set; }

        public List<string> Groups { get; set; }

        public List<string> Users { get; set; }

        [YamlIgnore]
        public Func<Player, bool> Custom { get; set; }
    }
}
