using PlayerRoles;
using System.Collections.Generic;

namespace Marine.ScpSwap.API
{
    public static class Swap
    {
        public static bool Prevent { get; set; }

        public static ushort SwapDuration { get; set; }

        public static List<RoleTypeId> AllowedScps { get; set; }

        public static Dictionary<RoleTypeId, int> Slots { get; set; }

        public static Dictionary<RoleTypeId, int> StartScps { get; set; }
    }
}
