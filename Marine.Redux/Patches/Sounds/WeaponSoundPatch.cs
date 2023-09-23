using CustomPlayerEffects;
using Exiled.API.Features;
using HarmonyLib;
using InventorySystem.Items.Firearms;
using Marine.Misc.API;
using Marine.Redux.API.Subclasses;
using Mirror;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp939.Ripples;
using RelativePositioning;
using System;
using System.Linq;
using UnityEngine;

#pragma warning disable IDE0060

namespace Marine.Redux.Patches.Sounds
{
    [HarmonyPatch(typeof(FirearmRippleTrigger), nameof(FirearmRippleTrigger.OnServerSoundPlayed))]
    internal static class WeaponSoundPatch
    {
        private static bool Prefix(FirearmRippleTrigger __instance, Firearm firearm, byte audioId, float dis)
        {
            try
            {
                if (__instance == null || firearm == null || !NetworkServer.active)
                {
                    return false;
                }

                if (firearm.Owner == null || firearm.Owner.gameObject == null || !Player.Dictionary.TryGetValue(firearm.Owner.gameObject, out Player player))
                {
                    return false;
                }

                if (player.Role == null || player.Role.Team is Team.Dead or Team.OtherAlive or Team.SCPs)
                {
                    return false;
                }

                var role = player.Role.Base as HumanRole;

                if (player.HasEffect<Invisible>() || role == null)
                {
                    return false;
                }

                Vector3 pos = __instance.ScpRole.FpcModule.Position;
                Vector3 humanPos = role.FpcModule.Position;

                if ((pos - humanPos).sqrMagnitude > dis * dis || __instance.CheckVisibility(player.ReferenceHub) || player.Role.Type == RoleTypeId.Tutorial)
                {
                    return false;
                }

                if (Subclass.HasAny(player))
                {
                    Subclass subclass = Subclass.ReadOnlyCollection.FirstOrDefault(sub => sub.Has(player));

                    if (subclass == null || !subclass.CanSoundFootstep)
                    {
                        return false;
                    }
                }

                __instance._syncRipplePos = new RelativePosition(role.FpcModule.Position);
                __instance._syncRoleColor = role.RoleTypeId;
                __instance._syncPlayer = firearm.Owner;
                __instance.ServerSendRpcToObservers();

                return false;
            }
            catch (Exception err)
            {
                Log.Error($"[FootstepSoundPatch] Error occured: {err}");

                return true;
            }
        }
    }
}
