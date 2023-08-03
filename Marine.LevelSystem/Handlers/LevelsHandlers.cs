using Marine.MySQL.API.Events;

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
            ev.Player.ShowHint(string.Format(_changedExpText, ev.New - ev.Old, ev.Reason), 4);
        }

        public void OnChangedLevel(ChangedLevelEventArgs ev)
        {
            if (ev.Old < ev.New)
            {
                ev.Player.ShowHint(string.Format(_levelUpText, ev.Old, ev.New), 6);
            }
            else
            {
                ev.Player.ShowHint(string.Format(_levelDownText, ev.Old, ev.New), 6);
            }
        }
    }
}
