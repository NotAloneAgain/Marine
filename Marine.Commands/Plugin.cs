using Exiled.Events.Handlers;
using HarmonyLib;
using Marine.Commands.API.Abstract;
using Marine.Commands.Patches.Generic;
using System;
using System.Collections.Generic;

namespace Marine.Commands
{
    public sealed class Plugin : Exiled.API.Features.Plugin<Config>
    {
        private const string HarmonyId = "NotAloneAgain.Commands";

        private Harmony _harmony;
        private Type _commandType;
        private List<CommandBase> _commands;

        public Plugin()
        {
            _commandType = typeof(CommandBase);
            _commands = new List<CommandBase>(25);
        }

        public override string Name => "Marine.Commands";

        public override string Prefix => "marine.commands";

        public override string Author => "NotAloneAgain";

        public override Version Version { get; } = new(1, 0, 0);

        public override void OnRegisteringCommands()
        {
            _harmony = new(HarmonyId);

            foreach (Type type in Assembly.GetTypes())
            {
                if (!type.IsClass || type.IsAbstract || !type.IsSubclassOf(_commandType))
                {
                    continue;
                }

                CommandBase command = Activator.CreateInstance(type) as CommandBase;

                command.Subscribe();

                _commands.Add(command);
            }

            Server.RestartingRound += ForceclassPatch.Reset;
            Server.RestartingRound += GiveItemPatch.Reset;

            _harmony.PatchAll(Assembly);
        }

        public override void OnUnregisteringCommands()
        {
            _harmony.UnpatchAll(HarmonyId);

            Server.RestartingRound -= GiveItemPatch.Reset;
            Server.RestartingRound -= ForceclassPatch.Reset;

            foreach (CommandBase command in _commands)
            {
                command.Unsubscribe();
            }

            _commands.Clear();

            _harmony = null;
        }

        public override void OnReloaded() { }
    }
}
