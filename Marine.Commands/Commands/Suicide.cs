﻿using Exiled.API.Features;
using Marine.Commands.API;
using Marine.Commands.API.Abstract;
using Marine.Commands.API.Enums;
using Marine.Redux.API.Subclasses;
using System.Collections.Generic;
using System.Linq;

namespace Marine.Commands.Commands
{
    public sealed class Suicide : CommandBase
    {
        public override string Command { get; set; } = "suicide";

        public override string Description { get; set; } = "Команда для суицида.";

        public override List<int> Counts { get; set; } = new List<int>(1) { 0 };

        public override Dictionary<int, string> Syntax { get; set; } = new Dictionary<int, string>()
        {
            { 0, string.Empty }
        };

        public override List<CommandType> Types { get; set; } = new List<CommandType>(1) { CommandType.PlayerConsole };

        public override CommandPermission Permission { get; set; } = new()
        {
            IsLimited = false,
        };

        public override bool ParseSyntax(List<string> input, int count, out List<object> output)
        {
            output = new List<object>();

            return true;
        }

        public override CommandResultType Handle(List<object> arguments, Player player, out string response)
        {
            response = string.Empty;

            if (!player.IsAlive || !player.IsHuman || player.IsTutorial)
            {
                response = "Суицид - не выход.";

                return CommandResultType.Fail;
            }

            if (Subclass.HasAny(player))
            {
                var sub = Subclass.ReadOnlyCollection.First(x => x.Has(player));

                sub.Revoke(player, Redux.API.Enums.RevokeReason.None);
            }

            player.Kill("Вскрылся...");

            return CommandResultType.Success;
        }
    }
}
