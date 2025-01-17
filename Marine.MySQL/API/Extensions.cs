﻿using Marine.MySQL.API.Enums;

namespace Marine.MySQL.API
{
    public static class Extensions
    {
        public static string AsString(this MySqlDataType type)
        {
            return type switch
            {
                MySqlDataType.VarChar => "VARCHAR(255)",
                _ => ((object)type).AsString().ToUpper(),
            };
        }

        public static string AsString(this MySqlDataFlags flags)
        {
            return flags switch
            {
                MySqlDataFlags.NotNull => "NOT NULL",
                MySqlDataFlags.PrimaryKey => "PRIMARY KEY",
                MySqlDataFlags.AutoIncrement => "AUTO_INCREMENT",
                _ => ((object)flags).AsString().ToUpper(),
            };
        }

        public static string AsString(this object obj)
        {
            return obj is null ? "null" : obj.ToString();
        }

        public static int AsNumber(this bool value)
        {
            return value ? 1 : 0;
        }

        public static bool AsBool(this byte value)
        {
            return value == 1;
        }
    }
}
