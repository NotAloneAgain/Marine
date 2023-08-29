using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Pickups;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Server;
using Marine.Misc.API;
using MEC;
using System.Collections.Generic;
using UnityEngine;

namespace Marine.Misc.Handlers
{
    internal sealed class ServerHandlers
    {
        #region Initialize
        private CoroutineHandle[] _coroutines;

        internal ServerHandlers()
        {
            _coroutines = new CoroutineHandle[4];
        }
        #endregion
        #region Handlers
        public void OnRoundStarted()
        {
            _coroutines[0] = Timing.RunCoroutine(_CleanupItems());
            _coroutines[1] = Timing.RunCoroutine(_CleanupRagdolls());
            _coroutines[2] = Timing.RunCoroutine(_RandomBlackout());
            _coroutines[3] = Timing.RunCoroutine(_RandomLockdown());
            Server.FriendlyFire = false;
        }

        public void OnRestartingRound()
        {
            Timing.KillCoroutines(_coroutines);
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
                    Map.TurnOffAllLights(Random.Range(10, 240));
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
                        door.Lock(duration, Exiled.API.Enums.DoorLockType.Isolation);
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
                foreach (var item in Pickup.List)
                {
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

                yield return Timing.WaitForSeconds(300);
            }
        }

        public IEnumerator<float> _CleanupRagdolls()
        {
            HashSet<Ragdoll> toClear = new(100);
            HashSet<Ragdoll> recalling = new(20);

            while (Round.InProgress)
            {
                foreach (var player in Player.List)
                {
                    var role = player.Role.As<Scp049Role>();

                    if (role != null)
                    {
                        continue;
                    }

                    recalling.Add(role.RecallingRagdoll);
                }

                foreach (var ragdoll in Ragdoll.List)
                {
                    if (recalling.Contains(ragdoll))
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

                yield return Timing.WaitForSeconds(120);
            }
        }
        #endregion
    }
}
