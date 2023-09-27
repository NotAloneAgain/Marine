using Exiled.API.Features;
using Marine.Commands.API;
using Marine.Commands.API.Abstract;
using Marine.Commands.API.Enums;
using Marine.Misc.API;
using Marine.Redux.API.Subclasses;
using Marine.Redux.Subclasses.Scientists.Single;
using PlayerRoles;
using System.Collections.Generic;

namespace Marine.Commands.Commands
{
    public class Zombie : CooldownCommand
    {
        public override string Command { get; set; } = "zombie";

        public override string Description { get; set; } = "Команда чтобы стать зомби.";

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

        public override int Cooldown { get; set; } = 120;

        public override CommandResultType Handle(List<object> arguments, Player player, out string response)
        {
            if (base.Handle(arguments, player, out response) == CommandResultType.Fail)
            {
                return CommandResultType.Fail;
            }

            player.DropAllWithoutKeycard();
            player.CurrentItem = null;
            player.Role.Set(RoleTypeId.Scp0492, RoleSpawnFlags.None);

            return CommandResultType.Success;
        }

        public override bool ParseSyntax(List<string> input, int count, out List<object> output)
        {
            output = new List<object>();

            return true;
        }

        public override bool CheckPermissions(Player player)
        {
            return base.CheckPermissions(player) || player.Role.Type == RoleTypeId.Scientist && Subclass.Has<Infected>(player);
        }
    }
}
