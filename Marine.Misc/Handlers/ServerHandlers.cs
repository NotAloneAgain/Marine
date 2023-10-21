﻿using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.API.Features.Pickups;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Server;
using Marine.Misc.API;
using MEC;
using PlayerRoles;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#pragma warning disable IDE0060

namespace Marine.Misc.Handlers
{
    internal sealed class ServerHandlers
    {
        #region Initialize
        private readonly List<CoroutineHandle> _coroutines;

        internal ServerHandlers()
        {
            _coroutines = new(10);
        }
        #endregion
        #region Handlers
        public void OnRoundStarted()
        {
            _coroutines.Clear();

            _coroutines.Add(Timing.RunCoroutine(_CleanupItems()));
            _coroutines.Add(Timing.RunCoroutine(_CleanupRagdolls()));
            _coroutines.Add(Timing.RunCoroutine(_RandomBlackout()));
            _coroutines.Add(Timing.RunCoroutine(_RandomLockdown()));

            Server.FriendlyFire = false;
        }

        public void OnRestartingRound()
        {
            Timing.KillCoroutines(_coroutines.ToArray());

            Server.FriendlyFire = false;
        }

        public void OnEndedRound(RoundEndedEventArgs ev)
        {
            Server.FriendlyFire = true;

            _ = Timing.CallDelayed(5, AudioExtensions.StopAudio);
        }
        #endregion
        #region Coroutines
        public IEnumerator<float> _RandomBlackout()
        {
            while (Round.InProgress)
            {
                yield return Timing.WaitForSeconds(360);

                if (Random.Range(0, 100) >= 86)
                {
                    var duration = Random.Range(10, 240);

                    ZoneType zone = Random.Range(0, 101) switch
                    {
                        100 => ZoneType.Unspecified,
                        >= 74 => ZoneType.LightContainment,
                        >= 49 => ZoneType.HeavyContainment,
                        >= 24 => ZoneType.Entrance,
                        _ => ZoneType.Surface,
                    };

                    Map.TurnOffAllLights(duration, zone);
                }
            }
        }

        public IEnumerator<float> _RandomLockdown()
        {
            while (Round.InProgress)
            {
                yield return Timing.WaitForSeconds(360);

                if (Random.Range(0, 100) >= 94)
                {
                    var duration = Random.Range(5, 20);

                    foreach (Door door in Door.List)
                    {
                        door.IsOpen = false;
                        door.Lock(duration, DoorLockType.Isolation);
                        door.Room.Blackout(0.5f);
                    }
                }
            }
        }

        public IEnumerator<float> _CleanupItems()
        {
            List<Pickup> toClear = new(500);

            while (Round.ElapsedTime.TotalMinutes <= 8)
            {
                foreach (Pickup item in Pickup.List)
                {
                    if (item == null)
                    {
                        continue;
                    }

                    if (item.Type.IsAmmo())
                    {
                        item.Destroy();

                        continue;
                    }

                    if (!item.IsSpawned || item.Room == null || item.Room.Type == RoomType.Hcz079)
                    {
                        continue;
                    }

                    if (item.InDetonatedComplex())
                    {
                        if (toClear.Contains(item))
                        {
                            toClear.Remove(item);
                        }

                        item.Destroy();

                        continue;
                    }

                    if (item.Position.IsCloseToChambers() || item.Type.GetCategory() is ItemCategory.MicroHID or ItemCategory.SCPItem or ItemCategory.Keycard)
                    {
                        continue;
                    }

                    if (!toClear.Contains(item))
                    {
                        toClear.Add(item);

                        continue;
                    }

                    toClear.Remove(item);

                    item.Destroy();
                }

                yield return Timing.WaitForSeconds(120);
            }

            toClear.Clear();
            toClear = null;

            while (Round.InProgress)
            {
                yield return Timing.WaitForSeconds(60);

                foreach (Pickup item in Pickup.List)
                {
                    if (item == null)
                    {
                        continue;
                    }

                    if (!item.IsSpawned)
                    {
                        continue;
                    }

                    if (item.InClosedLcz() || item.InDetonatedComplex())
                    {
                        item.Destroy();

                        continue;
                    }

                    if (item.Type.IsScp() || item.Type is ItemType.MicroHID or ItemType.Jailbird || item.Type == ItemType.KeycardO5)
                    {
                        continue;
                    }

                    item.Destroy();
                }
            }

            foreach (Pickup item in Pickup.List)
            {
                if (item == null)
                {
                    continue;
                }

                item.Destroy();
            }
        }

        public IEnumerator<float> _CleanupRagdolls()
        {
            HashSet<Ragdoll> toClear = new(100);

            while (Round.InProgress)
            {
                yield return Timing.WaitForSeconds(120);

                foreach (Ragdoll ragdoll in Ragdoll.List)
                {
                    if (ragdoll == null || Player.List.Any(ply => Vector3.Distance(ragdoll.Position, ply.Position) <= 4) || !ragdoll.Role.IsHuman())
                    {
                        continue;
                    }

                    bool hasDoctor = Player.List.Any(ply => ply.Role.Type == RoleTypeId.Scp049);
                    bool hasZombie = Player.List.Any(ply => ply.Role.Type == RoleTypeId.Scp0492);

                    if (!toClear.Contains(ragdoll) && (hasDoctor && ragdoll.IsExpired || hasZombie && ragdoll.IsConsumed) && ragdoll.CanBeCleanedUp && ragdoll.AllowCleanUp)
                    {
                        toClear.Add(ragdoll);

                        continue;
                    }

                    toClear.Remove(ragdoll);
                    ragdoll.Destroy();
                }
            }

            toClear.Clear();
            toClear = null;

            foreach (Ragdoll ragdoll in Ragdoll.List)
            {
                if (ragdoll == null)
                {
                    continue;
                }

                ragdoll.Destroy();
            }
        }
        #endregion
    }
}
