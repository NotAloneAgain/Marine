using Exiled.API.Features;
using UnityEngine;

namespace Marine.Commands.API
{
    internal static class Extensions
    {
        public static Player GetFromView(this Player owner)
        {
            if (!Physics.Raycast(owner.Position, owner.Transform.forward, out var hit, 5))
            {
                return null;
            }

            Player target = Player.Get(hit.transform.GetComponentInParent<ReferenceHub>());

            return target;
        }

        public static string GetSecondsString(this double seconds) => Mathf.RoundToInt((float)seconds).GetSecondsString();

        public static string GetSecondsString(this int seconds)
        {
            int secondsInt = (int)seconds;

            string secondsString = secondsInt switch
            {
                int n when n % 100 >= 11 && n % 100 <= 14 => "секунд",
                int n when n % 10 == 1 => "секунда",
                int n when n % 10 >= 2 && n % 10 <= 4 => "секунды",
                _ => "секунд"
            };

            return $"{secondsInt} {secondsString}";
        }
    }
}
