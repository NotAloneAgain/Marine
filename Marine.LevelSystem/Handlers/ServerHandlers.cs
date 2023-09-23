using Exiled.API.Features;
using Exiled.Events.EventArgs.Server;
using Marine.LevelSystem.API;

namespace Marine.LevelSystem.Handlers
{
    internal sealed class ServerHandlers
    {
        public void OnRoundEnded(RoundEndedEventArgs ev)
        {
            if ((byte)ev.LeadingTeam == 3)
            {
                return;
            }

            foreach (Player player in Player.List)
            {
                if (player.LeadingTeam != ev.LeadingTeam)
                {
                    continue;
                }

                player.Reward(100, "победу");
            }
        }
    }
}
