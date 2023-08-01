using Exiled.API.Interfaces;
using Marine.Commands.API.Interfaces;
using Marine.ScpSwap.Commands;
using System.Collections.Generic;
using System.ComponentModel;

namespace Marine.Commands.Configs
{
    public sealed class Config : IConfig
    {
        [Description("Enabled or not.")]
        public bool IsEnabled { get; set; } = true;

        [Description("Debug enabled or not.")]
        public bool Debug { get; set; } = false;

        [Description("Commands settings.")]
        public List<ICommand> Commands { get; set; } = new List<ICommand>()
        {
            new Force(),
            new Steal()
        };
    }
}
