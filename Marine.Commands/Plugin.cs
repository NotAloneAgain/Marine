using Marine.Commands.Configs;
using System;

namespace Marine.Commands
{
    public sealed class Plugin : Exiled.API.Features.Plugin<Config>
    {
        public override string Name => "Marine.Commands";

        public override string Prefix => "Marine.Commands";

        public override string Author => "i.your";

        public override Version Version { get; } = new(1, 0, 0);

        public override void OnReloaded() { }
    }
}
