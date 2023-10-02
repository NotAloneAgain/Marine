using Achievements;
using Exiled.API.Extensions;
using Exiled.API.Features;
using InventorySystem.Items.Usables;
using Marine.Commands.API;
using Marine.Commands.API.Abstract;
using Marine.Commands.API.Enums;
using Marine.Redux.API.Subclasses;
using Marine.Redux.Subclasses.Events.Halloween;
using Marine.Redux.Subclasses.Scientists.Group;
using PlayerStatsSystem;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Marine.Commands.Commands
{
    public sealed class Clothes : CooldownCommand
    {
        public override string Command { get; set; } = "clothes";

        public override string Description { get; set; } = "Команда для переодевания.";

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

        public override int Cooldown
        {
            get => DateTime.Now.Day == 31 ? 5 : 30;
            set
            {

            }
        }

        public override CommandResultType Handle(List<object> arguments, Player player, out string response)
        {
            if (base.Handle(arguments, player, out response) == CommandResultType.Fail)
            {
                return CommandResultType.Fail;
            }

            var target = player.GetRagdollFromView(6);

            if (target == null)
            {
                response = "Цель нераспознана";
                return CommandResultType.Fail;
            }

            if (target.Role is PlayerRoles.RoleTypeId.Scp173 or PlayerRoles.RoleTypeId.Scp106 or PlayerRoles.RoleTypeId.Scp0492 or PlayerRoles.RoleTypeId.Tutorial)
            {
                response = "Ты не можешь переодеться в этот труп.";
                return CommandResultType.Fail;
            }

            player.Role.Set(target.Role, PlayerRoles.RoleSpawnFlags.None);

            return CommandResultType.Success;
        }

        public override bool ParseSyntax(List<string> input, int count, out List<object> output)
        {
            output = new List<object>();

            return true;
        }

        public override bool CheckPermissions(Player player) => base.CheckPermissions(player) || Subclass.Has<TwoFaced>(player);
    }
}
