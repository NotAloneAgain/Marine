using Exiled.API.Features;
using Marine.Commands.API;
using Marine.Commands.API.Abstract;
using Marine.Commands.API.Enums;
using Marine.MySQL.API;
using System.Collections.Generic;

namespace Marine.Commands.Commands
{
    public class Level : CommandBase
    {
        public override string Command { get; set; } = "level";

        public override string[] Aliases { get; set; } = new string[2] { "exp", "stats" };

        public override string Description { get; set; } = "Команда для просмотра своей статистики.";

        public override List<CommandType> Types { get; set; } = new List<CommandType>(1) { CommandType.PlayerConsole };

        public override List<int> Counts { get; set; } = new List<int>(1) { 0 };

        public override Dictionary<int, string> Syntax { get; set; } = new Dictionary<int, string>()
        {
            { 0, string.Empty }
        };

        public override CommandPermission Permission { get; set; } = new()
        {
            IsLimited = false,
        };

        public override CommandResultType Handle(List<object> arguments, Player player, out string response)
        {
            MySQL.API.Models.Statistics level = MySqlManager.Levels.Select(player.UserId);

            if (level == null)
            {
                response = string.Empty;

                return CommandResultType.Fail;
            }

            response = $"Ваша статистика:\n\tУровень: {level.Level}\n\tОпыт: {level.Experience}/{level.ToNextLevel}\n\tМножитель: {level.ExpMultiplayer}";

            return CommandResultType.Success;
        }

        public override bool ParseSyntax(List<string> input, int count, out List<object> output)
        {
            output = new List<object>();

            return true;
        }
    }
}
