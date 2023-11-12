using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp330;
using Marine.Redux.API;
using Marine.Redux.API.Enums;
using Marine.Redux.API.Inventory;
using Marine.Redux.API.Subclasses;
using PlayerRoles;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Marine.Redux.Subclasses.ClassD.Single
{
    public class Scp343 : SingleSubclass
    {
        private RoleTypeId _model = RoleTypeId.Tutorial;

        public override string Name { get; set; } = "SCP-343";

        public override string Desc { get; set; } = "Ты пиздатый, ахуенный, современный, невъебенный";

        public override List<string> Abilities { get; set; } = new List<string>()
        {
            "Нанесение урона кому-либо, а также помощь SCP-Объектам будут наказуемы блокировкой сроком до 12 месяцев.",
            "Преобразование опасных предметов в аптечки.",
            "Выдача безопасных предметов [.item].",
            "Открытие любых дверей кроме опасных.",
            "Смена модельки монеткой.",
            "Телепортация [.tp].",
        };

        public override bool ConsoleRemark { get; } = true;

        public override SpawnInfo SpawnInfo { get; set; } = new()
        {
            ShowInfo = true,
            Message = new("Ты - Бог.\nО своих способностях ты можешь прочитать в консоли.", 15, true, "#009A63"),
            Inventory = new()
            {
                IsRandomable = false,
                Slots = new List<Slot>(8)
                {
                    new Slot(new ItemChances()
                    {
                        { ItemType.Coin, 100 },
                    }, false)
                }
            }
        };

        public override RoleTypeId Role { get; set; } = RoleTypeId.ClassD;

        public override RoleTypeId GameRole { get; set; } = RoleTypeId.Tutorial;

        public override bool CanTriggerTesla { get; set; } = false;

        public override bool CanSoundFootstep { get; set; } = false;

        public override int Chance { get; set; } = 3;

        public override void Subscribe()
        {
            base.Subscribe();

            Exiled.Events.Handlers.Player.Handcuffing += OnHandcuffing;
            Exiled.Events.Handlers.Scp330.EatingScp330 += OnEatingScp330;
            Exiled.Events.Handlers.Player.ChangingRole += OnChangingRole;
            Exiled.Events.Handlers.Player.FlippingCoin += OnFlippingCoin;
            Exiled.Events.Handlers.Player.PickingUpItem += OnPickupingUpItem;
            Exiled.Events.Handlers.Player.InteractingDoor += OnInteractingDoor;
            Exiled.Events.Handlers.Player.UnlockingGenerator += OnUnlockingGenerator;
            Exiled.Events.Handlers.Player.ActivatingGenerator += OnActivatingGenerator;
            Exiled.Events.Handlers.Player.ActivatingWarheadPanel += OnActivatingWarheadPanel;
            Exiled.Events.Handlers.Player.EnteringKillerCollision += OnEnteringKillerCollision;
            Exiled.Events.Handlers.Player.EnteringEnvironmentalHazard += OnEnteringEnvironmentalHazard;
        }

        public override void Unsubscribe()
        {
            Exiled.Events.Handlers.Player.EnteringEnvironmentalHazard -= OnEnteringEnvironmentalHazard;
            Exiled.Events.Handlers.Player.EnteringKillerCollision -= OnEnteringKillerCollision;
            Exiled.Events.Handlers.Player.ActivatingWarheadPanel -= OnActivatingWarheadPanel;
            Exiled.Events.Handlers.Player.ActivatingGenerator -= OnActivatingGenerator;
            Exiled.Events.Handlers.Player.UnlockingGenerator -= OnUnlockingGenerator;
            Exiled.Events.Handlers.Player.InteractingDoor -= OnInteractingDoor;
            Exiled.Events.Handlers.Player.PickingUpItem -= OnPickupingUpItem;
            Exiled.Events.Handlers.Player.FlippingCoin -= OnFlippingCoin;
            Exiled.Events.Handlers.Player.ChangingRole -= OnChangingRole;
            Exiled.Events.Handlers.Scp330.EatingScp330 -= OnEatingScp330;
            Exiled.Events.Handlers.Player.Handcuffing -= OnHandcuffing;

            base.Unsubscribe();
        }

        public override bool Can(in Player player) => base.Can(player) && !AnyHas<Scp073>() && !AnyHas<Scp181>() && Player.List.Count() >= 5;

        protected override void OnAssigned(Player player)
        {
            player.IsUsingStamina = false;
            player.IsGodModeEnabled = true;
        }

        protected override void OnRevoked(Player player, in RevokeReason reason)
        {
            player.IsGodModeEnabled = false;
            player.IsUsingStamina = true;
        }

        private void OnFlippingCoin(FlippingCoinEventArgs ev)
        {
            if (!Has(ev.Player))
            {
                return;
            }

            _model++;

            _model = _model switch
            {
                RoleTypeId.Overwatch or RoleTypeId.Filmmaker or RoleTypeId.Scp3114 => RoleTypeId.Scp173,
                RoleTypeId.Spectator => RoleTypeId.Scp106,
                RoleTypeId.CustomRole => RoleTypeId.ChaosRifleman,
                RoleTypeId.Scp079 => RoleTypeId.ChaosConscript,
                RoleTypeId.Scp0492 => RoleTypeId.NtfSergeant,
                _ => _model
            };

            ev.Player.ShowHint($"<line-height=95%><size=90%><voffset=-20em><color={_model.GetColor().ToHex()}>Ваша моделька: {_model.Translate()}</color></size></voffset>", 5);

            ev.Player.ChangeAppearance(_model, Player.List.Where(ply => ply.IsAlive && ply.Role.Type != RoleTypeId.Scp079 && ply.UserId != Player.UserId), true);
        }

        protected internal override void OnHurt(HurtingEventArgs ev)
        {
            ev.IsAllowed = false;

            if (ev.DamageHandler.Type == DamageType.Decontamination)
            {
                ev.Player.Teleport(Room.Get(RoomType.HczServers).Position + Vector3.up * 3);

                return;
            }

            if (ev.DamageHandler.Type == DamageType.Warhead)
            {
                ev.Player.Teleport(Room.Get(RoomType.Surface).Position + Vector3.up * 3);

                return;
            }
        }

        protected internal override void OnDamage(HurtingEventArgs ev)
        {
            ev.IsAllowed = false;
        }

        private void OnChangingRole(ChangingRoleEventArgs ev)
        {
            if (!Has(ev.Player) && Player != null && ev.IsAllowed)
            {
                if (ev.NewRole is RoleTypeId.Spectator or RoleTypeId.Overwatch && ev.Player.Role.Type is not RoleTypeId.Spectator and not RoleTypeId.Overwatch)
                {
                    Player.ChangeAppearance(GameRole, new List<Player>(1) { ev.Player }, true);

                    return;
                }

                if (ev.NewRole is not RoleTypeId.Spectator and not RoleTypeId.Overwatch && ev.Player.Role.Type is RoleTypeId.Spectator or RoleTypeId.Overwatch)
                {
                    Player.ChangeAppearance(_model, new List<Player>(1) { ev.Player }, true);

                    return;
                }
            }
        }

        private void OnInteractingDoor(InteractingDoorEventArgs ev)
        {
            if (!Has(ev.Player) || ev.Door.IsMoving || ev.Door.IsLocked)
            {
                return;
            }

            ev.IsAllowed = ev.Door.Type switch
            {
                DoorType.CheckpointArmoryA or DoorType.CheckpointArmoryB
                or DoorType.HczArmory or DoorType.LczArmory
                or DoorType.Scp049Armory or DoorType.Scp173Armory
                or DoorType.HID => false,
                _ => true,
            };
        }

        private void OnHandcuffing(HandcuffingEventArgs ev)
        {
            if (!Has(ev.Target))
            {
                return;
            }

            ev.IsAllowed = false;
        }

        private void OnPickupingUpItem(PickingUpItemEventArgs ev)
        {
            if (!Has(ev.Player) || !ev.IsAllowed)
            {
                return;
            }

            ItemType id = ev.Pickup.Info.ItemId;

            if (id.GetCategory() is ItemCategory.Firearm or ItemCategory.Grenade or ItemCategory.MicroHID or ItemCategory.Ammo || id is ItemType.Jailbird or ItemType.SCP244a or ItemType.SCP244b or ItemType.SCP018 or ItemType.SCP330)
            {
                ev.Pickup.Destroy();
                ev.IsAllowed = false;
                _ = ev.Player.AddItem(ItemType.Medkit);
            }
        }

        private void OnEnteringEnvironmentalHazard(EnteringEnvironmentalHazardEventArgs ev)
        {
            if (!Has(ev.Player))
            {
                return;
            }

            ev.IsAllowed = false;
        }

        private void OnEnteringKillerCollision(EnteringKillerCollisionEventArgs ev)
        {
            if (!Has(ev.Player))
            {
                return;
            }

            ev.IsAllowed = false;
            ev.Player.Teleport(ev.Player.CurrentRoom.Doors.First().Position + Vector3.up);
        }

        private void OnActivatingWarheadPanel(ActivatingWarheadPanelEventArgs ev)
        {
            if (!Has(ev.Player))
            {
                return;
            }

            ev.IsAllowed = false;
        }

        private void OnActivatingGenerator(ActivatingGeneratorEventArgs ev)
        {
            if (!Has(ev.Player))
            {
                return;
            }

            ev.IsAllowed = false;
        }

        private void OnUnlockingGenerator(UnlockingGeneratorEventArgs ev)
        {
            if (!Has(ev.Player))
            {
                return;
            }

            ev.IsAllowed = false;
        }

        private void OnEatingScp330(EatingScp330EventArgs ev)
        {
            if (!Has(ev.Player))
            {
                return;
            }

            ev.IsAllowed = ev.Candy.Kind switch
            {
                InventorySystem.Items.Usables.Scp330.CandyKindID.Rainbow => true,
                InventorySystem.Items.Usables.Scp330.CandyKindID.Yellow => true,
                InventorySystem.Items.Usables.Scp330.CandyKindID.White => true,
                _ => false,
            };
        }
    }
}
