using CustomPlayerEffects;
using Exiled.API.Features;
using HarmonyLib;
using Marine.Misc.API;
using Marine.Redux.API.Subclasses;
using Mirror;
using PlayerRoles;
using PlayerRoles.FirstPersonControl.Thirdperson;
using PlayerRoles.PlayableScps.Scp939.Ripples;
using RelativePositioning;
using System;
using System.Linq;
using UnityEngine;

namespace Marine.Redux.Patches.Sounds
{
    [HarmonyPatch(typeof(FootstepRippleTrigger), nameof(FootstepRippleTrigger.OnFootstepPlayed))]
    internal static class FootstepSoundPatch
    {
        private static bool Prefix(FootstepRippleTrigger __instance, AnimatedCharacterModel model, float dis)
        {
            try
            {
                if (__instance == null || model == null || !NetworkServer.active)
                {
                    return false;
                }

                if (model.OwnerHub == null || model.OwnerHub.gameObject == null || !Player.Dictionary.TryGetValue(model.OwnerHub.gameObject, out var player))
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
                    var subclass = Subclass.ReadOnlyCollection.FirstOrDefault(sub => sub.Has(player));

                    if (subclass == null || !subclass.CanSoundFootstep)
                    {
                        return false;
                    }
                }

                __instance._syncPlayer = model.OwnerHub;
                __instance._syncPos = new RelativePosition(humanPos);
                __instance._syncDistance = (byte)Mathf.Min(dis, 255);
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
