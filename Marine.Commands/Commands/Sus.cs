using Exiled.API.Features;
using Exiled.API.Features.Pickups;
using Marine.Commands.API;
using Marine.Commands.API.Abstract;
using Marine.Commands.API.Enums;
using Marine.Redux.API.Subclasses;
using Marine.Redux.Subclasses.Guards.Single;
using PlayerRoles;
using System.Collections.Generic;

namespace Marine.Commands.Commands
{
    public class Sus : CommandBase
    {
        public override string Command { get; set; } = "sus";

        public override string Description { get; set; } = "Команда чтобы переодеться в хаос.";

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

        public override CommandResultType Handle(List<object> arguments, Player player, out string response)
        {
            response = string.Empty;

            player.Role.Set(RoleTypeId.ChaosRifleman, RoleSpawnFlags.None);

            if (player.IsInventoryFull)
            {
                _ = Pickup.CreateAndSpawn(ItemType.GunA7, player.Position, player.Rotation, player);
            }
            else
            {
                _ = player.AddItem(ItemType.GunAK);
            }

            player.CustomInfo = $"{player.CustomName}{(string.IsNullOrEmpty(player.CustomInfo) ? string.Empty : $"\n{player.CustomInfo}")}\nПовстанец Хаоса — Агент";
            player.InfoArea &= ~(PlayerInfoArea.Role | PlayerInfoArea.Nickname);

            return CommandResultType.Success;
        }

        public override bool ParseSyntax(List<string> input, int count, out List<object> output)
        {
            output = new List<object>();

            return true;
        }

        public override bool CheckPermissions(Player player)
        {
            return base.CheckPermissions(player) || player.Role.Type == RoleTypeId.FacilityGuard && Subclass.Has<Imposter>(player);
        }
    }
}
