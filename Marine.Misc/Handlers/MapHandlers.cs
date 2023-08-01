using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Map;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp079;
using System.Linq;

namespace Marine.Misc.Handlers
{
    internal sealed class MapHandlers
    {
        public void OnGenerated()
        {
            var door = Door.Get(DoorType.NukeSurface);

            door.RequiredPermissions.RequiredPermissions = Interactables.Interobjects.DoorUtils.KeycardPermissions.AlphaWarhead;
            door.IgnoredDamageTypes &= Interactables.Interobjects.DoorUtils.DoorDamageType.Grenade;

            door = Door.Get(DoorType.Scp079First);

            door.RequiredPermissions.RequiredPermissions = Interactables.Interobjects.DoorUtils.KeycardPermissions.ContainmentLevelThree;

            door = Door.Get(DoorType.Scp079Second);

            door.RequiredPermissions.RequiredPermissions = Interactables.Interobjects.DoorUtils.KeycardPermissions.ContainmentLevelThree;

            door = Door.Get(DoorType.Scp079Armory);

            door.RequiredPermissions.RequiredPermissions = Interactables.Interobjects.DoorUtils.KeycardPermissions.ArmoryLevelTwo;

            door = Door.Get(DoorType.HID);

            door.IgnoredDamageTypes &= Interactables.Interobjects.DoorUtils.DoorDamageType.Grenade;
        }

        public void OnGeneratorActivated(GeneratorActivatedEventArgs ev)
        {
            var computers = Player.Get(RoleTypeId.Scp079);

            if (!ev.IsAllowed || computers.Count() == 0)
            {
                return;
            }

            int xp = 50 * (Generator.List.Count(x => x.IsEngaged) + 1);

            foreach (var ply in computers)
            {
                var scp = ply.Role.Base as Scp079Role;

                if (scp == null
                    || !scp.SubroutineModule.TryGetSubroutine<Scp079TierManager>(out var tier)
                    || !scp.SubroutineModule.TryGetSubroutine<Scp079LostSignalHandler>(out var lost))
                {
                    continue;
                }

                tier.ServerGrantExperience(xp, Scp079HudTranslation.ExpGainAdminCommand);
                lost.ServerLoseSignal(5);
            }
        }
    }
}
