using Marine.MySQL.API.Enums;
using System.Collections.Generic;
using System.Text;

namespace Marine.MySQL.API.Models
{
    public class Column
    {
        public Column(string name, MySqlDataType type, List<MySqlDataFlags> flags, object def = null)
        {
            Name = name;
            Type = type;
            Flags = flags;
            Default = def;
        }

        public string Name { get; set; }

        public MySqlDataType Type { get; set; }

        public List<MySqlDataFlags> Flags { get; set; }

        public object Default { get; set; }

        public override string ToString()
        {
            var builder = new StringBuilder(Name);

            _ = builder.Append(" ");

            _ = builder.Append(Type.AsString());

            foreach (MySqlDataFlags flag in Flags)
            {
                _ = builder.Append(" ");

                _ = builder.Append(flag.AsString());
            }

            if (Default != null)
            {
                _ = builder.Append($" DEFAULT {(Default is string ? $"'{Default}'" : Default)}");
            }

            return builder.ToString();
        }
    }
}
