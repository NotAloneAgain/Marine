using Marine.MySQL.API.Models;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Marine.MySQL.API.Tables
{
    public abstract class Table<TKey, TClass> where TClass : class
    {
        private protected readonly MySqlConnection _connection;

        public Table(string conn) => _connection = new(conn);

        public abstract string Name { get; }

        public abstract List<Column> Columns { get; }

        public bool IsClosed => _connection.State is not ConnectionState.Open and not ConnectionState.Connecting;

        public void Open()
        {
            _connection.Open();
        }

        public void Close()
        {
            _connection.Close();
        }

        public virtual void Create()
        {
            var builder = new StringBuilder($"CREATE TABLE IF NOT EXISTS {Name} (");

            for (var index = 0; index < Columns.Count; index++)
            {
                Column column = Columns[index];

                _ = builder.Append(column.ToString());

                if (index != Columns.Count - 1)
                {
                    _ = builder.Append(", ");
                }
            }

            _ = builder.Append(");");

            MySqlCommand command = new()
            {
                Connection = _connection,
                CommandText = builder.ToString()
            };

            _ = command.ExecuteNonQuery();
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

            var count = 0;

            using (MySqlDataReader reader = command.ExecuteReader())
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
