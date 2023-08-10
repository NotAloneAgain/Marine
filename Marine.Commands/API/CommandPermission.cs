using System.Collections.Generic;

namespace Marine.Commands.API
{
    public class CommandPermission
    {
        public bool IsLimited { get; set; } = false;

        public HashSet<string> Groups { get; set; } = new HashSet<string>();

        public HashSet<string> Users { get; set; } = new HashSet<string>();
    }
}
