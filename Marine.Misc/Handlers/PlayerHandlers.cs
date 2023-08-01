using CustomPlayerEffects;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using Marine.Misc.API;
using Marine.Misc.Models;
using MEC;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp079;
using System.Collections.Generic;
using UnityEngine;

namespace Marine.Misc.Handlers
{
    internal sealed class PlayerHandlers
    {
        #region Initialize
        private readonly RealisticEffectsConfig _realisticEffects;
        private readonly BetterFirearmsConfig _betterFirearms;
        private readonly RemoteKeycardConfig _remoteKeycard;
        private readonly BetterRolesConfig _betterRoles;
        private readonly DefaultConfig _zombieInfection;
        private readonly DefaultConfig _infinityAmmo;

        public PlayerHandlers(Config config)
        {
            _realisticEffects = config.RealisticEffects;
            _betterFirearms = config.BetterFirearms;
            _remoteKeycard = config.RemoteKeycard;
            _betterRoles = config.BetterRoles;

            _zombieInfection = config.ZombieInfection;
            _infinityAmmo = config.InfinityAmmo;
        }
        #endregion
        #region InfinityAmmo
        public void OnReloadingWeapon(ReloadingWeaponEventArgs ev)
        {
            if (!_infinityAmmo.IsEnabled)
            {
                return;
            }

            ev.Player.SetAmmo(ev.Firearm.AmmoType, ev.Firearm.MaxAmmo);
        }
        #endregion
        #region RemoteKeycard
        public void OnInteractingDoor(InteractingDoorEventArgs ev)
        {
            if (!_remoteKeycard.IsEnabled || _remoteKeycard.CheckAmnesia && ev.Player.HasEffect<AmnesiaItems>()
                || ev.IsAllowed || ev.Door.IsMoving || ev.Door.IsLocked || ev.Door.IsBroken|| ev.Door.IsCheckpoint && ev.Door.IsOpen)
            {
                return;
            }

            ev.IsAllowed = ev.Player.CheckPermissions(ev.Door.RequiredPermissions.RequiredPermissions);
        }

        public void OnInteractingLocker(InteractingLockerEventArgs ev)
        {
            if (!_remoteKeycard.IsEnabled || _remoteKeycard.CheckAmnesia && ev.Player.HasEffect<AmnesiaItems>()
                || ev.IsAllowed)
            {
                return;
            }

            bool hasCheckpoints = ev.Player.CheckPermissions(Interactables.Interobjects.DoorUtils.KeycardPermissions.Checkpoints);
            bool hasContaiment = ev.Player.CheckPermissions(Interactables.Interobjects.DoorUtils.KeycardPermissions.ContainmentLevelTwo);

            ev.IsAllowed = hasCheckpoints && hasContaiment;
        }

