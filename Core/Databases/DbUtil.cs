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
        public static SQLiteControl GetControl()
        {
            var path = "database.sqlite3";
            var work = System.AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');
            var full = Path.Combine(work, path);

            Directory.CreateDirectory(Path.GetDirectoryName(full));

            return new SQLiteControl(
                path,
                IsolationLevel.ReadCommitted,
                SynchronizationModes.Off,
                SQLiteJournalModeEnum.Wal,
                false,
                true,
                65536
            );
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
            sql.AppendLine($"    group       TEXT    NOT NULL,");
            sql.AppendLine($"    key         TEXT    NOT NULL,");
            sql.AppendLine($"    value       TEXT    NOT NULL,");
            sql.AppendLine($"PRIMARY KEY (group, key)");
            sql.AppendLine($")");

            await command.ExecuteNonQueryAsync(sql.ToString());

        }
    }
}
