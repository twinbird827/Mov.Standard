using My.Core.Databases.SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mov.Standard.Core.Database
{
    public static class DbUtil
    {
        public static Task<SQLiteControl> GetControl()
        {
            var path = @"lib\database.sqlite3";
            var work = System.AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');
            var full = Path.Combine(work, path);

            Directory.CreateDirectory(Path.GetDirectoryName(full));

            return SQLiteControl.CreateAsync(path);
        }

        public static async Task Initialize(SQLiteControl command)
        {
            await CreateTable(command);

            await CreateView(command);
        }

        private static async Task CreateTable(SQLiteControl command)
        {
            var sql = new StringBuilder();

            sql.Clear();
            sql.AppendLine($"CREATE TABLE IF NOT EXISTS t_nico_temporary (");
            sql.AppendLine($"    video_id     TEXT    NOT NULL,");
            sql.AppendLine($"    tick         INTEGER NOT NULL,");
            sql.AppendLine($"PRIMARY KEY (video_id)");
            sql.AppendLine($")");

            await command.ExecuteNonQueryAsync(sql.ToString());

            sql.Clear();
            sql.AppendLine($"CREATE TABLE IF NOT EXISTS t_nico_history (");
            sql.AppendLine($"    video_id     TEXT    NOT NULL,");
            sql.AppendLine($"    tick         INTEGER NOT NULL,");
            sql.AppendLine($"PRIMARY KEY (video_id, tick)");
            sql.AppendLine($")");

            await command.ExecuteNonQueryAsync(sql.ToString());

            sql.Clear();
            sql.AppendLine($"CREATE TABLE IF NOT EXISTS t_nico_favorite (");
            sql.AppendLine($"    mylist_id    TEXT    NOT NULL,");
            sql.AppendLine($"    tick         INTEGER NOT NULL,");
            sql.AppendLine($"PRIMARY KEY (mylist_id)");
            sql.AppendLine($")");

            await command.ExecuteNonQueryAsync(sql.ToString());

        }

        private static async Task CreateView(SQLiteControl command)
        {
            var sql = new StringBuilder();

            sql.Clear();
            sql.AppendLine($"CREATE VIEW IF NOT EXISTS v_nico_history AS");
            sql.AppendLine($"SELECT");
            sql.AppendLine($"    video_id,");
            sql.AppendLine($"    MAX(tick) tick,");
            sql.AppendLine($"    COUNT(video_id) cnt");
            sql.AppendLine($"FROM");
            sql.AppendLine($"    t_nico_history");
            sql.AppendLine($"GROUP BY");
            sql.AppendLine($"    video_id");

            await command.ExecuteNonQueryAsync(sql.ToString());
        }

    }
}
