using HarmonyLib;
using Marine.Based.Configs;
using Marine.Based.Handlers;
using PluginAPI.Core.Attributes;
using PluginAPI.Events;

namespace Marine.Based
{
    public sealed class Plugin
    {
        private const string HarmonyId = "Ray-Grey.Marine.Based";

        private EventHandlers _handlers;
        private Harmony _harmony;

        [PluginEntryPoint("Marine.Based", "1.0.0", "Anti-Abuse", "swear.to.god")]
        public void OnEnabled()
        {
            _harmony = new(HarmonyId);
            _handlers = new();

            EventManager.RegisterEvents(this, _handlers);

            _harmony.PatchAll(GetType().Assembly);
        }

        public void OnDisabled()
        {
            _harmony.UnpatchAll(HarmonyId);

            EventManager.UnregisterEvents(this, _handlers);

            _handlers = null;
            _harmony = null;
        }
    }
}
