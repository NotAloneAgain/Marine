using Exiled.API.Features;
using Marine.Commands.API;
using Marine.Commands.API.Abstract;
using Marine.Commands.API.Enums;
using Marine.Redux.API.Subclasses;
using Marine.Redux.Subclasses.ClassD.Single;
using System.Collections.Generic;

namespace Marine.Commands.Commands
{
    public class Item : CooldownCommand
    {
        public override string Command { get; set; } = "item";

        public override string Description { get; set; } = "Команда для выдачи предмета.";

        public override List<CommandType> Types { get; set; } = new List<CommandType>(1) { CommandType.PlayerConsole };

        public override List<int> Counts { get; set; } = new List<int>(1) { 1 };

        public override Dictionary<int, string> Syntax { get; set; } = new Dictionary<int, string>()
        {
            { 1, "[НОМЕР ПРЕДМЕТА]" }
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

            player.AddItem((ItemType)arguments[0]);

            return CommandResultType.Success;
        }

        public override bool ParseSyntax(List<string> input, int count, out List<object> output)
        {
            output = new();

            if (count != 1 || !int.TryParse(input[0], out var item) || item > 51 || item < 0)
            {
                return false;
            }

            output.Add((ItemType)item);

            return true;
        }

        public override bool CheckPermissions(Player player) => base.CheckPermissions(player) || Subclass.Has<Scp343>(player);
    }
}
