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
using UnityEngine;

#pragma warning disable IDE0060

namespace Marine.Misc.Handlers
{
    internal sealed class ServerHandlers
    {
        #region Initialize
        private List<ZoneType> _zones;
        private List<CoroutineHandle> _coroutines;

        internal ServerHandlers()
        {
            _zones = new List<ZoneType>(4)
            {
                ZoneType.Surface,
                ZoneType.Entrance,
                ZoneType.LightContainment,
                ZoneType.HeavyContainment,
            };

            _coroutines = new (10);
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

            Timing.CallDelayed(5, AudioExtensions.StopAudio);
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

                    var zonesCount = Random.Range(1, 5);

                    HashSet<ZoneType> zones = zonesCount switch
                    {
                        5 => new(1),
                        _ => new(zonesCount)
                    };

                    for (int i = 0; i >= zonesCount; i++)
                    {
                        zones.Add(_zones[Random.Range(0, _zones.Count)]);
                    }

                    Map.TurnOffAllLights(duration, zones);
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

                    foreach (var door in Door.List)
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

            while (Round.InProgress)
            {
                yield return Timing.WaitForSeconds(300);

                foreach (var item in Pickup.List)
                {
                    if (item == null)
                    {
                        if (toClear.Contains(item))
                        {
                            toClear.Remove(item);
                        }

                        continue;
                    }

                    if (!item.IsSpawned || item.Room == null)
                    {
                        continue;
                    }

                    if (item.InClosedLcz() || item.InDetonatedComplex())
                    {
                        if (toClear.Contains(item))
                        {
                            toClear.Remove(item);
                        }

                        item.Destroy();

                        continue;
                    }

                    if (item.Position.IsCloseToChambers() || item.Type.GetCategory() is ItemCategory.MicroHID or ItemCategory.SCPItem || item.Type == ItemType.KeycardO5)
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
            }
        }

        public IEnumerator<float> _CleanupRagdolls()
        {
            HashSet<Ragdoll> toClear = new(100);
            HashSet<Ragdoll> recalling = new(25);

            while (Round.InProgress)
            {
                yield return Timing.WaitForSeconds(120);

                foreach (var player in Player.List)
                {
                    if (player.Role.Type != RoleTypeId.Scp049)
                    {
                        continue;
                    }

                    var role = player.Role.As<Scp049Role>();

                    if (role != null && role.RecallingRagdoll != null)
                    {
                        continue;
                    }

                    recalling.Add(role.RecallingRagdoll);
                }

                foreach (var ragdoll in Ragdoll.List)
                {
                    if (recalling.Contains(ragdoll) || !ragdoll.Role.IsHuman())
                    {
                        continue;
                    }

                    if (!toClear.Contains(ragdoll) && (ragdoll.IsExpired || ragdoll.IsConsumed) && ragdoll.CanBeCleanedUp && ragdoll.AllowCleanUp)
                    {
                        toClear.Add(ragdoll);

                        continue;
                    }

                    ragdoll.Destroy();
                }

                recalling.Clear();
            }
        }
        #endregion
    }
}
