using Marine.Commands.API.Abstract;
using Marine.Commands.Configs;
using Marine.ScpSwap.Commands;
using System;
using System.Collections.Generic;

namespace Marine.Commands
{
    public sealed class Plugin : Exiled.API.Features.Plugin<Config>
    {
        private List<CommandBase> _commands;

        public override string Name => "Marine.Commands";

        public override string Prefix => "Marine.Commands";

        public override string Author => "i.your";

        public override Version Version { get; } = new(1, 0, 0);

        public override void OnRegisteringCommands()
        {
            _commands = new(1)
            {
                new Force()
            };

            foreach (var command in _commands)
            {
                command.Subscribe();
            }
        }

        public override void OnUnregisteringCommands()
        {
            if (_commands == null)
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
