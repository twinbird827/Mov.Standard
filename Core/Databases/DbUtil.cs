using My.Core;
using My.Core.Databases.SQLite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mov.Standard.Core.Databases
{
    public static class DbUtil
    {
        public static Task<SQLiteControl> GetControl()
        {
            var path = "database.sqlite3";
            var work = System.AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');
            var full = Path.Combine(work, path);

            Directory.CreateDirectory(Path.GetDirectoryName(full));

            return SQLiteControl.CreateAsync(full);
        }

        public static async Task InitializeDatabase(this SQLiteControl command)
        {
            await command.Create();
        }

        private static async Task Create(this SQLiteControl command)
        {
            var sql = new StringBuilder();

            sql.Clear();
            sql.AppendLine($"CREATE TABLE IF NOT EXISTS t_setting (");
            sql.AppendLine($"    key1        TEXT    NOT NULL,");
            sql.AppendLine($"    key2        TEXT    NOT NULL,");
            sql.AppendLine($"    value       TEXT    NOT NULL,");
            sql.AppendLine($"PRIMARY KEY (key1, key2)");
            sql.AppendLine($")");

            await command.ExecuteNonQueryAsync(sql.ToString());

            sql.Clear();
            sql.AppendLine($"CREATE TABLE IF NOT EXISTS t_favorite (");
            sql.AppendLine($"    mylist      TEXT    NOT NULL,");
            sql.AppendLine($"    tick        INTEGER NOT NULL,");
            sql.AppendLine($"PRIMARY KEY (mylist)");
            sql.AppendLine($")");

            await command.ExecuteNonQueryAsync(sql.ToString());

            sql.Clear();
            sql.AppendLine($"CREATE TABLE IF NOT EXISTS t_video_history (");
            sql.AppendLine($"    tick          INTEGER NOT NULL,");
            sql.AppendLine($"    video         TEXT    NOT NULL,");
            sql.AppendLine($"    status        INTEGER NOT NULL,");
            sql.AppendLine($"PRIMARY KEY (tick, video)");
            sql.AppendLine($")");

            await command.ExecuteNonQueryAsync(sql.ToString());

            sql.Clear();
            sql.AppendLine($"CREATE TABLE IF NOT EXISTS t_temporary (");
            sql.AppendLine($"    tick          INTEGER NOT NULL,");
            sql.AppendLine($"    video         TEXT    NOT NULL,");
            sql.AppendLine($"PRIMARY KEY (video)");
            sql.AppendLine($")");

            await command.ExecuteNonQueryAsync(sql.ToString());

        }
    }
}