        public void OnUnlockingGenerator(UnlockingGeneratorEventArgs ev)
        {
            if (!_remoteKeycard.IsEnabled || _remoteKeycard.CheckAmnesia && ev.Player.HasEffect<AmnesiaItems>()
                || ev.IsAllowed)
            {
                return;
            }

            ev.IsAllowed = ev.Player.CheckPermissions(ev.Generator.Base._requiredPermission);
        }
        #endregion
        #region BetterRoles
        public void OnChangingRole(ChangingRoleEventArgs ev)
        {
            if (!_betterRoles.IsEnabled || !ev.IsAllowed)
            {
                return;
            }

            if (ev.NewRole == RoleTypeId.Scp079)
            {
                Timing.RunCoroutine(_ChangeLostValue(ev.Player, _betterRoles.Scp079Ghostlight));

                return;
            }

            if (ev.NewRole.GetSide() != Side.ChaosInsurgency || ev.NewRole == RoleTypeId.ClassD)
            {
                return;
            }

            if (Random.Range(0, 101) >= 100 - _betterRoles.Chance)
            {
                var random = Random.Range(0, 101);

                if (random >= _betterRoles.FlashChance)
                {
                    ev.Items.Add(ItemType.GrenadeHE);

                    return;
                }

                ev.Items.Add(ItemType.GrenadeFlash);
            }
        }
        #endregion
        #region BetterFirearms
        public void OnShot(ShotEventArgs ev)
        {
            if (!_betterFirearms.IsEnabled
                || ev.Player.CurrentItem.Type is ItemType.ParticleDisruptor or ItemType.GunShotgun
                || !ev.CanHurt
                || ev.Target == null
                || ev.Player.IsScp)
            {
                return;
            }

            var weapon = ev.Player.CurrentItem.As<Firearm>();

            if (weapon == null || weapon.Ammo >= Mathf.RoundToInt(weapon.MaxAmmo * 0.6f))
            {
                return;
            }

            if (Random.Range(0, 101) == 100)
            {
                ev.Player.CurrentItem.As<Firearm>().Ammo = 0;

                ev.Player.ShowHint(_betterFirearms.HintText, 6);
            }
        }
        #endregion
        #region RealisticEffects
        public void OnHurting(HurtingEventArgs ev)
        {
            if (_realisticEffects.IsEnabled
                || !ev.IsAllowed || ev.Player == ev.Attacker || ev.Attacker == null || !ev.Player.IsHuman || (ev.Player.CurrentArmor?.Type ?? ItemType.None) == ItemType.ArmorHeavy)
            {
                return;
            }

            if (ev.DamageHandler.Type == DamageType.Scp0492)
            {
                ev.Player.EnableEffect(EffectType.Poisoned, _realisticEffects.ZombiePoison, _realisticEffects.AddDuration);

                return;
            }

            if (ev.DamageHandler.Type == DamageType.Scp939)
            {
                ev.Player.EnableEffect(EffectType.Bleeding, _realisticEffects.Bleeding939, _realisticEffects.AddDuration);

                return;
            }

            if (ev.DamageHandler.Type is DamageType.Firearm
                or DamageType.Revolver or DamageType.Crossvec or DamageType.AK or DamageType.E11Sr or DamageType.Fsp9
                or DamageType.Logicer or DamageType.Shotgun or DamageType.Com45 or DamageType.Com18)
            {
                ev.Player.EnableEffect(EffectType.Bleeding, _realisticEffects.BleedingShot, _realisticEffects.AddDuration);
            }
        }
        #endregion
        #region ZombieInfection
        public void OnDying(DyingEventArgs ev)
        {
            if (!_zombieInfection.IsEnabled
                || !ev.Player.IsHuman || !ev.IsAllowed)
            {
                return;
            }

            if ((ev.DamageHandler.Type == DamageType.Poison || ev.Attacker != null && ev.Attacker.Role.Type == RoleTypeId.Scp0492)
                && Random.Range(0, 101) >= 88)
            {
                ev.IsAllowed = false;

                ev.Player.DropAllWithoutKeycard();

                ev.Player.Role.Set(RoleTypeId.Scp0492, SpawnReason.Revived, RoleSpawnFlags.None);

                return;
            }
        }
        #endregion
        #region Others
        public void OnTriggeringTesla(TriggeringTeslaEventArgs ev)
        {
            if (!ev.IsAllowed || ev.Player.Role.Team != Team.FoundationForces && ev.Player.Role.Type != RoleTypeId.Scp079)
            {
                return;
            }

            ev.IsAllowed = false;
            ev.IsInIdleRange = false;
            ev.IsInHurtingRange = false;
            ev.IsTriggerable = false;
        }

        public void OnUsedItem(UsedItemEventArgs ev)
        {
            if (ev.Item.Type == ItemType.Painkillers)
            {
                ev.Player.GetEffect(EffectType.DamageReduction).ServerSetState(20, 180, true);
            }
        }

        #endregion
        #region Coroutines
        private IEnumerator<float> _ChangeLostValue(Player player, float value)
        {
            yield return Timing.WaitForSeconds(2f);

            var scp = player.Role.Base as Scp079Role;

            if (scp == null || !scp.SubroutineModule.TryGetSubroutine(out Scp079LostSignalHandler lost))
            {
                yield break;
            }

            lost._ghostlightLockoutDuration = value;
        }
        #endregion
    }
}
