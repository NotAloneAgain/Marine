using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Interactables.Interobjects.DoorUtils;
using System.Linq;
using UnityEngine;

namespace Marine.Commands.API
{
    internal static class Extensions
    {
        public static Player GetFromView(this Player owner, float distance)
        {
            if (!Physics.Raycast(owner.Position, owner.Transform.forward, out RaycastHit hit, distance))
            {
                return null;
            }

            var target = Player.Get(hit.transform.GetComponentInParent<ReferenceHub>());

            return target;
        }

        public static Door GetDoorFromView(this Player owner, float distance)
        {
            if (!Physics.Raycast(owner.Position, owner.Transform.forward, out RaycastHit hit, distance))
            {
                return null;
            }

            var door = Door.Get(hit.transform.GetComponentInParent<DoorVariant>());

            return door;
        }

        public static Ragdoll GetRagdollFromView(this Player owner, float distance)
        {
            if (!Physics.Raycast(owner.Position, owner.Transform.forward, out RaycastHit hit, distance) || !hit.transform.TryGetComponent<BasicRagdoll>(out var basic) && (basic = Ragdoll.List.FirstOrDefault(r => r.GameObject == hit.transform.gameObject).Base) == null)
            {
                return null;
            }

            var ragdoll = Ragdoll.Get(basic);

            return ragdoll;
        }

        public static string GetSecondsString(this double seconds)
        {
            return Mathf.RoundToInt((float)seconds).GetSecondsString();
        }

        public static string GetSecondsString(this int seconds)
        {
            var secondsInt = seconds;

            var secondsString = secondsInt switch
            {
                int n when n % 100 is >= 11 and <= 14 => "секунд",
                int n when n % 10 == 1 => "секунда",
                int n when n % 10 is >= 2 and <= 4 => "секунды",
                _ => "секунд"
            };

            return $"{secondsInt} {secondsString}";
        }
    }
}
