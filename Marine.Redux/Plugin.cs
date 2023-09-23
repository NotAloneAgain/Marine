using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.Handlers;
using HarmonyLib;
using Marine.Redux.API.Enums;
using Marine.Redux.API.Subclasses;
using System;
using System.Linq;

namespace Marine.Redux
{
    public sealed class Plugin : Exiled.API.Features.Plugin<Config>
    {
        private const string HarmonyId = "NotAloneAgain.Redux";

        private Harmony _harmony;

        public override string Name => "Marine.Redux";

        public override string Prefix => "marine.redux";

        public override string Author => "NotAloneAgain";

        public override Version Version { get; } = new(1, 0, 0);

        public override void OnEnabled()
        {
            _harmony = new(HarmonyId);

            _harmony.PatchAll(Assembly);

            Player.ChangingRole += OnChangingRole;
            Player.TriggeringTesla += OnTriggeringTesla;

            foreach (Subclass subclass in Config.Subclasses.All)
            {
                subclass.Subscribe();
            }

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Player.TriggeringTesla -= OnTriggeringTesla;
            Player.ChangingRole -= OnChangingRole;

            foreach (Subclass subclass in Config.Subclasses.All)
            {
                subclass.Unsubscribe();
            }

            _harmony.UnpatchAll(HarmonyId);

            _harmony = null;

            base.OnDisabled();
        }

        public override void OnReloaded() { }

        public override void OnRegisteringCommands() { }

        public override void OnUnregisteringCommands() { }

        private void OnTriggeringTesla(TriggeringTeslaEventArgs ev)
        {
            if (!Subclass.HasAny(ev.Player))
            {
                return;
            }

            Subclass subclass = Subclass.ReadOnlyCollection.First(sub => sub.Has(ev.Player));

            if (subclass == null || subclass.CanTriggerTesla)
            {
                return;
            }

            ev.IsAllowed = false;
            ev.IsInIdleRange = false;
            ev.IsInHurtingRange = false;
            ev.IsTriggerable = false;
        }

        private void OnChangingRole(ChangingRoleEventArgs ev)
        {
            if ((int)ev.SpawnFlags != 3 && ev.SpawnFlags != PlayerRoles.RoleSpawnFlags.All || !ev.IsAllowed || ev.Reason == SpawnReason.None)
            {
                return;
            }

            if (ev.NewRole.GetTeam() is PlayerRoles.Team.Dead or PlayerRoles.Team.OtherAlive)
            {
                if (Subclass.HasAny(ev.Player))
                {
                    Subclass subclass = Subclass.ReadOnlyCollection.First(sub => sub.Has(ev.Player));

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

                return;
            }

            if (Subclass.HasAny(ev.Player))
            {
                Subclass subclass = Subclass.ReadOnlyCollection.First(sub => sub.Has(ev.Player));

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
                foreach (Subclass subclass in Subclass.ReadOnlyCollection)
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
