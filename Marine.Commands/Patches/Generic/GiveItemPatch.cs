using CommandSystem;
using CommandSystem.Commands.RemoteAdmin.Inventory;
using Exiled.API.Extensions;
using Exiled.API.Features;
using HarmonyLib;
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

                if (player.GroupName != "don4" && player.GroupName.Contains("don"))
                {
                    if (targets.Count > 1 || targets.Count == 0 || targets.Any(target => target.characterClassManager.UserId != player.UserId))
                    {
                        response = "Вы можете выдавать предметы только себе";
                        return false;
                    }

                    if (items.Count() > 1)
                    {
                        response = "Ты можешь выдать только один предмет за раз.";
                        return false;
                    }

                    ItemType item = items.First();

                    if (Round.ElapsedTime.TotalMinutes < 5 && (item.IsWeapon() || item is ItemType.GrenadeHE or ItemType.SCP244a or ItemType.SCP244b or ItemType.SCP018))
                    {
                        response = "Атятя, 5 минут ещё не прошли";
                        return false;
                    }

                    if (Round.ElapsedTime.TotalMinutes < 4 && item.IsKeycard() && (int)item > 3)
                    {
                        response = "Атятя, 4 минуты ещё не прошли";
                        return false;
                    }

                    var max = player.GroupName switch
                    {
                        "don3" or "don2" or "don1" => 3,
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

                    handled++;
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
