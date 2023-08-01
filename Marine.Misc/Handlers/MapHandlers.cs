using Exiled.API.Enums;
using Exiled.API.Features;

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
    }
}
