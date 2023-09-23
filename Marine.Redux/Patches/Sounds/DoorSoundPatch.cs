using CustomPlayerEffects;
using Exiled.API.Features;
using HarmonyLib;
using Interactables.Interobjects;
using Interactables.Interobjects.DoorUtils;
using Marine.Misc.API;
using Marine.Redux.API.Subclasses;
using Marine.Redux.Subclasses.ClassD.Single;
using Mirror;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp939.Ripples;
using System;
using System.Linq;
using UnityEngine;

namespace Marine.Redux.Patches.Sounds
{
    [HarmonyPatch(typeof(DoorRippleTrigger), nameof(DoorRippleTrigger.OnDoorAction))]
    internal static class DoorSoundPatch
    {
        private static bool Prefix(DoorRippleTrigger __instance, DoorVariant dv, DoorAction da, ReferenceHub hub)
        {
            try
            {
                if (__instance == null || da is DoorAction.Opened or DoorAction.Closed || !NetworkServer.active)
                {
                    return false;
                }

                BasicDoor basicDoor = dv as BasicDoor;

                if (basicDoor == null)
                {
                    return false;
                }

                if (hub == null || hub.gameObject == null || !Player.Dictionary.TryGetValue(hub.gameObject, out var player))
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

                var magnitude = (dv.transform.position + DoorRippleTrigger.PosOffset - __instance.ScpRole.FpcModule.Position).sqrMagnitude;
                var maxDistance = basicDoor.MainSource.maxDistance * basicDoor.MainSource.maxDistance;
                Vector3 pos = __instance.ScpRole.FpcModule.Position;
                Vector3 humanPos = role.FpcModule.Position;

                if (magnitude > maxDistance || __instance.CheckVisibility(player.ReferenceHub) || player.Role.Type == RoleTypeId.Tutorial)
                {
                    return false;
                }

                if (Subclass.HasAny(player))
                {
                    var subclass = Subclass.ReadOnlyCollection.FirstOrDefault(sub => sub.Has(player));

                    if (subclass == null || subclass is Scp343)
                    {
                        return false;
                    }
                }

                if (__instance._rippleAssigned)
                {
                    __instance._surfaceRippleTrigger.ProcessRipple(hub);
                    return false;
                }

                __instance.Player.Play(dv.transform.position + DoorRippleTrigger.PosOffset, Color.red);

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
