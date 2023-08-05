using Exiled.API.Features;
using Marine.MySQL.API;
using Marine.MySQL.API.Models;
using System.Collections.Generic;

namespace Marine.LevelSystem.API
{
    public static class Extensions
    {
        private static readonly Dictionary<string, Statistics> _stats;

        static Extensions() => _stats = new Dictionary<string, Statistics>(100);

        public static void Track(this Player player)
        {
            if (player == null || _stats.ContainsKey(player.UserId))
            {
                return;
            }

            Statistics stats = MySqlManager.Levels.Select(player.UserId);

            if (stats == null)
            {
                stats = new(player.UserId, 0, 0, 1);

                MySqlManager.Levels.Insert(stats);
            }

            _stats.Add(player.UserId, stats);

            player.RankName = $"{player.RankName} | Уровень {stats.Level}";
        }

        public static void Remove(this Player player)
        {
            if (player == null || !_stats.ContainsKey(player.UserId))
            {
                return;
            }

            MySqlManager.Levels.Update(_stats[player.UserId]);

            _stats.Remove(player.UserId);
        }

        public static void Reward(this Player player, int amount, string action)
        {
            var stats = _stats[player.UserId];

            stats.ApplyModifiers(ref amount);

            stats.AddExp(amount, action);
        }
    }
}
