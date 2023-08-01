using Exiled.API.Features;
using Marine.Commands.API;
using Marine.Commands.API.Abstract;
using Marine.Commands.API.Enums;
using Marine.Redux.API.Subclasses;
using Marine.Redux.Subclasses.Group;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Marine.Commands.Commands
{
    internal sealed class Steal : CooldownCommand
    {
        #region Initialize
        private const string StealFailed = "<line-height=95%><size=95%><voffset=-20em><color=#BC5D58>Вы услышали как что-то шуршит в ваших карманах... {0} выглядит подозрительным...</color></size></voffset>";
        private static readonly IReadOnlyList<ItemType> _banned;

        static Steal()
        {
            _banned = new List<ItemType>
            {
                 ItemType.ParticleDisruptor,
                 ItemType.MicroHID,
                 ItemType.GunLogicer,
                 ItemType.ArmorCombat,
                 ItemType.ArmorHeavy,
                 ItemType.ArmorLight,
                 ItemType.GunShotgun,
                 ItemType.GunCom45,
                 ItemType.GunFSP9,
                 ItemType.GunE11SR,
                 ItemType.GunCrossvec,
                 ItemType.SCP244a,
                 ItemType.SCP244b,
            };
        }
        #endregion

        public override string Command { get; set; } = "steal";

        public override List<CommandType> Types { get; set; } = new List<CommandType>(1) { CommandType.PlayerConsole };

        public override List<int> Counts { get; set; } = new List<int>(1) { 0 };

        public override Dictionary<int, string> Syntax { get; set; } = new Dictionary<int, string>()
        {
            { 0, string.Empty }
        };

        public override CommandPermission Permission { get; set; } = new()
        {
            IsLimited = true,
            Custom = (Player ply) => Subclass.Has<Pickpocket>(ply) || Subclass.Has<Thief>(ply)
        };

        public override int Cooldown { get; set; } = 30;

        public override CommandResultType Handle(List<object> arguments, Player player, out string response)
        {
            if (base.Handle(arguments, player, out response) == CommandResultType.Fail)
            {
                return CommandResultType.Fail;
            }

            if (player.IsCuffed)
            {
                response = "Вы не можете ничего украсть когда связаны!";

                return CommandResultType.Fail;
            }

            Player target = player.GetFromView();

            if (target == null || target == player || !target.IsAlive)
            {
                response = "Цель нераспознана";

                return CommandResultType.Fail;
            }

            if (target.IsInventoryEmpty)
            {
                response = "Вы обнаружили только пустые карманы";

                return CommandResultType.Fail;
            }

            var items = target.Items.Where(item => item != null && item != target.CurrentItem && !_banned.Contains(item.Type)).Select(x => x.Type).Distinct();

            if (items.Count() == 0)
            {
                response = "Вы не обнаружили предметы, которые можете украсть.";

                return CommandResultType.Fail;
            }

            ItemType targetItem = items.ElementAt(Random.Range(0, items.Count()));

            if (Random.Range(0, 101) >= 90)
            {
                target.ShowHint(string.Format(StealFailed, player.CustomName), 6);

                return CommandResultType.Fail;
            }

            player.AddItem(targetItem);
            target.RemoveItem(target.Items.First(item => item.Type == targetItem));

            if (Random.Range(0, 101) >= 60)
            {
                response = "Успешно, но кажется он что-то понял...";

                target.ShowHint(string.Format(StealFailed, player.CustomName), 6);
            }

            return CommandResultType.Success;
        }

        public override bool ParseSyntax(List<string> input, int count, out List<object> output)
        {
            output = new List<object>();

            return true;
        }
    }
}
