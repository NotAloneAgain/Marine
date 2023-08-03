using Exiled.API.Features;
using Marine.MySQL.API;
using Marine.MySQL.API.Models;
using System.Collections.Generic;

namespace Marine.LevelSystem.API
{
    public static class Extensions
    {
        private static readonly Dictionary<Player, Statistics> _stats;

        static Extensions() => _stats = new Dictionary<Player, Statistics>(100);

        public static void Track(this Player player)
        {
            if (player == null || _stats.ContainsKey(player))
            {
                return;
            }

            Statistics stats = MySqlManager.Levels.Select(player.UserId);

            if (stats == null)
            {
                stats = new(player.UserId, 0, 0, 1);

                MySqlManager.Levels.Insert(stats);
            }

            _stats.Add(player, stats);

            player.CustomInfo = $"Уровень {stats.Level}";
        }

        public static void Remove(this Player player)
        {
            if (player == null || !_stats.ContainsKey(player))
            {
                return;
            }

            MySqlManager.Levels.Update(_stats[player]);

            _stats.Remove(player);
        }

        public static void Reward(this Player player, int amount, string action)
        {
            var stats = _stats[player];

            stats.ApplyModifiers(ref amount);

            stats.AddExp(amount, action);
        }
    }
}
