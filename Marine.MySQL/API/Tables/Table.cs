using Marine.MySQL.API.Models;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Text;

namespace Marine.MySQL.API.Tables
{
    public abstract class Table<TKey, TClass> where TClass : class
    {
        private protected readonly MySqlConnection _connection;

        public Table(string conn) => _connection = new(conn);

        public abstract string Name { get; }

        public abstract List<Column> Columns { get; }

        public void Open() => _connection.Open();

        public void Close() => _connection.Close();

        public virtual void Create()
        {
            var builder = new StringBuilder($"CREATE TABLE IF NOT EXISTS {Name} (");

            for (var index = 0; index < Columns.Count; index++)
            {
                var column = Columns[index];

                builder.Append(column.ToString());

                if (index != Columns.Count - 1)
                {
                    builder.Append(", ");
                }
            }

            builder.Append(");");

            MySqlCommand command = new()
            {
                Connection = _connection,
                CommandText = builder.ToString()
            };

            command.ExecuteNonQuery();
        }

        public abstract void Insert(TClass arg);

        public abstract void Update(TClass arg);

        public abstract void Delete(TClass arg);

        public virtual int Count()
        {
            MySqlCommand command = new()
            {
                Connection = _connection,
                CommandText = $"SELECT * FROM {Name};"
            };

            int count = 0;

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    count++;
                }
            }

            return count;
        }

        public abstract TClass Select(TKey key);
    }
}
