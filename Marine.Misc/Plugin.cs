using Exiled.Events.Handlers;
using HarmonyLib;
using Marine.Misc.Handlers;
using System;

namespace Marine.Misc
{
    public sealed class Plugin : Exiled.API.Features.Plugin<Config>
    {
        private const string HarmonyId = "Ray-Grey.Marine.Misc";

        private WarheadHandlers _warhead;
        private ServerHandlers _server;
        private PlayerHandlers _player;
        private MapHandlers _map;
        private Harmony _harmony;

        public override string Name => "Marine.Misc";

        public override string Prefix => "Marine.Misc";

        public override string Author => "i.your";

        public override Version Version { get; } = new(1, 0, 0);

        public override void OnEnabled()
        {
            _warhead = new();
            _server  = new();
            _player  = new(Config);
            _map     = new();

            _harmony = new(HarmonyId);

            Warhead.Detonated += _warhead.OnDetonated;

            Server.RoundStarted += _server.OnRoundStarted;
            Server.RestartingRound += _server.OnRestartingRound;

            Player.ActivatingGenerator += _player.OnActivatingGenerator;
            Player.UnlockingGenerator += _player.OnUnlockingGenerator;
            Player.InteractingLocker += _player.OnInteractingLocker;
            Player.TriggeringTesla += _player.OnTriggeringTesla;
            Player.ReloadingWeapon += _player.OnReloadingWeapon;
            Player.InteractingDoor += _player.OnInteractingDoor;
            Player.ChangingRole += _player.OnChangingRole;
            Player.UsedItem += _player.OnUsedItem;
            Player.Hurting += _player.OnHurting;
            Player.Dying += _player.OnDying;
            Player.Shot += _player.OnShot;


            Map.Generated += _map.OnGenerated;

            _harmony.PatchAll(GetType().Assembly);

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            _harmony.UnpatchAll(HarmonyId);

            Map.Generated -= _map.OnGenerated;

            Player.Shot -= _player.OnShot;
            Player.Dying -= _player.OnDying;
            Player.Hurting -= _player.OnHurting;
            Player.UsedItem -= _player.OnUsedItem;
            Player.ChangingRole -= _player.OnChangingRole;
            Player.TriggeringTesla -= _player.OnTriggeringTesla;
            Player.ReloadingWeapon -= _player.OnReloadingWeapon;
            Player.InteractingDoor -= _player.OnInteractingDoor;
            Player.InteractingLocker -= _player.OnInteractingLocker;
            Player.UnlockingGenerator -= _player.OnUnlockingGenerator;
            Player.ActivatingGenerator -= _player.OnActivatingGenerator;

            Server.RestartingRound -= _server.OnRestartingRound;
            Server.RoundStarted -= _server.OnRoundStarted;

            Warhead.Detonated -= _warhead.OnDetonated;

            _harmony = null;

            _map     = null;
            _player  = null;
            _server  = null;
            _warhead = null;

            base.OnDisabled();
        }

        public override void OnReloaded() { }

        public override void OnRegisteringCommands() { }

        public override void OnUnregisteringCommands() { }
    }
}
