using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marine.Commands.Patches
{
    [HarmonyPatch()]
    public class CustomKickPatch : CustomCommandPatch
    {
        public override List<object> ParseArguments(List<string> args)
        {
            throw new NotImplementedException();
        }
    }
}
