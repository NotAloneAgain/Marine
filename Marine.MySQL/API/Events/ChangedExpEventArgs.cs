using Exiled.API.Features;
using Exiled.Events.EventArgs.Interfaces;
using Marine.MySQL.API.Models;
using System;

namespace Marine.MySQL.API.Events
{
    public class ChangedExpEventArgs : EventArgs, IExiledEvent, IPlayerEvent
    {
        public ChangedExpEventArgs(Player player, Statistics statistics, int old, string reason)
        {
            Player = player;
            Statistics = statistics;
            Old = old;
            Reason = reason;
        }

        public Player Player { get; }

        public Statistics Statistics { get; }

        public int Old { get; }

        public string Reason { get; }

        public int New => Statistics.Experience;
    }
}
