﻿using Exiled.API.Features;
using Marine.MySQL.API.Enums;
using Marine.MySQL.API.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
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
            if (_connection.State != System.Data.ConnectionState.Open)
            {
                _connection.Open();
            }

            MySqlCommand command = new()
            {
                Connection = _connection,
                CommandText = $"INSERT INTO {Name} ({string.Join(", ", Columns.Select(column => column.Name))}) VALUES ('{stats.UserId}', {stats.Level}, {stats.Experience}, {stats.ExpMultiplayer});"
            };

            _ = command.ExecuteNonQuery();
        }

        public override void Update(Statistics stats)
        {
            if (_connection.State != System.Data.ConnectionState.Open)
            {
                _connection.Open();
            }

            MySqlCommand command = new()
            {
                Connection = _connection,
                CommandText = $"UPDATE {Name} SET level={stats.Level}, experience={stats.Experience}, multiplayer={stats.ExpMultiplayer} WHERE user_id='{stats.UserId}';"
            };

            _ = command.ExecuteNonQuery();
        }

        public override void Delete(Statistics stats)
        {
            if (_connection.State != System.Data.ConnectionState.Open)
            {
                _connection.Open();
            }

            MySqlCommand command = new()
            {
                Connection = _connection,
                CommandText = $"DELETE FROM {Name} WHERE user_id='{stats.UserId}';"
            };

            _ = command.ExecuteNonQuery();
        }

        public override Statistics Select(string key)
        {
            if (_connection.State != System.Data.ConnectionState.Open)
            {
                _connection.Open();
            }

            MySqlCommand command = new()
            {
                Connection = _connection,
                CommandText = $"SELECT * FROM {Name} WHERE user_id='{key}';"
            };

            Statistics level = null;

            using (MySqlDataReader reader = command.ExecuteReader())
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
