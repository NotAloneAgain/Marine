using Exiled.API.Features;
using Marine.Commands.API.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marine.Commands.API.Abstract
{
    public abstract class CooldownCommand : CommandBase
    {
        public abstract int Cooldown { get; set; }

        public override CommandResultType Handle(List<object> arguments, Player player, out string response)
        {
            var time = (DateTime.Now - History.GetLastSuccessfulUse(player).Time);

            response = string.Empty;

            if (time.TotalSeconds <= Cooldown)
            {
                response = $"Вам осталось ждать {(Cooldown - time.TotalSeconds).GetSecondsString()}.";

                return CommandResultType.Fail;
            }

            return CommandResultType.Success;
        }
    }
}
