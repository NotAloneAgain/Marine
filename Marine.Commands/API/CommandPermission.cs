using Exiled.API.Features;
using System;
using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace Marine.Commands.API
{
    public class CommandPermission
    {
        public bool IsLimited { get; set; } = false;

        public HashSet<string> Groups { get; set; } = new HashSet<string>();

        public HashSet<string> Users { get; set; } = new HashSet<string>();
    }
}
