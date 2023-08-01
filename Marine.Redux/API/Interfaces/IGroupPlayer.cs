using Exiled.API.Features;
using System.Collections.Generic;

namespace Marine.Redux.API.Interfaces
{
    public interface IGroupPlayer
    {
        public HashSet<Player> Players { get; internal set; }
    }
}
