using Exiled.API.Features;
using Exiled.API.Features.Pickups;
using MapGeneration.Distributors;
using Marine.Commands.API;
using Marine.Commands.API.Abstract;
using Marine.Commands.API.Enums;
using Marine.Redux.API.Subclasses;
using Marine.Redux.Subclasses.ClassD.Single;
using System;
using System.Collections.Generic;

namespace Marine.Commands.Commands
{
    public class Teleport : CooldownCommand
    {
        private static readonly IEnumerable<Type> _teleportObjects;

        static Teleport() => _teleportObjects = new List<Type>(10)
        {
            typeof(Camera),
            typeof(Door),
            typeof(Room),
            typeof(Player),
            typeof(Pickup),
            typeof(Locker),
            typeof(TeslaGate),
            typeof(Window),
            typeof(Generator)
        };

        public override string Command { get; set; } = "tp";

        public override string Description { get; set; } = "Команда для выдачи предмета.";

        public override List<CommandType> Types { get; set; } = new List<CommandType>(1) { CommandType.PlayerConsole };

        public override string[] Aliases { get; set; } = new string[1] { "teleport" };

        public override List<int> Counts { get; set; } = new List<int>(1) { 0 };

        public override Dictionary<int, string> Syntax { get; set; } = new Dictionary<int, string>()
        {
            { 0, string.Empty }
        };

        public override CommandPermission Permission { get; set; } = new()
        {
            IsLimited = true,
        };

        public override int Cooldown { get; set; } = 5;

        public override CommandResultType Handle(List<object> arguments, Player player, out string response)
        {
            if (base.Handle(arguments, player, out response) == CommandResultType.Fail)
            {
                return CommandResultType.Fail;
            }

            player.RandomTeleport(_teleportObjects);

            return CommandResultType.Success;
        }

        public override bool ParseSyntax(List<string> input, int count, out List<object> output)
        {
            output = new();

            return true;
        }

        public override bool CheckPermissions(Player player) => base.CheckPermissions(player) || Subclass.Has<Scp343>(player);
    }
}
