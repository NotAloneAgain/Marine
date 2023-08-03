using Exiled.API.Features;
using Exiled.Events.EventArgs.Interfaces;
using Marine.MySQL.API.Models;
using System;

namespace Marine.MySQL.API.Events
{
    public class ChangedLevelEventArgs : EventArgs, IExiledEvent, IPlayerEvent
    {
        public ChangedLevelEventArgs(Player player, Statistics statistics, int old)
        {
            Player = player;
            Statistics = statistics;
            Old = old;
        }

        public Player Player { get; }

        public Statistics Statistics { get; }

        public int Old { get; }

        public int New => Statistics.Level;
    }
}
