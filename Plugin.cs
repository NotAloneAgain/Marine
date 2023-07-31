﻿using HarmonyLib;
using Marine.Configs;
using Marine.Handlers;
using PluginAPI.Core.Attributes;
using PluginAPI.Events;
using System;

namespace Marine
{
    public sealed class Plugin : Exiled.API.Features.Plugin<Config>
    {
        private const string HarmonyId = "Ray-Grey.Marine";

        private EventHandlers _handlers;
        private Harmony _harmony;

        public override string Name => "Marine";

        public override string Prefix => "Marine";

        public override string Author => ".grey#9120";

        public override Version Version { get; } = new(1, 0, 0);

        public override void OnEnabled()
        {
            _harmony = new(HarmonyId);
            _handlers = new();

            _harmony.PatchAll(GetType().Assembly);

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            _harmony.UnpatchAll(HarmonyId);

            _handlers = null;
            _harmony = null;

            base.OnDisabled();
        }

        public override void OnReloaded() { }

        public override void OnRegisteringCommands() { }

        public override void OnUnregisteringCommands() { }
    }
}
