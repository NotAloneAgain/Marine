using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Interactables.Interobjects.DoorUtils;
using Marine.Commands.API;
using Marine.Commands.API.Abstract;
using Marine.Commands.API.Enums;
using Marine.Redux.API.Subclasses;
using Marine.Redux.Subclasses.ClassD.Group;
using Marine.Redux.Subclasses.ClassD.Single;
using Marine.Redux.Subclasses.Guards.Group;
using PlayerRoles;
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

            if (player.IsCuffed)
            {
                response = "Ты связан бобик";

                return CommandResultType.Fail;
            }

            if (door == null)
            {
                response = "Цель не распознана.";

                return CommandResultType.Fail;
            }

            if (!door.Is<BreakableDoor>(out BreakableDoor breakable) || breakable.IsDestroyed || door.IsElevator || door.IsLocked && door.DoorLockType is DoorLockType.Lockdown2176 or DoorLockType.Regular079 || door.IsGate || IsBlocked(door.Type))
            {
                response = "Эту дверь нельзя сломать.";

                return CommandResultType.Fail;
            }

            _ = breakable.Damage(250, DoorDamageType.Grenade);

            return CommandResultType.Success;
        }

        public override bool ParseSyntax(List<string> input, int count, out List<object> output)
        {
            output = new List<object>();

            return true;
        }

        public override bool CheckPermissions(Player player)
        {
            return base.CheckPermissions(player) || player.Role.Team is Team.FoundationForces or Team.ChaosInsurgency && (player.Role.Type != RoleTypeId.FacilityGuard || Subclass.Has<Assault>(player)) || Subclass.Has<Gang>(player) || Subclass.Has<Scp073>(player) || Subclass.Has<GigaChad>(player);
        }

        private bool IsBlocked(DoorType door)
        {
            return door switch
            {
                DoorType.CheckpointArmoryA or DoorType.CheckpointArmoryB
                or DoorType.HczArmory or DoorType.LczArmory
                or DoorType.Scp049Armory or DoorType.Scp173Armory
                or DoorType.HID or DoorType.Intercom => true,
                _ => false,
            };
        }
    }
}
