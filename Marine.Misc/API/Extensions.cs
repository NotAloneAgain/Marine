using CustomPlayerEffects;
using Exiled.API.Features;
using Exiled.API.Features.Pickups;
using Interactables.Interobjects.DoorUtils;
using InventorySystem.Items.Keycards;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Marine.Misc.API
{
    public static class Extensions
    {
        public static bool CheckPermissions(this Player player, KeycardPermissions perms)
        {
            for (var i = 0; i < player.Items.Distinct().Count(); i++)
            {
                InventorySystem.Items.ItemBase item = player.Items.Select(x => x.Base).ElementAt(i);

                if (item.Category != ItemCategory.Keycard || item is not KeycardItem card || !card.Permissions.HasFlagFast(perms))
                {
                    continue;
                }

                return true;
            }

            return false;
        }

        public static void DropAllWithoutKeycard(this Player player)
        {
            if (player.IsInventoryEmpty)
            {
                return;
            }

            var items = player.Items.ToList();

            foreach (Exiled.API.Features.Items.Item item in items)
            {
                if (item.IsKeycard)
                {
                    continue;
                }

                _ = player.DropItem(item);
            }
        }

        public static Bounds FindBounds(this GameObject gameObject)
        {
            Renderer renderer = gameObject.GetComponent<Renderer>();

            if (renderer != null)
            {
                return renderer.bounds;
            }
            else
            {
                var bounds = new Bounds(Vector3.zero, Vector3.zero);

                Renderer[] childRenderers = gameObject.GetComponentsInChildren<Renderer>();

                foreach (Renderer childRenderer in childRenderers)
                {
                    bounds.Encapsulate(childRenderer.bounds);
                }

                return bounds;
            }
        }

        public static bool IsHumanDamage(this Exiled.API.Enums.DamageType type)
        {
            return type switch
            {
                Exiled.API.Enums.DamageType.Firearm or Exiled.API.Enums.DamageType.Revolver or Exiled.API.Enums.DamageType.Crossvec or
                Exiled.API.Enums.DamageType.AK or Exiled.API.Enums.DamageType.E11Sr or Exiled.API.Enums.DamageType.Fsp9 or
                Exiled.API.Enums.DamageType.Logicer or Exiled.API.Enums.DamageType.Shotgun or Exiled.API.Enums.DamageType.Com45 or
                Exiled.API.Enums.DamageType.Com18 or Exiled.API.Enums.DamageType.A7 or Exiled.API.Enums.DamageType.Fsp9 => true,
                _ => false
            };
        }

        public static bool HasEffect<TEffect>(this Player player) where TEffect : StatusEffectBase
        {
            return player.TryGetEffect<TEffect>(out TEffect effect) && effect.IsEnabled;
        }

        public static bool HasFlagFast(this KeycardPermissions perm, KeycardPermissions perm2)
        {
            return (perm & perm2) == perm2;
        }

        public static bool IsCloseToChambers(this Vector3 pos)
        {
            return Map.Lockers.Select(locker => locker.Chambers).Any(chambers => chambers.Any(chamber => chamber.gameObject.FindBounds().Contains(pos)));
        }

        public static bool InClosedLcz(this Pickup pickup)
        {
            return pickup.Room.Zone == Exiled.API.Enums.ZoneType.LightContainment && Map.IsLczDecontaminated;
        }

        public static bool InDetonatedComplex(this Pickup pickup)
        {
            return pickup.Room.Zone != Exiled.API.Enums.ZoneType.Surface && Warhead.IsDetonated;
        }

        public static void AddLog(this string str)
        {
            var path = Path.Combine(Paths.Exiled, "Logs", "Debug.txt");

            var print = $"{(File.Exists(path) ? "\n" : string.Empty)}[{DateTime.Now:G}] [{Assembly.GetCallingAssembly().FullName}] {str}";

            File.AppendAllText(path, print);
        }
    }
}
