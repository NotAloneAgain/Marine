using Exiled.Events.Handlers;
using Marine.LevelSystem.Handlers;
using System;

namespace Marine.LevelSystem
{
    public sealed class Plugin : Exiled.API.Features.Plugin<Config>
    {
        private ServerHandlers _server;
        private PlayerHandlers _player;
        private LevelsHandlers _levels;

        public override string Name => "Marine.LevelSystem";

        public override string Prefix => "marine.level_system";

        public override string Author => "NotAloneAgain";

        public override Version Version { get; } = new(1, 0, 0);

        public override void OnEnabled()
        {
            _server = new();
            _player = new();
            _levels = new(Config.ExperienceGained, Config.LevelDown, Config.LevelUp);

            Server.RoundEnded += _server.OnRoundEnded;

            Scp079.GainingExperience += _player.OnGainingExperience;
            Scp049.FinishingRecall += _player.OnFinishingRecall;
            Player.Destroying += _player.OnDestroying;
            Player.Escaping += _player.OnEscaping;
            Player.Verified += _player.OnVerified;
            Player.Died += _player.OnDied;

            MySQL.API.Events.Handlers.ChangedExp += _levels.OnChangedExp;
            MySQL.API.Events.Handlers.ChangedLevel += _levels.OnChangedLevel;

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Server.RoundEnded -= _server.OnRoundEnded;

            Player.Died -= _player.OnDied;
            Player.Verified -= _player.OnVerified;
            Player.Escaping -= _player.OnEscaping;
            Player.Destroying -= _player.OnDestroying;
            Scp049.FinishingRecall -= _player.OnFinishingRecall;
            Scp079.GainingExperience -= _player.OnGainingExperience;

            MySQL.API.Events.Handlers.ChangedExp -= _levels.OnChangedExp;
            MySQL.API.Events.Handlers.ChangedLevel -= _levels.OnChangedLevel;

            _levels = null;
            _player = null;
            _server = null;

            base.OnDisabled();
        }

        public override void OnReloaded() { }

        public override void OnRegisteringCommands() { }

        public override void OnUnregisteringCommands() { }
    }
}
