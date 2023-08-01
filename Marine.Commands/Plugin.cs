using Marine.Commands.API.Abstract;
using Marine.Commands.Configs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Marine.Commands
{
    public sealed class Plugin : Exiled.API.Features.Plugin<Config>
    {
        public override string Name => "Marine.Commands";

        public override string Prefix => "marine.commands";

        public override string Author => "i.your";

        public override Version Version { get; } = new(1, 0, 0);

        public override void OnRegisteringCommands()
        {
            foreach (var command in Config.Commands.All)
            {
                command.Subscribe();
            }
        }

        public override void OnUnregisteringCommands()
        {
            foreach (var command in Config.Commands.All)
            {
                command.Unsubscribe();
            }
        }

        public override void OnReloaded() { }
    }
}
