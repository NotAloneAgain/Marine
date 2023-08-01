using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.Events.EventArgs.Player;
using Marine.ScpSwap.API;

namespace Marine.ScpSwap.Handlers
{
    internal sealed class PlayerHandlers
    {
        private readonly string _text;
        private readonly float _duration;

        internal PlayerHandlers(string text, float duration)
        {
            _text = text;
            _duration = duration;
        }

        public void OnChangingRole(ChangingRoleEventArgs ev)
        {
            if (ev.Reason is not SpawnReason.RoundStart and not SpawnReason.LateJoin || ev.NewRole.GetTeam() != PlayerRoles.Team.SCPs)
            {
                return;
            }

            ev.Player.ShowHint(string.Format(_text, Swap.SwapDuration), _duration);
        }
    }
}
