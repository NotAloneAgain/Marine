using Exiled.API.Interfaces;
using Marine.Commands.API.Abstract;
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

        [Description("CommandsSettings.")]
        public List<CommandBase> Commands { get; set; } = new List<CommandBase>()
        {
            new Force()
        };
    }
}
