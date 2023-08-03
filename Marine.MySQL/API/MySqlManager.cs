using Marine.MySQL.API.Tables;

namespace Marine.MySQL.API
{
    public static class MySqlManager
    {
        public static LevelsTable Levels { get; private set; }

        public static SyncTable Sync { get; private set; }

        public static void Init(string connDiscord, string connScp)
        {
            Levels = new(connScp);
            Sync = new(connDiscord);

            Levels.Open();
            Sync.Open();

            Levels.Create();
            Sync.Create();
        }
    }
}
