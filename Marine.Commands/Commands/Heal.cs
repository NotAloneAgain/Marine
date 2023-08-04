using Achievements;
using Exiled.API.Features;
using InventorySystem.Items.Usables;
using Marine.Commands.API;
using Marine.Commands.API.Abstract;
using Marine.Commands.API.Enums;
using Marine.Redux.API.Subclasses;
using Marine.Redux.Subclasses.Scientists.Group;
using PlayerStatsSystem;
using System.Collections.Generic;

namespace Marine.Commands.Commands
{
    public sealed class Heal : CooldownCommand
    {
        public override string Command { get; set; } = "heal";

        public override string Description { get; set; } = "Команда для лечения.";

        public override List<CommandType> Types { get; set; } = new List<CommandType>(1) { CommandType.PlayerConsole };

        public override List<int> Counts { get; set; } = new List<int>(1) { 0 };

        public override Dictionary<int, string> Syntax { get; set; } = new Dictionary<int, string>()
        {
            { 0, string.Empty }
        };

        public override CommandPermission Permission { get; set; } = new()
        {
            IsLimited = true,
        };

        public override int Cooldown { get; set; } = 3;

        public override CommandResultType Handle(List<object> arguments, Player player, out string response)
        {
            if (base.Handle(arguments, player, out response) == CommandResultType.Fail)
            {
                return CommandResultType.Fail;
            }

            if (player.CurrentItem == null || player.CurrentItem.Type is not ItemType.Painkillers and not ItemType.SCP500 and not ItemType.Medkit)
            {
                response = "Чем лечить то? Поцелуем?";

                return CommandResultType.Fail;
            }

            Player target = player.GetFromView(5);

            if (target == null || target == player || !target.IsAlive)
            {
                response = "Цель нераспознана";

                return CommandResultType.Fail;
            }

            if (target.Health == target.MaxHealth)
            {
                response = "У него и так полное здоровье!";

                return CommandResultType.Fail;
            }

            var item = player.CurrentItem.Base as Consumable;

            switch (item.ItemTypeId)
            {
                case ItemType.Painkillers:
                    {
                        var med = item as Painkillers;

                        item.ServerAddRegeneration(med._healProgress, 0.06666667f, 100);
                        target.ReferenceHub.playerEffectsController.UseMedicalItem(med);

                        break;
                    }
                case ItemType.SCP500:
                    {
                        var med = item as Scp500;

                        HealthStat module = target.GetModule<HealthStat>();

                        if (module.CurValue < 20f)
                        {
                            AchievementHandlerBase.ServerAchieve(target.NetworkIdentity.connectionToClient, AchievementName.CrisisAverted);
                        }

                        module.ServerHeal(100);
                        item.ServerAddRegeneration(med._healProgress, 0.1f, 200);
                        target.ReferenceHub.playerEffectsController.UseMedicalItem(med);

                        break;
                    }
                case ItemType.Medkit:
                    {
                        target.Heal(130);

                        break;
                    }
            }

            player.RemoveHeldItem();
            target.ShowHint($"<b>Вас подлатал {player.CustomName}</b>", 4);

            return CommandResultType.Success;
        }

        public override bool ParseSyntax(List<string> input, int count, out List<object> output)
        {
            output = new List<object>();

            return true;
        }

        public override bool CheckPermissions(Player player) => base.CheckPermissions(player) || Subclass.Has<Medic>(player);
    }
}
