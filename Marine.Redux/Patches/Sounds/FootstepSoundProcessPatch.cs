using AudioPooling;
using Exiled.API.Features;
using HarmonyLib;
using Marine.Redux.API.Subclasses;
using Mirror;
using PlayerRoles;
using PlayerRoles.FirstPersonControl.Thirdperson;
using PlayerRoles.PlayableScps.Scp939.Ripples;
using RelativePositioning;
using System;
using System.Linq;
using Utils.Networking;

namespace Marine.Redux.Patches.Sounds
{
    [HarmonyPatch(typeof(FootstepRippleTrigger), nameof(FootstepRippleTrigger.ClientProcessRpc))]
    internal static class FootstepSoundProcessPatch
    {
        private static bool Prefix(FootstepRippleTrigger __instance, NetworkReader reader)
        {
            try
            {
                if (__instance == null || reader == null || !NetworkServer.active)
                {
                    return false;
                }

                if (!reader.TryReadReferenceHub(out __instance._syncPlayer))
                {
                    return false;
                }

                if (__instance._syncPlayer == null || __instance._syncPlayer.gameObject == null || !Player.Dictionary.TryGetValue(__instance._syncPlayer.gameObject, out var player))
                {
                    return false;
                }

                if (player.Role == null || player.Role.Team is Team.Dead or Team.OtherAlive or Team.SCPs)
                {
                    return false;
                }

                var role = player.Role.Base as HumanRole;

                if (role == null || player.Role.Type == RoleTypeId.Tutorial)
                {
                    return false;
                }

                AnimatedCharacterModel model = role.FpcModule.CharacterModelInstance as AnimatedCharacterModel;

                if (model == null)
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

                __instance._syncPos = reader.ReadRelativePosition();
                __instance._syncDistance = reader.ReadByte();
                __instance.Player.Play(__instance._syncPos.Position, role.RoleColor);
                AudioSourcePoolManager.PlaySound(model.RandomFootstep, __instance._syncPos.Position, (float)__instance._syncDistance, 1, FalloffType.Exponential, AudioMixerChannelType.DefaultSfx, 1, false);
                __instance.OnPlayedRipple(__instance._syncPlayer);

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
