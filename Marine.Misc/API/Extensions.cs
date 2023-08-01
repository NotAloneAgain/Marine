using CustomPlayerEffects;
using Exiled.API.Features;
using Interactables.Interobjects.DoorUtils;
using InventorySystem.Items.Keycards;
using System.Linq;
using UnityEngine;

namespace Marine.Misc.API
{
    internal static class Extensions
    {
        public static bool CheckPermissions(this Player player, KeycardPermissions perms)
        {
            for (var i = 0; i < player.Items.Distinct().Count(); i++)
            {
                var item = player.Items.Select(x => x.Base).ElementAt(i);

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

            foreach (var item in player.Items)
            {
                if (item.IsKeycard)
                {
                    continue;
                }

                player.DropItem(item);
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
                Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);

                Renderer[] childRenderers = gameObject.GetComponentsInChildren<Renderer>();

                foreach (Renderer childRenderer in childRenderers)
                {
                    bounds.Encapsulate(childRenderer.bounds);
                }

                return bounds;
            }
        }

        public static bool HasEffect<TEffect>(this Player player) where TEffect : StatusEffectBase => player.TryGetEffect<TEffect>(out var effect) && effect.IsEnabled;

        public static bool HasFlagFast(this KeycardPermissions perm, KeycardPermissions perm2) => (perm & perm2) == perm2;
    }
}
