using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Interactables.Interobjects.DoorUtils;
using System.Collections.Generic;
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
            if (!Physics.Raycast(owner.Position, owner.Transform.forward, out RaycastHit hit, distance))
            {
                return null;
            }

            if (!hit.transform.TryGetComponent<BasicRagdoll>(out var basic) && (basic = Ragdoll.List.FirstOrDefault(r => r.GameObject == hit.transform.gameObject)?.Base) == null)
            {
                return Ragdoll.List.Select(r => (Ragdoll: r, Distance: Vector3.Distance(r.Position, owner.Position))).OrderBy(pair => pair.Distance).FirstOrDefault().Ragdoll;
            }

            var ragdoll = Ragdoll.Get(basic);

            return ragdoll;
        }

        public static string GetSecondsString(this double seconds) => Mathf.RoundToInt((float)seconds).GetSecondsString();

        public static bool IsDonator(this string group) => !string.IsNullOrEmpty(group) && group != "don4" && (group.Contains("don") || group.Contains("cons"));

        public static string GetNameByGroup(this UserGroup group)
        {
            PermissionsHandler handler = ServerStatic.GetPermissionsHandler();

            Dictionary<string, UserGroup> groups = handler.GetAllGroups();

            KeyValuePair<string, UserGroup>? pair = null!;

            foreach (KeyValuePair<string, UserGroup> gru in groups)
            {
                (var key, UserGroup g) = (gru.Key, gru.Value);

                if (g.Permissions == group.Permissions && g.RequiredKickPower == group.RequiredKickPower && g.KickPower == group.KickPower && g.BadgeText == group.BadgeText && g.BadgeColor == group.BadgeColor)
                {
                    pair = gru;

                    break;
                }
            }

            return pair is null ? string.Empty : pair.Value.Key;
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
