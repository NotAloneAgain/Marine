using Marine.MySQL.API.Tables;

namespace Marine.MySQL.API
{
    public static class MySqlManager
    {
        private static LevelsTable _levels;
        private static SyncTable _sync;

        public static LevelsTable Levels
        {
            get
            {
                if (_levels.IsClosed)
                {
                    _levels.Open();
                }

                return _levels;
            }
        }

        public static SyncTable Sync
        {
            get
            {
                if (_sync.IsClosed)
                {
                    _sync.Open();
                }

                return _sync;
            }
        }

        public static void Init(string connDiscord, string connScp)
        {
            _levels = new(connScp);
            _sync = new(connDiscord);

            Levels.Open();
            Sync.Open();

            Levels.Create();
            Sync.Create();
        }
    }
}
