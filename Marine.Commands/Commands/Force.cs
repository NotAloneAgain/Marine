using Exiled.API.Enums;
using Exiled.API.Features;
using Marine.Commands.API;
using Marine.Commands.API.Abstract;
using Marine.Commands.API.Enums;
using Marine.Redux.API;
using Marine.Redux.API.Subclasses;
using Marine.Redux.Subclasses.Guards.Single;
using Marine.ScpSwap.API;
using PlayerRoles;
using System.Collections.Generic;
using System.Linq;

namespace Marine.Commands.Commands
{
    public sealed class Force : CommandBase
    {
        public override string Command { get; set; } = "force";

        public override string Description { get; set; } = "Команда для смены своего SCP-Объекта.";

        public override List<int> Counts { get; set; } = new List<int>(1) { 1 };

        public override Dictionary<int, string> Syntax { get; set; } = new Dictionary<int, string>()
        {
            { 1, "[Номер]" }
        };

        public override List<CommandType> Types { get; set; } = new List<CommandType>(1) { CommandType.PlayerConsole };

        public override CommandPermission Permission { get; set; } = new()
        {
            IsLimited = true,
        };

        public override bool ParseSyntax(List<string> input, int count, out List<object> output)
        {
            output = new List<object>();

            if (count != 1)
            {
                return false;
            }

            var scp = input[0];

            if (!ushort.TryParse(scp, out var number))
            {
                return false;
            }

            RoleTypeId role = number switch
            {
                49 => RoleTypeId.Scp049,
                79 => RoleTypeId.Scp079,
                96 => RoleTypeId.Scp096,
                106 => RoleTypeId.Scp106,
                173 => RoleTypeId.Scp173,
                939 => RoleTypeId.Scp939,
                _ => RoleTypeId.None
            };

            output.Add(role);

            return role != RoleTypeId.None;
        }

        public override CommandResultType Handle(List<object> arguments, Player player, out string response)
        {
            response = string.Empty;

            if (arguments == null || !arguments.Any())
            {
                return CommandResultType.Fail;
            }

            RoleTypeId oldRole = player.Role.Type;
            var role = (RoleTypeId)arguments[0];

            if (role == player.Role.Type)
            {
                response = "Вы и так являетесь этим SCP-Объектом.";

                return CommandResultType.Fail;
            }

            if (player.Health != player.MaxHealth)
            {
                response = "Вы уже успели получить урон!";

                return CommandResultType.Fail;
            }

            if (Swap.Prevent && History.HasSuccessfulUse(player))
            {
                response = "Сменить роль можно лишь один раз.";

                return CommandResultType.Fail;
            }

            if (Round.ElapsedTime.TotalSeconds > Swap.SwapDuration)
            {
                response = $"Прошло более {Swap.SwapDuration} секунд после начала раунда.";

                return CommandResultType.Fail;
            }

            if (Swap.StartScps[role] >= Swap.Slots[role] || Player.List.Count(ply => ply.Role.Type == role) >= Swap.Slots[role])
            {
                response = "Все слоты за данный объект заняты.";

                return CommandResultType.Fail;
            }

            Swap.StartScps[oldRole]--;
            Swap.StartScps[role]++;

            player.Role.Set(role, SpawnReason.ForceClass, RoleSpawnFlags.All);

            var scp = role.Translate();

            player.ShowHint($"<line-height=95%><size=95%><voffset=-20em><b><color=#FF9500>Желаем удачной игры за {scp}!</color></b></voffset></size>", 6);

            foreach (Player informator in Player.List)
            {
                if (!Subclass.Has<Informator>(informator))
                {
                    continue;
                }

                var text = $"{oldRole.Translate()} стал {scp}";

                informator.ShowHint($"<line-height=95%><size=95%><voffset=-20em><b><color=#FF9500>{text}</color></b></voffset></size>", 3);
                informator.SendConsoleMessage(text, "yellow");
            }

            return CommandResultType.Success;
        }

        public override bool CheckPermissions(Player player)
        {
            return base.CheckPermissions(player) || player.IsScp && Swap.AllowedScps.Contains(player.Role);
        }
    }
}
