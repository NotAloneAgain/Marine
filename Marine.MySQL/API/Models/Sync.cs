namespace Marine.MySQL.API.Models
{
    public class Sync
    {
        public Sync(uint id, ulong discordId, string userId, bool inGame)
        {
            Id = id;
            DiscordId = discordId;
            UserId = userId;
            InGame = inGame;
        }

        public uint Id { get; }

        public ulong DiscordId { get; }

        public string UserId { get; set; }

        public bool InGame { get; set; }
    }
}
