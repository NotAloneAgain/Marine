using Exiled.Events.Handlers;
using HarmonyLib;
using Marine.Misc.Handlers;
using System;

namespace Marine.Misc
{
    public sealed class Plugin : Exiled.API.Features.Plugin<Config>
    {
        private const string HarmonyId = "NotAloneAgain.Misc";

        private WarheadHandlers _warhead;
        private ServerHandlers _server;
        private PlayerHandlers _player;
        private MapHandlers _map;
        private Harmony _harmony;

        public override string Name => "Marine.Misc";

        public override string Prefix => "marine.misc";

        public override string Author => "NotAloneAgain";

        public override Version Version { get; } = new(1, 0, 0);

        public override void OnEnabled()
        {
            _warhead = new();
            _server = new();
            _player = new(Config);
            _map = new();

            _harmony = new(HarmonyId);

            Warhead.Detonated += _warhead.OnDetonated;
            Warhead.Starting += _warhead.OnStarting;
            Warhead.Stopping += _warhead.OnStopping;

            Server.RoundEnded += _server.OnEndedRound;
            Server.RoundStarted += _server.OnRoundStarted;
            Server.RestartingRound += _server.OnRestartingRound;

            Player.EnteringPocketDimension += _player.OnEnteringPocketDimension;
            Player.UnlockingGenerator += _player.OnUnlockingGenerator;
            Player.InteractingLocker += _player.OnInteractingLocker;
            Player.UsingRadioBattery += _player.OnUsingRadioBattery;
            Player.TriggeringTesla += _player.OnTriggeringTesla;
            Player.ReloadingWeapon += _player.OnReloadingWeapon;
            Player.InteractingDoor += _player.OnInteractingDoor;
            Player.ChangingRole += _player.OnChangingRole;
            Player.Destroying += _player.OnDestroying;
            Player.UsedItem += _player.OnUsedItem;
            Player.Verified += _player.OnVerified;
            Player.Hurting += _player.OnHurting;
            Player.Dying += _player.OnDying;
            Player.Shot += _player.OnShot;

            Exiled.Events.Handlers.Scp914.UpgradingInventoryItem += _map.OnUpgradingInventoryItem;
            Exiled.Events.Handlers.Scp914.UpgradingPickup += _map.OnUpgradingPickup;
            Map.GeneratorActivating += _map.OnGeneratorActivated;
            Map.PlacingBulletHole += _map.OnPlacingBulletHole;
            Map.Generated += _map.OnGenerated;

            _harmony.PatchAll(GetType().Assembly);

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            _harmony.UnpatchAll(HarmonyId);

            Map.Generated -= _map.OnGenerated;
            Map.PlacingBulletHole -= _map.OnPlacingBulletHole;
            Map.GeneratorActivating -= _map.OnGeneratorActivated;
            Exiled.Events.Handlers.Scp914.UpgradingPickup -= _map.OnUpgradingPickup;
            Exiled.Events.Handlers.Scp914.UpgradingInventoryItem -= _map.OnUpgradingInventoryItem;

            Player.Shot -= _player.OnShot;
            Player.Dying -= _player.OnDying;
            Player.Hurting -= _player.OnHurting;
            Player.Verified -= _player.OnVerified;
            Player.UsedItem -= _player.OnUsedItem;
            Player.Destroying -= _player.OnDestroying;
            Player.ChangingRole -= _player.OnChangingRole;
            Player.TriggeringTesla -= _player.OnTriggeringTesla;
            Player.ReloadingWeapon -= _player.OnReloadingWeapon;
            Player.InteractingDoor -= _player.OnInteractingDoor;
            Player.UsingRadioBattery -= _player.OnUsingRadioBattery;
            Player.InteractingLocker -= _player.OnInteractingLocker;
            Player.UnlockingGenerator -= _player.OnUnlockingGenerator;
            Player.EnteringPocketDimension -= _player.OnEnteringPocketDimension;

            Server.RestartingRound -= _server.OnRestartingRound;
            Server.RoundStarted -= _server.OnRoundStarted;
            Server.RoundEnded -= _server.OnEndedRound;

            Warhead.Stopping -= _warhead.OnStopping;
            Warhead.Starting -= _warhead.OnStarting;
            Warhead.Detonated -= _warhead.OnDetonated;

            _harmony = null;

            _map = null;
            _player = null;
            _server = null;
            _warhead = null;

            base.OnDisabled();
        }

        public override void OnReloaded() { }

        public override void OnRegisteringCommands() { }

        public override void OnUnregisteringCommands() { }
    }
}
