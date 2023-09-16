using CommandSystem;
using CommandSystem.Commands.RemoteAdmin;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using HarmonyLib;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;

namespace Marine.Commands.Patches.Generic
{
    [HarmonyPatch(typeof(ForceRoleCommand), nameof(ForceRoleCommand.Execute))]
    public static class ForceclassPatch
    {
        private static Dictionary<string, int> _usings;

        static ForceclassPatch() => _usings = new();

        private static bool Prefix(ForceRoleCommand __instance, ArraySegment<string> arguments, ICommandSender sender, out string response, ref bool __result)
        {
            __result = false;

            if (!ReferenceHub.TryGetHostHub(out ReferenceHub hub))
            {
                response = "You are not connected to a server.";
                return false;
            }

            if (arguments.Count < 2)
            {
                response = "To execute this command provide at least 2 arguments!\nUsage: " + arguments.Array[0] + " " + __instance.DisplayCommandUsage();
                return false;
            }

            var senderHub = (sender as PlayerCommandSender).ReferenceHub;

            List<ReferenceHub> list = RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out var array, false);
            bool self = list.Count == 1 && senderHub == list[0];

            if (!__instance.TryParseRole(array[0], out var roleBase))
            {
                response = "Invalid role ID / name.";
                return false;
            }

            if (!__instance.HasPerms(roleBase.RoleTypeId, self, sender, out response))
            {
                return false;
            }

            __instance.ProvideRoleFlag(array, out var flags);
            var role = roleBase.RoleTypeId;

            if (Player.TryGet(senderHub, out var player) && player.GroupName != "don4" && (player.GroupName.Contains("don") || player.GroupName.Contains("cons")))
            {
                if (flags != RoleSpawnFlags.All && (int)flags != 3)
                {
                    response = "Разрешено только с флагами!";
                    return false;
                }

                if (role is RoleTypeId.Overwatch or RoleTypeId.Filmmaker or RoleTypeId.Tutorial or RoleTypeId.Scp0492)
                {
                    response = "В эту роль вам запрещено меняться!";
                    return false;
                }

                var team = RoleExtensions.GetTeam(role);
                var leading = team.GetLeadingTeam();
                var ntfOrChaos = team is Team.FoundationForces or Team.ChaosInsurgency;
                var players = Player.List.Where(x => x.IsAlive && x.UserId != player.UserId);

                if (ntfOrChaos && (Round.ElapsedTime.TotalMinutes < 3 || players.Any(x => x.Zone == ZoneType.Surface && x.LeadingTeam != leading && x.Role.Base is FpcStandardRoleBase fpcRole && !fpcRole.IsAFK)))
                {
                    response = "3 минуты ещё не прошло или же кто-то есть на улице и он не помечен АФК.";
                    return false;
                }

                if (!ntfOrChaos && (Round.ElapsedTime.TotalMinutes > 10 || Warhead.IsDetonated))
                {
                    response = "Прошло 10 минут с начала раунда/взорвана боеголовка.";
                    return false;
                }

                if (role == RoleTypeId.FacilityGuard && players.Any(x => x.Zone == ZoneType.Entrance && x.LeadingTeam != leading && x.Role.Base is FpcStandardRoleBase fpcRole && !fpcRole.IsAFK))
                {
                    response = "В офисах ходит какой-то человек, нельзя.\nP.S. Если спалим что ты так чекаешь людей в офисах пойдешь на небо за звездочками.";
                    return false;
                }

                if (team == Team.SCPs && (players.Count(x => x.Role.Type == role) >= (role == RoleTypeId.Scp939 ? 2 : 1) || Round.ElapsedTime.TotalMinutes > 2))
                {
                    response = "Уже прошло более двух минут с начала раунда";
                    return false;
                }

                int max = player.GroupName switch
                {
                    "don3" => 3,
                    "don2" or "don1" => 2,
                    _ => 5
                };

                if (!_usings.ContainsKey(player.UserId))
                {
                    _usings.Add(player.UserId, 0);
                }

                if (_usings[player.UserId] > max)
                {
                    response = "Ты уже максимальное кол-во раз использовал донат!";
                    return false;
                }

                _usings[player.UserId]++;
            }

            return true;
        }

        public static void Reset() => _usings.Clear();
    }
}
