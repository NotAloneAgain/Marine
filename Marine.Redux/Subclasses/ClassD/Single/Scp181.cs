using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Marine.Redux.API;
using Marine.Redux.API.Inventory;
using Marine.Redux.API.Subclasses;
using PlayerRoles;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Marine.Redux.Subclasses.ClassD.Single
{
    public class Scp181 : SingleSubclass
    {
        private const string ConsoleMessage = "\n\t+ У тебя есть:" +
                "\n\t\t- Шанс в 3% открыть дверь, к которой доступа не имеешь из-за сбоев в комплексе." +
                "\n\t\t- На 10% меньше урона от любых источников, ведь все травмы приходятся в менее важные места." +
                "\n\t\t- Шанс 5% избежать смертельный урон.";

        public Scp181() : base() { }

        public int DoorChance { get; set; } = 3;

        public int SurviveChance { get; set; } = 5;

        public override string Name { get; set; } = "SCP-181";

        public override SpawnInfo SpawnInfo { get; set; } = new()
        {
            ShowInfo = true,
            Message = new("Ты - Везунчик.\nТебе настолько сильно везет, что тебя записали как аномальный объект SCP (проверь консоль).", 15, true, "#009A63"),
            Inventory = new()
            {
                IsRandomable = true,
                Slots = new List<Slot>(8)
            }
        };

        public override RoleTypeId Role { get; set; } = RoleTypeId.ClassD;

        public override int Chance { get; set; } = 3;

        public override void Subscribe()
        {
            base.Subscribe();

            Exiled.Events.Handlers.Player.Dying += OnDying;
            Exiled.Events.Handlers.Player.InteractingDoor += OnInteractingDoor;
        }

        public override void Unsubscribe()
        {
            Exiled.Events.Handlers.Player.Dying -= OnDying;
            Exiled.Events.Handlers.Player.InteractingDoor -= OnInteractingDoor;

            base.Unsubscribe();
        }

        protected override void OnAssigned(Player player)
        {
            base.OnAssigned(player);

            player.GetEffect(EffectType.DamageReduction)?.ServerSetState(20, 0, false);

            player.SendConsoleMessage(ConsoleMessage, "yellow");
        }

        private void OnDying(DyingEventArgs ev)
        {
            if (!Has(ev.Player) || ev.Attacker == null || ev.Player == ev.Attacker)
            {
                return;
            }

            if (Random.Range(0, 101) >= 100 - SurviveChance)
            {
                ev.IsAllowed = false;
            }
        }

        private void OnInteractingDoor(InteractingDoorEventArgs ev)
        {
            if (!Has(ev.Player) || ev.IsAllowed || ev.Door.IsMoving || ev.Door.IsBroken || ev.Door.IsLocked || ev.Door.IsGate && ev.Door.Type is DoorType.Scp914Gate or DoorType.GR18Gate || ev.Door.IsOpen)
            {
                return;
            }

            if (Random.Range(0, 101) >= 100 - DoorChance)
            {
                var otherDoors = ev.Door.Room.Doors.Where(door => door != ev.Door);

                foreach (var door in otherDoors)
                {
                    door.IsOpen = false;
                    door.Lock(1.28f, DoorLockType.NoPower);
                }

                ev.Door.Room.TurnOffLights(1.18f);

                ev.IsAllowed = true;
            }
        }

        public override bool Can(in Player player) => base.Can(player) && !AnyHas<Scp073>() && !AnyHas<Scp343>() && Player.List.Count(ply => ply.IsScp) >= 2;
    }
}
