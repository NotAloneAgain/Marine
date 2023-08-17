/*using CommandSystem;
using CommandSystem.Commands.RemoteAdmin.MutingAndIntercom;
using Exiled.API.Features;
using HarmonyLib;
using PluginAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using VoiceChat;

namespace Marine.Commands.Patches
{
    [HarmonyPatch(typeof(MuteCommand), nameof(MuteCommand.Execute))]
    public class CustomMutePatch : CustomCommandPatch
    {
        private static PlayerPermissions[] _perms;
        private static CustomMutePatch _singleton;

        static CustomMutePatch() => _perms = new PlayerPermissions[]
        {
            PlayerPermissions.BanningUpToDay,
            PlayerPermissions.LongTermBanning,
            PlayerPermissions.PlayersManagement
        };

        private CustomMutePatch() : base() { }

        public static CustomMutePatch Singleton => _singleton ??= new();

        public static bool Prefix(MuteCommand __instance, ArraySegment<string> args, ICommandSender sender, out string response, ref bool __result)
        {
            __result = false;

            if (!sender.CheckPermission(_perms, out response))
            {
                return false;
            }

            if (args.Count < 1)
            {
                response = "To execute this command provide at least 1 argument!\nUsage: " + args.Array[0] + " " + __instance.DisplayCommandUsage();
                return false;
            }

            var ply = Player.Get(sender);

            var arguments = Singleton.ParseArguments(args.ToList(), ply);

            foreach (Player player in (List<Player>)arguments[0])
            {
                if (EventManager.ExecuteEvent(new PlayerMutedEvent(player.ReferenceHub, ply.ReferenceHub, false)))
                {
                    VoiceChatMutes.IssueLocalMute(player.UserId, false);
                }
            }

            __result = true;

            response = "Успешно!";
            return false;
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
                players
            };

            if (args.Count == 1)
            {
                return result;
            }

            result.Add(ParseDuration(args[1]));
            result.Add(string.Join(" ", args.Skip(2)));

            return result;
        }
    }
}*/
