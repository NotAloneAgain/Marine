using CustomPlayerEffects;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using Marine.Misc.API;
using Marine.Misc.Models;
using Marine.MySQL.API;
using MEC;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp079;
using System.Collections.Generic;
using System.Linq;
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
        private UserGroup _discordGroup;

        public PlayerHandlers(Config config)
        {
            _realisticEffects = config.RealisticEffects;
            _betterFirearms = config.BetterFirearms;
            _remoteKeycard = config.RemoteKeycard;
            _betterRoles = config.BetterRoles;

            _zombieInfection = config.ZombieInfection;
            _infinityAmmo = config.InfinityAmmo;

            _discordGroup = ServerStatic.GetPermissionsHandler()?.GetGroup("discord");
        }
        #endregion
        #region InfinityAmmo
        public void OnReloadingWeapon(ReloadingWeaponEventArgs ev)
        {
            if (!_infinityAmmo.IsEnabled || ev.Player.IsHost || ev.Player.IsNPC)
            {
                return;
            }

            ev.Player.SetAmmo(ev.Firearm.AmmoType, ev.Firearm.MaxAmmo);
        }
        #endregion
        #region RemoteKeycard
        public void OnInteractingDoor(InteractingDoorEventArgs ev)
        {
            if (!_remoteKeycard.IsEnabled || ev.Player.IsHost || ev.Player.IsNPC || ev.IsAllowed && ev.Player.IsHuman || !ev.Door.IsKeycardDoor || ev.Door.Is<BreakableDoor>(out BreakableDoor breakable) && breakable.IsDestroyed || ev.Door.IsMoving || _remoteKeycard.CheckAmnesia && ev.Player.HasEffect<AmnesiaItems>())
            {
                return;
            }

            if (ev.Player.IsScp && ev.Door.IsLocked && ev.Player.Role.Type != RoleTypeId.Scp079)
            {
                ev.IsAllowed = !ev.Door.IsGate && ev.Door.DoorLockType is DoorLockType.Regular079 or DoorLockType.Lockdown079;

                return;
            }

            if (ev.IsAllowed || ev.Door.IsLocked)
            {
                return;
            }

            ev.IsAllowed = (ev.Door.IsCheckpoint || ev.Door.Type is DoorType.CheckpointArmoryA or DoorType.CheckpointArmoryB) ? ev.Player.CheckPermissions(Interactables.Interobjects.DoorUtils.KeycardPermissions.Checkpoints) : ev.Player.CheckPermissions(ev.Door.RequiredPermissions.RequiredPermissions);
        }

        public void OnInteractingLocker(InteractingLockerEventArgs ev)
        {
            if (!_remoteKeycard.IsEnabled || _remoteKeycard.CheckAmnesia && ev.Player.HasEffect<AmnesiaItems>()
                || ev.IsAllowed || ev.Player.IsHost || ev.Player.IsNPC)
            {
                return;
            }

            var hasAccess = ev.Player.CheckPermissions(ev.Chamber.RequiredPermissions);
            var hasCheckpoints = ev.Player.CheckPermissions(Interactables.Interobjects.DoorUtils.KeycardPermissions.Checkpoints);
            var hasContainment = ev.Player.CheckPermissions(Interactables.Interobjects.DoorUtils.KeycardPermissions.ContainmentLevelTwo);

            ev.IsAllowed = hasAccess || ev.Locker.Loot.All(x => !x.TargetItem.IsWeapon()) && hasCheckpoints && hasContainment;
        }

        public void OnUnlockingGenerator(UnlockingGeneratorEventArgs ev)
        {
            if (ev.Player.IsHost || ev.Player.IsNPC)
            {
                return;
            }

            if (Round.ElapsedTime.TotalMinutes < 4)
            {
                ev.IsAllowed = false;

                return;
            }

            if (!_remoteKeycard.IsEnabled || _remoteKeycard.CheckAmnesia && ev.Player.HasEffect<AmnesiaItems>() || ev.IsAllowed)
            {
                return;
            }

            ev.IsAllowed = ev.Player.CheckPermissions(ev.Generator.Base._requiredPermission);
        }
        #endregion
        #region BetterRoles
        public void OnChangingRole(ChangingRoleEventArgs ev)
        {
            if (!_betterRoles.IsEnabled || ev.Player.IsHost || ev.Player.IsNPC || !ev.IsAllowed)
            {
                return;
            }

            if (ev.NewRole == RoleTypeId.Scp079)
            {
                Timing.RunCoroutine(_ChangeLostValue(ev.Player, _betterRoles.Scp079Ghostlight));

                return;
            }

            if (ev.NewRole == RoleTypeId.Scp3114)
            {
                Timing.CallDelayed(0.005f, delegate ()
                {
                    ev.Player.MaxHealth = 1250;
                    ev.Player.Health = 1250;

                    var transform = Door.Get(DoorType.Scp173Gate).GameObject.transform;

                    var position = transform.position;
                    var rotation = transform.rotation;

                    var direction = rotation * Vector3.back;

                    ev.Player.Teleport(position + Vector3.up * 2 + direction * 4);
                });

                return;
            }

            Team team = RoleExtensions.GetTeam(ev.NewRole);

            if (team is not Team.FoundationForces or Team.ChaosInsurgency || ev.NewRole != RoleTypeId.NtfPrivate && team == Team.FoundationForces)
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

        public void OnEnteringPocketDimension(EnteringPocketDimensionEventArgs ev)
        {
            if (!ev.IsAllowed || ev.Player == null)
            {
                return;
            }

            if (Warhead.IsDetonated)
            {
                ev.IsAllowed = false;
                ev.Player.Kill(DamageType.Scp106);
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
                || ev.Player.IsScp
                || ev.Player.IsHost
                || ev.Player.IsNPC)
            {
                return;
            }

            Firearm weapon = ev.Player.CurrentItem.As<Firearm>();

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
            if (!ev.IsAllowed || ev.Attacker == null || ev.Attacker.IsNPC || ev.Attacker.IsHost || ev.Player.IsHost || ev.Player.IsNPC || ev.Player.UserId == ev.Attacker.UserId || ev.Player.LeadingTeam == ev.Attacker.LeadingTeam || ev.DamageHandler.Type is DamageType.Explosion or DamageType.Scp018)
            {
                return;
            }

            var isHuman = ev.DamageHandler.Type.IsHumanDamage() || ev.Attacker.CurrentItem != null && ev.Attacker.CurrentItem.IsWeapon && ev.DamageHandler.Type != DamageType.Explosion;

            if (ev.DamageHandler.Type is not DamageType.PocketDimension and not DamageType.Poison and not DamageType.Bleeding && (isHuman || ev.Attacker.IsScp) && ev.Amount > 0)
            {
                if (ev.Player.Role.Is<Exiled.API.Features.Roles.Scp3114Role>(out var role) && RoleExtensions.GetTeam(role.StolenRole).GetLeadingTeam() == ev.Attacker.LeadingTeam)
                {
                    return;
                }

                if (ev.Player.Health - ev.Amount <= 0)
                {
                    ev.Attacker.ShowHint($"<line-height=95%><voffset=5em><size=90%><color=#E55807>Убит!</color></size></voffset>", 1);
                }
                else
                {
                    ev.Attacker.ShowHint($"<line-height=95%><voffset=5em><size=90%><color=#E55807>{Mathf.RoundToInt(ev.Amount)}</color></size></voffset>", 1);
                }
            }

            if (!ev.Player.IsHuman || (ev.Player.CurrentArmor?.Type ?? ItemType.None) == ItemType.ArmorHeavy || !_realisticEffects.IsEnabled || ev.Player.Health - ev.Amount <= 0)
            {
                return;
            }

            if (isHuman)
            {
                ev.Player.EnableEffect(EffectType.Bleeding, _realisticEffects.BleedingShot, _realisticEffects.AddDuration);
            }
            else if (ev.DamageHandler.Type == DamageType.Scp0492)
            {
                ev.Player.Stamina -= 0.03f;

                if (Random.Range(0, 101) >= 98)
                {
                    ev.Player.DropHeldItem();
                }

                return;
            }
            else if (ev.DamageHandler.Type == DamageType.Scp939)
            {
                ev.Player.EnableEffect(EffectType.Bleeding, _realisticEffects.Bleeding939, _realisticEffects.AddDuration);

                return;
            }
        }
        #endregion
        #region ZombieInfection
        public void OnDying(DyingEventArgs ev)
        {
            if (!_zombieInfection.IsEnabled
                || !ev.Player.IsHuman || !ev.IsAllowed || ev.Player.IsHost || ev.Player.IsNPC)
            {
                return;
            }

            if (ev.DamageHandler.Type != DamageType.Poison && (ev.Attacker == null || ev.Attacker.Role.Type != RoleTypeId.Scp0492) || Random.Range(0, 101) < 88)
            {
                return;
            }

            ev.IsAllowed = false;

            ev.Player.DropAllWithoutKeycard();

            ev.Player.Role.Set(RoleTypeId.Scp0492, SpawnReason.Revived, RoleSpawnFlags.None);
        }
        #endregion
        #region Others
        public void OnTriggeringTesla(TriggeringTeslaEventArgs ev)
        {
            if (!ev.IsAllowed || ev.Player.IsHost || ev.Player.IsNPC || !Warhead.IsInProgress && ev.Player.Role.Team != Team.FoundationForces && ev.Player.Role.Type != RoleTypeId.Scp079 && !ev.Tesla.Room.AreLightsOff)
            {
                return;
            }

            ev.IsAllowed = false;
            ev.IsInIdleRange = false;
            ev.IsInHurtingRange = false;
            ev.IsTriggerable = false;
        }

        public void OnUsingRadioBattery(UsingRadioBatteryEventArgs ev)
        {
            if (ev.Player.IsHost || ev.Player.IsNPC)
            {
                return;
            }

            ev.Drain *= 0.05f;
        }

        public void OnUsedItem(UsedItemEventArgs ev)
        {
            if (ev.Item.Type == ItemType.Painkillers)
            {
                ev.Player.GetEffect(EffectType.DamageReduction).ServerSetState(20, 180, true);
            }
        }

        public void OnVerified(VerifiedEventArgs ev)
        {
            if (ev.Player == null || ev.Player.IsHost || ev.Player.IsNPC || ev.Player.AuthenticationType is not AuthenticationType.Steam and not AuthenticationType.Discord)
            {
                return;
            }

            MySQL.API.Models.Sync sync = ev.Player.AuthenticationType switch
            {
                AuthenticationType.Steam => MySqlManager.Sync.Select(ev.Player.UserId),
                AuthenticationType.Discord => MySqlManager.Sync.SelectByDiscord(ev.Player.UserId),
                _ => null
            };

            if (sync != null)
            {
                sync.InGame = true;

                if (!ev.Player.RemoteAdminAccess || ev.Player.Group == null || ev.Player.Group.BadgeColor == "none")
                {
                    ev.Player.Group = _discordGroup ??= ServerStatic.GetPermissionsHandler()?.GetGroup("discord");
                }

                MySqlManager.Sync.Update(sync);
            }
        }

        public void OnDestroying(DestroyingEventArgs ev)
        {
            if (ev.Player == null || ev.Player.IsHost || ev.Player.IsNPC || ev.Player.AuthenticationType is not AuthenticationType.Steam and not AuthenticationType.Discord)
            {
                return;
            }

            MySQL.API.Models.Sync sync = ev.Player.AuthenticationType switch
            {
                AuthenticationType.Steam => MySqlManager.Sync.Select(ev.Player.UserId),
                AuthenticationType.Discord => MySqlManager.Sync.SelectByDiscord(ev.Player.UserId),
                _ => null
            };

            if (sync != null)
            {
                sync.InGame = false;

                MySqlManager.Sync.Update(sync);
            }
        }

        #endregion
        #region Coroutines
        private IEnumerator<float> _ChangeLostValue(Player player, float value)
        {
            yield return Timing.WaitForSeconds(2f);

            var scp = player.Role.Base as Scp079Role;

            if (scp == null || player.IsHost || player.IsNPC || !scp.SubroutineModule.TryGetSubroutine(out Scp079LostSignalHandler lost))
            {
                yield break;
            }

            lost._ghostlightLockoutDuration = value;
        }
        #endregion
    }
}
