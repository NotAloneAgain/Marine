using Exiled.API.Features;
using Interactables.Interobjects.DoorUtils;
using Marine.Commands.API;
using Marine.Commands.API.Abstract;
using Marine.Commands.API.Enums;
using Marine.Redux.API.Subclasses;
using Marine.Redux.Subclasses.Scientists.Single;
using System.Collections.Generic;

namespace Marine.Commands.Commands
{
    public sealed class Knock : CooldownCommand
    {
        public override string Command { get; set; } = "knock";

        public override string Description { get; set; } = "Команда для того чтобы выбить дверь с ноги.";

        public override List<CommandType> Types { get; set; } = new List<CommandType>(1) { CommandType.PlayerConsole };

        public override List<int> Counts { get; set; } = new List<int>(1) { 0 };

        public override Dictionary<int, string> Syntax { get; set; } = new Dictionary<int, string>()
        {
            { 0, string.Empty }
        };

        public override CommandPermission Permission { get; set; } = new()
        {
            IsLimited = true,
        };

        public override int Cooldown { get; set; } = 100;

        public override CommandResultType Handle(List<object> arguments, Player player, out string response)
        {
            if (base.Handle(arguments, player, out response) == CommandResultType.Fail)
            {
                return CommandResultType.Fail;
            }

            Door door = player.GetDoorFromView(5);

            if (door == null || !door.IsBreakable || door.IsBroken || door.IsElevator || door.IsLocked)
            {
                response = "Цель нераспознана (дверь нельзя выбить блять бобик).";

                return CommandResultType.Fail;
            }

            if (door.IsGate)
            {
                response = "Бобик ты зачем ворота пытаешься выбить, нахуй пошел";

                return CommandResultType.Fail;
            }

            door.DamageDoor(250, DoorDamageType.Grenade);

            return CommandResultType.Success;
        }

        public override bool ParseSyntax(List<string> input, int count, out List<object> output)
        {
            output = new List<object>();

            return true;
        }

        public override bool CheckPermissions(Player player) => base.CheckPermissions(player) && Subclass.Has<GigaChad>(player);
    }
}
