/*using CommandSystem;
using CommandSystem.Commands.RemoteAdmin;
using Exiled.API.Features;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Marine.Commands.Patches
{
    [HarmonyPatch(typeof(BanCommand), nameof(BanCommand.Execute))]
    public class CustomBanPatch : CustomCommandPatch
    {
        private static CustomBanPatch _singleton;

        private CustomBanPatch() : base() { }

        public static CustomBanPatch Singleton => _singleton ??= new();

        public static bool Prefix(BanCommand __instance, ArraySegment<string> arguments, ICommandSender sender, out string response, ref bool __result)
        {

        }

        public override List<object> ParseArguments(List<string> args, Player sender)
        {
            var players = ParsePlayers(args[0], sender);

            if (players.Count > 1)
            {
                return null;
            }

            List<object> result = new()
            {
                players,
                ParseDuration(args[1]),
                string.Join(" ", args.Skip(2))
            };

            return result;
        }
    }
}*/