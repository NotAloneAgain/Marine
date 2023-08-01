using Exiled.API.Features;
using Exiled.API.Features.Pickups;
using Marine.Misc.API;
using MEC;
using System.Collections.Generic;
using System.Linq;

namespace Marine.Misc.Handlers
{
    internal sealed class ServerHandlers
    {
        #region Initialize
        private CoroutineHandle[] _coroutines;

        internal ServerHandlers()
        {
            _coroutines = new CoroutineHandle[2];
        }
        #endregion
        #region Handlers
        public void OnRoundStarted()
        {
            _coroutines[0] = Timing.RunCoroutine(_CleanupItems());
            _coroutines[1] = Timing.RunCoroutine(_CleanupRagdolls());
        }

        public void OnRestartingRound()
        {
            Timing.KillCoroutines(_coroutines);
        }
        #endregion
        #region Coroutines
        public IEnumerator<float> _CleanupItems()
        {
            List<Pickup> toClear = new(500);

            while (true)
            {
                foreach (var item in Pickup.List)
                {
                    if (!item.IsSpawned || Map.Lockers.Any(locker => locker.gameObject.FindBounds().Contains(item.Position)))
                    {
                        continue;
                    }

                    if (!toClear.Contains(item))
                    {
                        toClear.Add(item);

                        continue;
                    }

                    item.Destroy();
                }

                yield return Timing.WaitForSeconds(300);
            }
        }

        public IEnumerator<float> _CleanupRagdolls()
        {
            List<Ragdoll> toClear = new(100);

            while (true)
            {
                foreach (var ragdoll in Ragdoll.List)
                {
                    if (!toClear.Contains(ragdoll) && (ragdoll.IsExpired || ragdoll.IsConsumed) && ragdoll.CanBeCleanedUp && ragdoll.AllowCleanUp)
                    {
                        toClear.Add(ragdoll);

                        continue;
                    }

                    ragdoll.Destroy();
                }

                yield return Timing.WaitForSeconds(120);
            }
        }
        #endregion
    }
}
