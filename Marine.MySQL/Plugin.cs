﻿using Exiled.API.Enums;
using Marine.MySQL.API;
using System;

namespace Marine.MySQL
{
    public sealed class Plugin : Exiled.API.Features.Plugin<Config>
    {
        public override string Name => "Marine.MySQL";

        public override string Prefix => "Marine.MySQL";

        public override string Author => "i.your";

        public override PluginPriority Priority { get; } = PluginPriority.First;

        public override Version Version { get; } = new(1, 0, 0);

        public override void OnEnabled()
        {
            MySqlManager.Init(Config.ConnectionDiscord, Config.ConnectionScp);

            base.OnEnabled();
        }

        public override void OnReloaded() { }

        public override void OnRegisteringCommands() { }

        public override void OnUnregisteringCommands() { }
    }
}
