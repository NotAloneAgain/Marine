using static Exiled.Events.Events;

namespace Marine.MySQL.API.Events
{
    public static class Handlers
    {
        public static event CustomEventHandler<ChangedExpEventArgs> ChangedExp;

        public static event CustomEventHandler<ChangedLevelEventArgs> ChangedLevel;

        public static void Invoke(ChangedExpEventArgs ev) => ChangedExp?.Invoke(ev);

        public static void Invoke(ChangedLevelEventArgs ev) => ChangedLevel?.Invoke(ev);
    }
}
