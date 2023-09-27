using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
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
        public override string Name { get; set; } = "SCP-181";

        public override string Desc { get; set; } = "Тебе настолько сильно везет, что тебя записали как аномальный SCP-объект";

        public override List<string> Abilities { get; set; } = new List<string>()
        {
            "Вероятность в 3% открыть дверь, к которой нет доступа.",
            "Вероятность в 6% избежать смертельную атаку.",
            "Пониженный до 90% получаемый урон.",
        };

        public override bool ConsoleRemark { get; } = true;

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

        public override float HurtMultiplayer { get; set; } = 0.9f;

        public override int Chance { get; set; } = 3;

        public int SurviveChance { get; set; } = 6;

        public int DoorChance { get; set; } = 3;

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

        private void OnDying(DyingEventArgs ev)
        {
            if (!Has(ev.Player) || ev.Attacker == null || ev.Player.UserId == ev.Attacker.UserId)
            {
                return;
            }

            if (Random.Range(0, 101) >= 100 - SurviveChance && ev.DamageHandler.Type is not DamageType.Warhead)
            {
                ev.IsAllowed = false;
            }
        }

        private void OnInteractingDoor(InteractingDoorEventArgs ev)
        {
            if (!Has(ev.Player) || ev.IsAllowed || ev.Door == null || ev.Door.IsMoving || ev.Door.Is(out BreakableDoor breakable) && breakable.IsDestroyed || ev.Door.IsLocked || ev.Door.IsGate && ev.Door.Type is DoorType.Scp914Gate or DoorType.GR18Gate || ev.Door.IsOpen)
            {
                return;
            }

            if (Random.Range(0, 101) >= 100 - DoorChance)
            {
                IEnumerable<Door> otherDoors = ev.Door.Room.Doors.Where(door => door != ev.Door);

                foreach (Door door in otherDoors)
                {
                    door.IsOpen = false;
                    door.Lock(1.2f, DoorLockType.NoPower);
                }

                if (ev.Door.Room == null)
                {
                    ev.Player.CurrentRoom?.TurnOffLights(1.3f);
                }
                else
                {
                    ev.Door.Room.TurnOffLights(1.3f);
                }

                ev.IsAllowed = true;
            }
        }

        public override bool Can(in Player player)
        {
            return base.Can(player) && !AnyHas<Scp073>() && !AnyHas<Scp343>() && Player.List.Count() >= 5;
        }
    }
}
