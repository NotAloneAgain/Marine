using Exiled.API.Features;
using Marine.MySQL.API.Events;
using System;

namespace Marine.MySQL.API.Models
{
    public class Statistics
    {
        private protected Player _player;

        public Statistics(string userId, int level, int experience, short expMultiplayer)
        {
            UserId = userId;
            Level = level;
            Experience = experience;
            ExpMultiplayer = expMultiplayer;
        }

        public Player Player => _player ??= Player.Get(UserId);

        public string UserId { get; set; } = "(null)@steam";

        public int Level { get; set; } = 0;

        public int Experience { get; set; } = 0;

        public short ExpMultiplayer { get; set; } = 1;

        public int ToNextLevel => 100 + 150 * Level;

        public void ApplyModifiers(ref int exp)
        {
            exp *= ExpMultiplayer;

            var time = DateTime.Now;

            if (time.Hour is >= 8 and < 10 or >= 22)
            {
                exp *= 2;
            }
            else if (time.Hour is >= 0 and <= 7)
            {
                exp *= 3;
            }
        }

        public void AddExp(int value, string reason = "")
        {
            if (value == 0)
            {
                return;
            }

            Experience += value;

            Handlers.Invoke(new ChangedExpEventArgs(Player, this, Experience - value, reason));

            var old = Level;

            while (Experience >= ToNextLevel)
            {
                Experience -= ToNextLevel;
                Level++;
            }

            while (Experience < 0)
            {
                Experience = ToNextLevel + Experience;
                Level--;
            }

            if (Level != old)
            {
                Handlers.Invoke(new ChangedLevelEventArgs(Player, this, old));
            }
        }
    }
}
