using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.API.Features.Pickups;
using Exiled.API.Features.Roles;
using Interactables.Interobjects.DoorUtils;
using MapGeneration;
using Marine.Commands.API;
using Marine.Commands.API.Abstract;
using Marine.Commands.API.Enums;
using Marine.Redux.API.Subclasses;
using Marine.Redux.Subclasses.ClassD.Single;
using PlayerRoles.PlayableScps.Scp106;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Marine.Commands.Commands
{
    public class Teleport : CommandBase
    {
        private static readonly Type[] _teleportObjects;

        static Teleport() => _teleportObjects = new Type[6]
        {
            typeof(Door),
            typeof(Room),
            typeof(Player),
            typeof(Pickup),
            typeof(TeslaGate),
            typeof(Window),
        };

        public override string Command { get; set; } = "tp";

        public override string Description { get; set; } = "Команда для телепортации.";

        public override List<CommandType> Types { get; set; } = new List<CommandType>(1) { CommandType.PlayerConsole };

        public override string[] Aliases { get; set; } = new string[1] { "teleport" };

        public override List<int> Counts { get; set; } = new List<int>(1) { 0, 1 };

        public override Dictionary<int, string> Syntax { get; set; } = new Dictionary<int, string>()
        {
            { 0, string.Empty },
            { 1, "[ТИП ОБЪЕКТА]" }
        };

        public override CommandPermission Permission { get; set; } = new()
        {
            IsLimited = true,
        };

        public override CommandResultType Handle(List<object> arguments, Player player, out string response)
        {
            response = string.Empty;

            Type type = arguments.Count == 0 ? _teleportObjects.GetRandomValue() : arguments[0] as Type;

            TryTeleport(player, type);

            return CommandResultType.Success;
        }

        public override bool ParseSyntax(List<string> input, int count, out List<object> output)
        {
            output = new();

            if (count == 1)
            {
                switch (input[0].ToLower())
                {
                    case "игрок" or "player":
                        {
                            output.Add(typeof(Player));

                            break;
                        }
                    case "дверь" or "door":
                        {
                            output.Add(typeof(Door));

                            break;
                        }
                    case "комната" or "room":
                        {
                            output.Add(typeof(Room));

                            break;
                        }
                }
            }

            return true;
        }

        public override bool CheckPermissions(Player player)
        {
            return base.CheckPermissions(player) || Subclass.Has<Scp343>(player);
        }

        private void TryTeleport(Player player, Type type)
        {
            if (type.Name == "Room")
            {
                Room room;
                do
                {
                    room = Room.List.GetRandomValue();
                } while (room is null || room.Zone == ZoneType.LightContainment && Map.IsLczDecontaminated);

                if (Warhead.IsDetonated)
                {
                    room = Room.Get(RoomType.Surface);
                }

                Vector3 position = room.Position;

                FpcRole role = player.Role.As<FpcRole>();

                if (room.Identifier == null || role == null)
                {
                    player.Teleport(position);

                    return;
                }

                DoorVariant[] whitelistedDoorsForZone = Scp106PocketExitFinder.GetWhitelistedDoorsForZone(room.Identifier.Zone);

                if (whitelistedDoorsForZone.Length != 0)
                {
                    DoorVariant randomDoor = Scp106PocketExitFinder.GetRandomDoor(whitelistedDoorsForZone);

                    float num = room.Identifier.Zone == FacilityZone.Surface ? 45 : 11;

                    position = Scp106PocketExitFinder.GetSafePositionForDoor(randomDoor, num, role.FirstPersonController.FpcModule.CharController);

                    player.Teleport(position);

                    return;
                }

                player.Teleport(position);

                return;
            }

            player.RandomTeleport(type);
        }
    }
}
