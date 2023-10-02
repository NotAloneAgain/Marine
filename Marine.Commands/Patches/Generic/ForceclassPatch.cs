using CommandSystem;
using CommandSystem.Commands.RemoteAdmin;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
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
        private static readonly Dictionary<string, int> _usings;

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

            ReferenceHub senderHub = (sender as PlayerCommandSender).ReferenceHub;

            List<ReferenceHub> list = RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out var array, false);
            var self = list.Count == 1 && senderHub == list[0];

            if (!__instance.TryParseRole(array[0], out PlayerRoleBase roleBase))
            {
                response = "Invalid role ID / name.";
                return false;
            }

            if (!__instance.HasPerms(roleBase.RoleTypeId, self, sender, out response))
            {
                return false;
            }

            __instance.ProvideRoleFlag(array, out RoleSpawnFlags flags);
            RoleTypeId role = roleBase.RoleTypeId;

            if (!Player.TryGet(senderHub, out Player player))
            {
                return true;
            }

            var overrideName = GetNameByGroup(player.Group);
            var isOverride = false;
            var isDonator = IsDonator(player.GroupName) || (isOverride = IsDonator(overrideName));

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

                if (role == player.Role.Type)
                {
                    response = "Вы и так играете за эту роль!";
                    return false;
                }

                Team team = RoleExtensions.GetTeam(role);
                LeadingTeam leading = team.GetLeadingTeam();

                var ntfOrChaos = team is Team.FoundationForces or Team.ChaosInsurgency && role != RoleTypeId.FacilityGuard;
                IEnumerable<Player> players = Player.List.Where(x => x.IsAlive && !x.IsNPC && x.LeadingTeam != leading && x.UserId != player.UserId && x.Role.Base is FpcStandardRoleBase fpcRole && !fpcRole.IsAFK);

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

                        if (Swap.StartScps[role] >= Swap.Slots[role] || Player.List.Count(ply => ply.Role.Type == role) >= Swap.Slots[role])
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

                var max = (isOverride ? overrideName : player.GroupName) switch
                {
                    "don3" => 3,
                    "don2" or "don1" => 2,
                    _ => 5
                };

                var remaining = max - _usings[player.UserId];

                if (_usings[player.UserId] >= max || remaining < 0)
                {
                    response = "Ты уже максимальное кол-во раз использовал донат!";
                    return false;
                }

                _usings[player.UserId]++;

                remaining--;

                response = string.Format("Вы успешно стали: {0}! {1}", role.Translate(), remaining > 0 ? $"Осталось {remaining} использований" : "Вы использовали максимальное кол-во раз!");

                __result = true;

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
                    }
                }

                if (Swap.AllowedScps.Contains(role))
                {
                    Swap.StartScps[role]++;

                    foreach (Player informator in Player.List)
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

            return true;
        }

        public static void Reset()
        {
            _usings.Clear();
        }

        private static bool IsDonator(string group)
        {
            return !string.IsNullOrEmpty(group) && group != "don4" && (group.Contains("don") || group.Contains("cons"));
        }

        private static string GetNameByGroup(UserGroup group)
        {
            PermissionsHandler handler = ServerStatic.GetPermissionsHandler();

            Dictionary<string, UserGroup> groups = handler.GetAllGroups();

            KeyValuePair<string, UserGroup>? pair = null!;

            foreach (KeyValuePair<string, UserGroup> gru in groups)
            {
                (var key, UserGroup g) = (gru.Key, gru.Value);

                if (g.Permissions == group.Permissions && g.RequiredKickPower == group.RequiredKickPower && g.KickPower == group.KickPower && g.BadgeText == group.BadgeText && g.BadgeColor == group.BadgeColor)
                {
                    pair = gru;

                    break;
                }
            }

            return pair is null ? string.Empty : pair.Value.Key;
        }

        private static void AddLog(int module, string msg, int type, bool init = false)
        {
            var text = TimeBehaviour.Rfc3339Time();

            var lockObject = ServerLogs.LockObject;

            lock (lockObject)
            {
                ServerLogs.Queue.Enqueue(new(msg, ServerLogs.Txt[type], ServerLogs.Modulestxt[module], text));
            }

            if (init)
            {
                return;
            }

            ServerLogs._state = ServerLogs.LoggingState.Write;
        }
    }
}
