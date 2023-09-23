using Exiled.Events.Handlers;
using HarmonyLib;
using Marine.Commands.Patches.Generic;
using System;

namespace Marine.Commands
{
    public sealed class Plugin : Exiled.API.Features.Plugin<Config>
    {
        private Harmony _harmony;

        public override string Name => "Marine.Commands";

        public override string Prefix => "marine.commands";

        public override string Author => "NotAloneAgain";

        public override Version Version { get; } = new(1, 0, 0);

        public override void OnRegisteringCommands()
        {
            _harmony = new("swear.to.god");

            foreach (API.Abstract.CommandBase command in Config.Commands.All)
            {
                command.Subscribe();
            }

            Server.RestartingRound += ForceclassPatch.Reset;
            Server.RestartingRound += GiveItemPatch.Reset;

            _harmony.PatchAll(Assembly);
        }

        public override void OnUnregisteringCommands()
        {
            _harmony.UnpatchAll("swear.to.god");

            Server.RestartingRound -= GiveItemPatch.Reset;
            Server.RestartingRound -= ForceclassPatch.Reset;

            foreach (API.Abstract.CommandBase command in Config.Commands.All)
            {
                command.Unsubscribe();
            }

            _harmony = null;
        }

        public override void OnReloaded() { }
    }
}
