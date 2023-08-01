using Marine.Redux.API.Enums;
using PlayerRoles;

namespace Marine.Redux.API.Interfaces
{
    public interface ISubclass : IHasName
    {
        SubclassType Type { get; }

        int Chance { get; set; }

        RoleTypeId Role { get; set; }

        RoleTypeId GameRole { get; set; }

        SpawnInfo SpawnInfo { get; set; }

        void Subscribe();

        void Unsubscribe();
    }
}
