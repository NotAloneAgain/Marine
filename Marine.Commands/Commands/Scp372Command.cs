/*using Exiled.API.Features;
using Marine.Commands.API;
using Marine.Commands.API.Abstract;
using Marine.Commands.API.Enums;
using Marine.Redux.API.Subclasses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Marine.Commands.Commands
{
    public sealed class Scp372Command : CooldownCommand
    {
        public override string Command { get; set; } = "scp372";

        public override string Description { get; set; } = "Команда для проявления SCP-372.";

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
            get => DateTime.Now.Day == 31 ? 60 : 90;
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

            Scp372 sub = Subclass.ReadOnlyCollection.First(x => x.Has(player)) as Scp372;

            sub.Run();

            return CommandResultType.Success;
        }

        public override bool ParseSyntax(List<string> input, int count, out List<object> output)
        {
            output = new List<object>();

            return true;
        }

        public override bool CheckPermissions(Player player) => base.CheckPermissions(player) || Subclass.Has<Scp372>(player);
    }
}*/