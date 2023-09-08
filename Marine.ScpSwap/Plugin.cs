using Exiled.Events.Handlers;
using Marine.ScpSwap.API;
using Marine.ScpSwap.Configs;
using Marine.ScpSwap.Handlers;
using System;

namespace Marine.ScpSwap
{
    public sealed class Plugin : Exiled.API.Features.Plugin<Config>
    {
        private PlayerHandlers _handlers;

        public override string Name => "Marine.ScpSwap";

        public override string Prefix => "marine.scp_swap";

        public override string Author => "NotAloneAgain";

        public override Version Version { get; } = new(1, 0, 0);

        public override void OnEnabled()
        {
            _handlers = new(Config.InfoText, Config.InfoDuration);

            Swap.Slots = Config.Slots;
            Swap.Prevent = Config.PreventMultipleSwaps;
            Swap.AllowedScps = Config.AllowedScps;
            Swap.SwapDuration = Config.SwapDuration;

            Player.ChangingRole += _handlers.OnChangingRole;

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Player.ChangingRole -= _handlers.OnChangingRole;

            _handlers = null;

            base.OnDisabled();
        }

        public override void OnReloaded() { }

        public override void OnRegisteringCommands() { }

        public override void OnUnregisteringCommands() { }
    }
}
