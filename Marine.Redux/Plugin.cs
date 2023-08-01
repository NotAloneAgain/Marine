using System;

namespace Marine.Redux
{
    public sealed class Plugin : Exiled.API.Features.Plugin<Config>
    {
        public override string Name => "Marine.Redux";

        public override string Prefix => "marine.redux";

        public override string Author => "i.your";

        public override Version Version { get; } = new(1, 0, 0);

        public override void OnEnabled()
        {
            foreach (var subclass in Config.Subclasses.All)
            {
                subclass.Subscribe();
            }

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            foreach (var subclass in Config.Subclasses.All)
            {
                subclass.Unsubscribe();
            }

            base.OnDisabled();
        }

        public override void OnReloaded() { }

        public override void OnRegisteringCommands() { }

        public override void OnUnregisteringCommands() { }
    }
}
