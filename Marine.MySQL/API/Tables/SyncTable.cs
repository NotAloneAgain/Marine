using Marine.MySQL.API.Enums;
using Marine.MySQL.API.Models;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Marine.MySQL.API.Tables
{
    public class SyncTable : Table<string, Sync>
    {
        public SyncTable(string conn) : base(conn)
        {
        }

        public override string Name { get; } = "sync";

        public override List<Column> Columns { get; } = new List<Column>(3)
        {
            new Column("id", MySqlDataType.Int, new List<MySqlDataFlags>
            {
                MySqlDataFlags.Unsigned,
                MySqlDataFlags.NotNull,
                MySqlDataFlags.Unique,
                MySqlDataFlags.PrimaryKey,
                MySqlDataFlags.AutoIncrement
            }),
            new Column("discord_id", MySqlDataType.BigInt, new List<MySqlDataFlags>
            {
                MySqlDataFlags.Unsigned,
                MySqlDataFlags.NotNull,
                MySqlDataFlags.Unique,
            }),
            new Column("user_id", MySqlDataType.VarChar, new List<MySqlDataFlags>
            {
                MySqlDataFlags.NotNull,
                MySqlDataFlags.Unique,
            }),
            new Column("game", MySqlDataType.TinyInt, new List<MySqlDataFlags>
            {
                MySqlDataFlags.NotNull,
            })
        };

        public override void Insert(Sync sync)
        {
            if (sync == null)
            {
                return;
            }

            MySqlCommand command = new()
            {
                Connection = _connection,
                CommandText = $"INSERT INTO {Name} ({string.Join(", ", Columns.Skip(1).Select(column => column.Name))}) VALUES ({sync.DiscordId}, '{sync.UserId}', {sync.InGame.AsNumber()});"
            };

            command.ExecuteNonQuery();
        }

        public override void Update(Sync sync)
        {
            if (sync == null)
            {
                return;
            }

            MySqlCommand command = new()
            {
                Connection = _connection,
                CommandText = $"UPDATE {Name} SET discord_id={sync.DiscordId}, user_id='{sync.UserId}', game={sync.InGame.AsNumber()} WHERE id={sync.Id};"

            };

            command.ExecuteNonQuery();
        }

        public override void Delete(Sync sync)
        {
            if (sync == null)
            {
                return;
            }

            MySqlCommand command = new()
            {
                Connection = _connection,
                CommandText = $"DELETE FROM {Name} WHERE id={sync.Id};"
            };

            command.ExecuteNonQuery();
        }

        public override Sync Select(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return null!;
            }

            MySqlCommand command = new()
            {
                Connection = _connection,
                CommandText = $"SELECT * FROM {Name} WHERE user_id='{userId}';"
            };


            Sync sync = null!;

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    try
                    {
                        sync = new Sync(reader.GetUInt32(0), reader.GetUInt64(1), reader.GetString(2), reader.GetByte(3).AsBool());
                    }
                    catch (Exception err)
                    {
                        Console.WriteLine("Error occured on Select Sync: {0}", err);
                    }
                }
            }

            return sync;
        }
    }
}
