using Marine.Commands.API.Abstract;
using Marine.Commands.Configs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Marine.Commands
{
    public sealed class Plugin : Exiled.API.Features.Plugin<Config>
    {
        private List<CommandBase> _commands;

        public override string Name => "Marine.Commands";

        public override string Prefix => "marine.commands";

        public override string Author => "i.your";

        public override Version Version { get; } = new(1, 0, 0);

        public override void OnRegisteringCommands()
        {
            _commands = Config.Commands.All;

            foreach (var command in _commands)
            {
                command.Subscribe();
            }
        }

        public override void OnUnregisteringCommands()
        {
            if (_commands == null || !_commands.Any())
            {
                return;
            }

            foreach (var command in _commands)
            {
                command.Unsubscribe();
            }

            _commands = null;
        }

        public override void OnReloaded() { }
    }
}
