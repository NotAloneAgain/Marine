using Exiled.API.Features;
using Marine.LevelSystem.API;
using Marine.MySQL.API.Events;
using MEC;
using System.Collections.Generic;

namespace Marine.LevelSystem.Handlers
{
    internal sealed class LevelsHandlers
    {
        private readonly string _changedExpText;
        private readonly string _levelDownText;
        private readonly string _levelUpText;

        public LevelsHandlers(string exp, string down, string up)
        {
            _changedExpText = exp;
            _levelDownText = down;
            _levelUpText = up;
        }

        public void OnChangedExp(ChangedExpEventArgs ev)
        {
            ShowHint(ev.Player, string.Format(_changedExpText, ev.New - ev.Old, ev.Reason), 4);

            ev.Player.Update();
        }

        public void OnChangedLevel(ChangedLevelEventArgs ev)
        {
            if (ev.Old < ev.New)
            {
                ShowHint(ev.Player, string.Format(_levelUpText, ev.Old, ev.New), 6);
            }
            else
            {
                ShowHint(ev.Player, string.Format(_levelDownText, ev.Old, ev.New), 6);
            }

            ev.Player.Update();
        }

        private IEnumerator<float> ShowHint(Player player, string text, float duration)
        {
            yield return Timing.WaitForSeconds(player.CurrentHint.Duration);

            player.ShowHint(text, duration);
        }
    }
}
