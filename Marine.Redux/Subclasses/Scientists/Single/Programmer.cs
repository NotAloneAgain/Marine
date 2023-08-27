using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Server;
using Marine.Redux.API;
using Marine.Redux.API.Inventory;
using Marine.Redux.API.Subclasses;
using MEC;
using PlayerRoles;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Marine.Redux.Subclasses.Scientists.Single
{
    public class Programmer : SingleSubclass
    {
        public override string Name { get; set; } = "Программист";

        public override SpawnInfo SpawnInfo { get; set; } = new()
        {
            ShowInfo = true,
            Message = new("Ты - Программист! Система C.A.S.S.I.E периодически сообщает тебе данные о комплексе.", 12, true),
            Inventory = new()
            {
                IsRandomable = false,
                Slots = new List<Slot>(8)
                {
                    new Slot(new ItemChances()
                    {
                        { ItemType.KeycardScientist, 100 },
                    }, false),
                    new Slot(new ItemChances()
                    {
                        { ItemType.Painkillers, 100 },
                    }, false)
                }
            }
        };

        public override RoleTypeId Role { get; set; } = RoleTypeId.Scientist;

        public override int Chance { get; set; } = 10;

        public override void Subscribe()
        {
            base.Subscribe();

            Exiled.Events.Handlers.Server.RespawningTeam += OnRespawningTeam;
        }

        public override void Unsubscribe()
        {
            Exiled.Events.Handlers.Server.RespawningTeam -= OnRespawningTeam;

            base.Unsubscribe();
        }

        protected override void OnAssigned(Player player)
        {
            player.Teleport(DoorType.LczCafe);

            Timing.RunCoroutine(_ShowData(player));
        }

        private void OnRespawningTeam(RespawningTeamEventArgs ev)
        {
            if (!ev.IsAllowed || Player == null)
            {
                return;
            }

            Player.PlayCassieAnnouncement("Warning . Unauthorized Personnel Detected in Entrance Zone");
        }

        private IEnumerator<float> _ShowData(Player player)
        {
            while (player != null && player.IsAlive && Has(player))
            {
                yield return Timing.WaitForSeconds(60);

                var list = Player.List.Where(x => x.IsAlive && x.UserId != player.UserId && x.LeadingTeam == player.LeadingTeam && !x.IsTutorial);

                if (list.Any())
                {
                    foreach (var ply in list)
                    {
                        player.SendConsoleMessage($"{ply.CustomName} - {ply.Role.Type.Translate()}: {ParseState(ply)} {ParseDistance(player, ply)}", ply.Role.Team switch
                        {
                            Team.SCPs => "red",
                            Team.ChaosInsurgency => "green",
                            _ => "yellow"
                        });
                    }

                    player.ShowHint(SpawnInfo.Message.Formate("Вам поступила информация от C.A.S.S.I.E", Role), 6);
                }
            }
        }

        private string ParseDistance(Player player, Player ply)
        {
            if (player.Zone != ply.Zone)
            {
                return "находиться " + ply.Zone switch
                {
                    ZoneType.LightContainment => "в лёгкой зоне содержания",
                    ZoneType.HeavyContainment => "в тяжёлой зоне содержания",
                    ZoneType.Entrance => "в офисной зоне",
                    ZoneType.Surface => "на поверхности",
                    _ => "в неизвестной зоне",
                };
            }

            return $"находиться в {GetSuffix(Vector3.Distance(player.Position, ply.Position))}";
        }

        private string ParseState(Player ply)
        {
            if (ply.IsScp)
            {
                return "состояние неизвестно";
            }

            var percent = ply.Health / ply.MaxHealth * 100;

            if (ply.IsScp)
            {
                return "состояние неизвестно";
            }

            return percent switch
            {
                > 75 => "в норме",
                > 50 => "потрепан",
                > 25 => "ранен",
                > 0 => "при смерти",
                _ => "ты че бля еже какой нахуй, ArgumentOutOfRangeException бы выкинуть, но тогда абилка крашнется нахуй"
            };
        }

        private string GetSuffix(float value)
        {
            int distance = Mathf.RoundToInt(value);

            if (distance % 10 == 1 && distance % 100 != 11)
            {
                return $"{value} метр";
            }
            else if (new List<int> { 2, 3, 4 }.Contains(distance % 10) && !new List<int> { 12, 13, 14 }.Contains(distance % 100))
            {
                return $"{value} метра";
            }
            else
            {
                return $"{value} метров";
            }
        }
    }
}
