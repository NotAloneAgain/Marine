using Marine.MySQL.API.Models;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System;
using Marine.MySQL.API.Enums;
using Exiled.API.Features;
using System.Linq;

namespace Marine.MySQL.API.Tables
{
    public class LevelsTable : Table<string, Statistics>
    {
        public LevelsTable(string connection) : base(connection)
        {
        }

        public override string Name { get; } = "levels";

        public override List<Column> Columns { get; } = new()
        {
            new Column("user_id", MySqlDataType.VarChar, new List<MySqlDataFlags>()
            {
                MySqlDataFlags.NotNull,
                MySqlDataFlags.PrimaryKey,
                MySqlDataFlags.Unique,
            }),
            new Column("level", MySqlDataType.Int, new List<MySqlDataFlags>()
            {
                MySqlDataFlags.NotNull,
            }, 0),
            new Column("experience", MySqlDataType.Int, new List<MySqlDataFlags>()
            {
                MySqlDataFlags.NotNull,
            }, 0),
            new Column("multiplayer", MySqlDataType.SmallInt, new List<MySqlDataFlags>()
            {
                MySqlDataFlags.Unsigned,
                MySqlDataFlags.NotNull
            })
        };

        public override void Insert(Statistics stats)
        {
            MySqlCommand command = new()
            {
                Connection = _connection,
                CommandText = $"INSERT INTO {Name} ({string.Join(", ", Columns.Select(column => column.Name))}) VALUES ('{stats.UserId}', {stats.Level}, {stats.Experience}, {stats.Experience});"
            };

            command.ExecuteNonQuery();
        }

        public override void Update(Statistics stats)
        {
            MySqlCommand command = new()
            {
                Connection = _connection,
                CommandText = $"UPDATE {Name} SET level={stats.Level}, experience={stats.Experience}, multiplayer={stats.ExpMultiplayer} WHERE user_id='{stats.UserId}';"
            };

            command.ExecuteNonQuery();
        }

        public override void Delete(Statistics stats)
        {
            MySqlCommand command = new()
            {
                Connection = _connection,
                CommandText = $"DELETE FROM {Name} WHERE user_id={stats.UserId};"
            };

            command.ExecuteNonQuery();
        }

        public override Statistics Select(string key)
        {
            MySqlCommand command = new()
            {
                Connection = _connection,
                CommandText = $"SELECT * FROM {Name} WHERE user_id='{key}';"
            };

            Statistics level = null;

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    try
                    {
                        level = new(reader.GetString(0), reader.GetInt32(1), reader.GetInt32(2), reader.GetInt16(3));
                    }
                    catch (Exception err)
                    {
                        Log.Error($"Error occured on reading: {err}");
                    }
                }
            }

            return level;
        }
    }
}
