﻿using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.API.Features.Pickups;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Scp914;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp079;
using System.Linq;

namespace Marine.Misc.Handlers
{
    internal sealed class MapHandlers
    {
        public void OnGenerated()
        {
            var door = Door.Get(DoorType.NukeSurface).As<BreakableDoor>();

            door.IgnoredDamage |= Interactables.Interobjects.DoorUtils.DoorDamageType.Scp096;
            door.IgnoredDamage |= Interactables.Interobjects.DoorUtils.DoorDamageType.Grenade;

            door = Door.Get(DoorType.HID).As<BreakableDoor>();

            door.IgnoredDamage |= Interactables.Interobjects.DoorUtils.DoorDamageType.Grenade;
        }

        public void OnPlacingBulletHole(PlacingBulletHoleEventArgs ev) => ev.IsAllowed = false;

        public void OnGeneratorActivated(GeneratorActivatingEventArgs ev)
        {
            var computers = Player.Get(RoleTypeId.Scp079);

            if (!ev.IsAllowed || computers.Count() == 0)
            {
                return;
            }

            int generators = Generator.List.Count(x => x.IsEngaged) + 1;
            int xp = 50 * generators;

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

                lost.ServerLoseSignal(tier.AccessTierLevel switch
                {
                    5 => 10,
                    4 => 8,
                    3 => 6,
                    2 => 5,
                    _ => 3
                });
            }
        }

        public void OnUpgradingPickup(UpgradingPickupEventArgs ev)
        {
            if (ev.Pickup.Type == ItemType.MicroHID && ev.KnobSetting == Scp914.Scp914KnobSetting.Coarse)
            {
                ev.IsAllowed = false;

                Pickup.CreateAndSpawn(ItemType.Jailbird, ev.OutputPosition, ev.Pickup.Rotation, ev.Pickup.PreviousOwner);

                ev.Pickup.Destroy();
            }

            if (ev.Pickup.Type is ItemType.ParticleDisruptor or ItemType.Jailbird)
            {
                ev.IsAllowed = false;

                ev.Pickup.Destroy();
            }
        }

        public void OnUpgradingInventoryItem(UpgradingInventoryItemEventArgs ev)
        {
            if (ev.Item.Type == ItemType.MicroHID && ev.KnobSetting == Scp914.Scp914KnobSetting.Coarse)
            {
                ev.IsAllowed = false;

                ev.Item.Destroy();

                ev.Player.AddItem(ItemType.Jailbird);
            }

            if (ev.Item.Type is ItemType.ParticleDisruptor or ItemType.Jailbird)
            {
                ev.IsAllowed = false;

                ev.Item.Destroy();
            }
        }
    }
}
