using CommandSystem;
using CommandSystem.Commands.RemoteAdmin;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Roles;
using HarmonyLib;
using Marine.Redux.API;
using Marine.Redux.API.Subclasses;
using Marine.Redux.Subclasses.Guards.Single;
using Marine.ScpSwap.API;
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

            if (!Player.TryGet(senderHub, out var player))
            {
                return true;
            }

            string overrideName = GetNameByGroup(player.Group);
            bool isOverride = false;
            bool isDonator = IsDonator(player.GroupName) || (isOverride = !string.IsNullOrEmpty(overrideName) && IsDonator(overrideName));
            var team = RoleExtensions.GetTeam(role);
            var leading = team.GetLeadingTeam();

            int max = (isOverride ? overrideName : player.GroupName) switch
            {
                "don3" => 3,
                "don2" or "don1" => 2,
                _ => 5
            };

            if (isDonator)
            {
                if (!Round.IsStarted)
                {
                    response = "Дождитесь начала раунда!";
                    return false;
                }

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

                var ntfOrChaos = team is Team.FoundationForces or Team.ChaosInsurgency && role != RoleTypeId.FacilityGuard;
                var players = Player.List.Where(x => x.IsAlive && !x.IsNPC && x.LeadingTeam != leading && x.UserId != player.UserId && x.Role.Base is FpcStandardRoleBase fpcRole && !fpcRole.IsAFK);

                if (ntfOrChaos)
                {
                    if (Round.ElapsedTime.TotalMinutes < 3)
                    {
                        response = "3 минуты с начала раунда ещё не прошло.";
                        return false;
                    }

                    if (players.Any(x => x.Zone == ZoneType.Surface))
                    {
                        response = "Кто-то есть на улице и он не помечен АФК.";
                        return false;
                    }
                }
                else
                {
                    if (Round.ElapsedTime.TotalMinutes > 10)
                    {
                        response = "Прошло 10 минут с начала раунда.";
                        return false;
                    }

                    if (Warhead.IsDetonated)
                    {
                        response = "Альфа-Боеголовка взорвана.";
                        return false;
                    }

                    if (role == RoleTypeId.FacilityGuard && players.Any(x => x.Zone == ZoneType.Entrance))
                    {
                        response = "Кто-то есть в офисной зоне и он не помечен АФК.";
                        return false;
                    }

                    if (team == Team.SCPs)
                    {
                        if (Round.ElapsedTime.TotalMinutes > 5)
                        {
                            response = "Уже прошло более пяти минут с начала раунда";
                            return false;
                        }

                        if (players.Count(x => x.Role.Team == Team.SCPs) >= 5)
                        {
                            response = "SCP-Объектов и так 5 или более.";
                            return false;
                        }

                        if (Swap.StartScps[role] >= Swap.Slots[role])
                        {
                            response = "Все слоты за данный объект заняты!";
                            return false;
                        }
                    }
                }

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

            __result = true;

            int forced = 0;

            foreach (ReferenceHub target in list)
            {
                if (target != null && role != RoleTypeId.Overwatch)
                {
                    target.roleManager.ServerSetRole(role, RoleChangeReason.RemoteAdmin, flags);

                    try
                    {
                        AddLog(2, string.Format("{0} changed role of player {1} to {2}.", sender.LogName, target.LoggedNameFromRefHub(), role), 1, false);
                    }
                    catch (Exception err)
                    {
                        Log.Error(err);
                    }

                    forced++;
                }
            }

            var remaining = max - _usings[player.UserId];

            response = isDonator switch
            {
                true => string.Format("Вы успешно стали: {0}! {1}", role.Translate(), remaining > 0 ? $"Осталось {remaining} использований" : "Вы использовали максимальное кол-во раз!"),
                false => string.Format("Done! Changed role of {0} player{1} to {2}!", forced, (forced == 1) ? "" : "s", role)
            };

            if (Swap.AllowedScps.Contains(role))
            {
                Swap.StartScps[role]++;

                foreach (var informator in Player.List)
                {
                    if (!Subclass.Has<Informator>(informator))
                    {
                        continue;
                    }

                    var text = $"Донатер стал {role.Translate()}";

                    informator.ShowHint($"<line-height=95%><size=95%><voffset=-20em><b><color=#FF9500>{text}</color></b></voffset></size>", 3);
                    informator.SendConsoleMessage(text, "yellow");
                }
            }

            return false;
        }

        public static void Reset() => _usings.Clear();

        private static bool IsDonator(string group) => group != "don4" && (group.Contains("don") || group.Contains("cons"));

        private static string GetNameByGroup(UserGroup group)
        {
            var handler = ServerStatic.GetPermissionsHandler();

            var groups = handler.GetAllGroups();

            KeyValuePair<string, UserGroup>? pair = null!;

            foreach (var gru in groups)
            {
                (string key, UserGroup g) = (gru.Key, gru.Value);

                if (g.Permissions == group.Permissions && g.RequiredKickPower == group.RequiredKickPower && g.KickPower == group.KickPower && g.BadgeText == group.BadgeText && g.BadgeColor == group.BadgeColor)
                {
                    pair = gru;

                    break;
                }
            }

            if (pair is null)
            {
                return string.Empty;
            }

            return pair.Value.Key;
        }

        private static void AddLog(int module, string msg, int type, bool init = false)
        {
            string text = TimeBehaviour.Rfc3339Time();

            object lockObject = ServerLogs.LockObject;

            lock (lockObject)
            {
                ServerLogs.Queue.Enqueue(new (msg, ServerLogs.Txt[type], ServerLogs.Modulestxt[module], text));
            }

            if (init)
            {
                return;
            }

            ServerLogs._state = ServerLogs.LoggingState.Write;
        }
    }
}
