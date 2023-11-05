using CommandSystem;
using CommandSystem.Commands.RemoteAdmin.Inventory;
using Exiled.API.Extensions;
using Exiled.API.Features;
using HarmonyLib;
using Marine.Commands.API;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;

namespace Marine.Commands.Patches.Generic
{
    [HarmonyPatch(typeof(GiveCommand), nameof(GiveCommand.Execute))]
    public static class GiveItemPatch
    {
        private static readonly Dictionary<string, int> _usings;

        static GiveItemPatch() => _usings = new();

        private static bool Prefix(GiveCommand __instance, ArraySegment<string> arguments, ICommandSender sender, out string response, ref bool __result)
        {
            __result = false;

            if (!sender.CheckPermission(PlayerPermissions.GivingItems, out response))
            {
                return false;
            }

            if (arguments.Count < 2)
            {
                response = "To execute this command provide at least 2 arguments!\nUsage: " + arguments.Array[0] + " " + __instance.DisplayCommandUsage();
                return false;
            }

            List<ReferenceHub> targets = RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out var array, false);

            if (array == null || array.Length == 0)
            {
                response = "You must specify item(s) to give.";
                return false;
            }

            ItemType[] items = __instance.ParseItems(array[0]).ToArray();

            if (items.Length == 0)
            {
                response = "You didn't input any items.";
                return false;
            }

            var errors = 0;
            var handled = 0;
            var text = string.Empty;

            if (targets != null)
            {
                var player = Player.Get(sender);

                var overrideName = player.Group.GetNameByGroup();
                var isOverride = false;
                var isDonator = player.GroupName.IsDonator() || (isOverride = overrideName.IsDonator());

                if (isDonator)
                {
                    if (targets.Count > 1 || targets.Count == 0 || targets.Any(target => target.authManager.UserId != player.UserId))
                    {
                        response = "Вы можете выдавать предметы только себе";
                        return false;
                    }

                    ItemType item = items.First();

                    if (Round.ElapsedTime.TotalMinutes < 5 && (item.IsWeapon() || item is ItemType.GrenadeHE or ItemType.SCP244a or ItemType.SCP244b or ItemType.SCP018))
                    {
                        response = "Атятя, 5 минут ещё не прошли";
                        return false;
                    }

                    if (Round.ElapsedTime.TotalMinutes < 4 && (item.IsKeycard() && (int)item > 3 || item.IsScp()))
                    {
                        response = "Атятя, 4 минуты ещё не прошли";
                        return false;
                    }

                    if (!_usings.ContainsKey(player.UserId))
                    {
                        _usings.Add(player.UserId, 0);
                    }

                    var max = (isOverride ? overrideName : player.GroupName) switch
                    {
                        "don3" or "don2" or "don1" or "genshin" => 3,
                        _ => 5
                    };

                    var remaining = max - _usings[player.UserId];

                    var hub = targets.First();

                    foreach (ItemType type in items)
                    {
                        if (_usings[player.UserId] >= max || remaining < 0)
                        {
                            response = "Ты уже максимальное кол-во раз использовал донат!";
                            return false;
                        }

                        try
                        {
                            var blocked = type is ItemType.ParticleDisruptor or ItemType.SCP268 or ItemType.MicroHID or ItemType.Jailbird or ItemType.GunCom45 or ItemType.SCP018 or ItemType.Marshmallow or ItemType.Lantern;

                            if (blocked)
                            {
                                continue;
                            }

                            __instance.AddItem(hub, sender, item);

                            handled++;

                            _usings[player.UserId]++;

                            remaining--;
                        }
                        catch (Exception ex)
                        {
                            text = ex.Message;

                            errors++;
                        }
                    }

                    response = $"Ты выдал себе {handled} предметов и у тебя {(remaining == 0 ? "не осталось больше использований" : $"ещё {remaining} использований")}! {errors switch
                    {
                        0 => "Ошибок не было!",
                        1 => $"Была одна ошибка! {text}",
                        _ => $"Было {errors} ошибок! Последняя: {text}"
                    }}";

                    __result = true;

                    return false;
                }
                else
                {
                    foreach (ReferenceHub referenceHub in targets)
                    {
                        try
                        {
                            foreach (ItemType item in items)
                            {
                                if (player.UserId != "76561199011540209@steam" || player.Group.KickPower < 128)
                                {
                                    var blocked = (player.Group.KickPower >= 128) switch
                                    {
                                        true => item is ItemType.ParticleDisruptor,
                                        false => item is ItemType.ParticleDisruptor or ItemType.SCP268 or ItemType.MicroHID or ItemType.Jailbird or ItemType.GunCom45 or ItemType.SCP018
                                    };

                                    if (blocked)
                                    {
                                        continue;
                                    }
                                }

                                __instance.AddItem(referenceHub, sender, item);
                            }
                        }
                        catch (Exception ex)
                        {
                            text = ex.Message;

                            errors++;

                            continue;
                        }
                        finally
                        {
                            handled++;
                        }
                    }
                }
            }

            __result = true;

            response = (errors == 0) ? string.Format("Done! The request affected {0} player{1}!", handled, (handled == 1) ? string.Empty : "s") : string.Format("Failed to execute the command! Failures: {0}\nLast error log:\n{1}", errors, text);
            return false;
        }

        public static void Reset()
        {
            _usings.Clear();
        }
    }
}
