﻿using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.Handlers;
using HarmonyLib;
using Marine.Redux.API.Enums;
using Marine.Redux.API.Subclasses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Marine.Redux
{
    public sealed class Plugin : Exiled.API.Features.Plugin<Config>
    {
        private const string HarmonyId = "NotAloneAgain.Redux";

        private Harmony _harmony;
        private Type _subclassType;
        private List<Subclass> _subclasses;

        public Plugin()
        {
            _subclassType = typeof(Subclass);
            _subclasses = new List<Subclass>(100);
        }

        public override string Name => "Marine.Redux";

        public override string Prefix => "marine.redux";

        public override string Author => "NotAloneAgain";

        public override Version Version { get; } = new(1, 0, 0);

        public override void OnEnabled()
        {
            _harmony = new(HarmonyId);

            _harmony.PatchAll(Assembly);

            foreach (Type type in Assembly.GetTypes())
            {
                if (!type.IsClass || type.IsAbstract || !type.IsSubclassOf(_subclassType))
                {
                    continue;
                }

                Subclass subclass = Activator.CreateInstance(type) as Subclass;

                subclass.Subscribe();

                _subclasses.Add(subclass);
            }

            Player.Died += OnDied;
            Player.Hurting += OnHurting;
            Player.Destroying += OnDestroying;
            Player.ChangingRole += OnChangingRole;
            Player.TriggeringTesla += OnTriggeringTesla;

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            foreach (Subclass subclass in _subclasses)
            {
                subclass.Unsubscribe();
            }

            Player.TriggeringTesla -= OnTriggeringTesla;
            Player.ChangingRole -= OnChangingRole;
            Player.Destroying -= OnDestroying;
            Player.Hurting -= OnHurting;
            Player.Died -= OnDied;

            _subclasses.Clear();

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

        private void OnHurting(HurtingEventArgs ev)
        {
            if (ev.Player == null || ev.Player.IsHost || ev.Player.IsGodModeEnabled || !ev.IsAllowed)
            {
                return;
            }

            bool playerHas = Subclass.HasAny(ev.Player);
            bool attackerHas = Subclass.HasAny(ev.Attacker);

            if (!playerHas && !attackerHas || ev.Player.UserId == (ev.Attacker?.UserId ?? string.Empty))
            {
                return;
            }

            if (playerHas)
            {
                Subclass subclass = Subclass.ReadOnlyCollection.First(sub => sub.Has(ev.Player));

                subclass.OnHurt(ev);

                ev.Amount *= subclass.HurtMultiplayer;
            }

            if (ev.Attacker != null && attackerHas)
            {
                Subclass subclass = Subclass.ReadOnlyCollection.First(sub => sub.Has(ev.Attacker));

                subclass.OnDamage(ev);

                ev.Amount *= subclass.DamageMultiplayer;
            }
        }

        private void OnDestroying(DestroyingEventArgs ev)
        {
            if (ev.Player == null || !Subclass.HasAny(ev.Player))
            {
                return;
            }

            Subclass subclass = Subclass.ReadOnlyCollection.First(sub => sub.Has(ev.Player));

            subclass.Revoke(ev.Player, RevokeReason.Leave);
        }

        private void OnDied(DiedEventArgs ev)
        {
            if (ev.Player == null || !Subclass.HasAny(ev.Player))
            {
                return;
            }

            Subclass subclass = Subclass.ReadOnlyCollection.First(sub => sub.Has(ev.Player));

            subclass.Revoke(ev.Player, RevokeReason.Died);
        }

        private void OnChangingRole(ChangingRoleEventArgs ev)
        {
            if ((int)ev.SpawnFlags != 3 && ev.SpawnFlags != PlayerRoles.RoleSpawnFlags.All || !ev.IsAllowed || ev.Reason == SpawnReason.None)
            {
                return;
            }

            if ((ev.Player.RoleManager?.CurrentRole?.RoleTypeId ?? PlayerRoles.RoleTypeId.None) == PlayerRoles.RoleTypeId.Scp3114
                || ev.Player.Role.Is<Scp3114Role>(out _)
                || ev.NewRole == PlayerRoles.RoleTypeId.Scp3114)
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
                    if (ev.NewRole != subclass.Role)
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
