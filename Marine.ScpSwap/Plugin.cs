using HarmonyLib;
using Marine.ScpSwap.Configs;
using Marine.ScpSwap.Handlers;
using System;

namespace Marine.ScpSwap
{
    public sealed class Plugin : Exiled.API.Features.Plugin<Config>
    {
        private const string HarmonyId = "Ray-Grey.Marine.ScpSwap";

        private EventHandlers _handlers;
        private Harmony _harmony;

        public override string Name => "Marine.ScpSwap";

        public override string Prefix => "Marine.ScpSwap";

        public override string Author => "i.your";

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
