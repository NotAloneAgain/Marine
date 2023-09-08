using Exiled.API.Enums;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.Handlers;
using Marine.Redux.API.Enums;
using Marine.Redux.API.Subclasses;
using System;
using System.Linq;

namespace Marine.Redux
{
    public sealed class Plugin : Exiled.API.Features.Plugin<Config>
    {
        public override string Name => "Marine.Redux";

        public override string Prefix => "marine.redux";

        public override string Author => "NotAloneAgain";

        public override Version Version { get; } = new(1, 0, 0);

        public override void OnEnabled()
        {
            Player.ChangingRole += OnChangingRole;

            foreach (var subclass in Config.Subclasses.All)
            {
                subclass.Subscribe();
            }

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Player.ChangingRole -= OnChangingRole;

            foreach (var subclass in Config.Subclasses.All)
            {
                subclass.Unsubscribe();
            }

            base.OnDisabled();
        }

        public override void OnReloaded() { }

        public override void OnRegisteringCommands() { }

        public override void OnUnregisteringCommands() { }

        private void OnChangingRole(ChangingRoleEventArgs ev)
        {
            if ((int)ev.SpawnFlags != 3 && ev.SpawnFlags != PlayerRoles.RoleSpawnFlags.All || !ev.IsAllowed || (ev.Player.Group?.KickPower ?? 0) <= 0 && ev.Reason == SpawnReason.ForceClass)
            {
                return;
            }

            if (Subclass.HasAny(ev.Player))
            {
                var subclass = Subclass.ReadOnlyCollection.First(sub => sub.Has(ev.Player));

                if (ev.Reason == SpawnReason.Escaped)
                {
                    subclass.OnEscaping(ev.Player);

                    return;
                }

                subclass.Revoke(ev.Player, ev.Reason switch
                {
                    SpawnReason.Died => RevokeReason.Died,
                    SpawnReason.Destroyed => RevokeReason.Leave,
                    SpawnReason.ForceClass => RevokeReason.Admin,
                    _ => RevokeReason.None
                });
            }
            else
            {
                foreach (var subclass in Subclass.ReadOnlyCollection)
                {
                    if (subclass.Role != ev.NewRole)
                    {
                        continue;
                    }

                    if (subclass.Can(ev.Player))
                    {
                        subclass.Assign(ev.Player);

                        break;
                    }
                }
            }
        }
    }
}
