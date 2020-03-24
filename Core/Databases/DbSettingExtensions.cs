using My.Core.Databases.SQLite;
using My.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mov.Standard.Core.Databases
{
    public static class DbSettingExtensions
    {
        public static async Task<IEnumerable<TSetting>> SelectTSetting(this SQLiteControl command)
        {
            var sql = new StringBuilder();

            sql.Clear();
            sql.AppendLine($"SELECT group, key, value FROM t_setting");

            using (var reader = await command.ExecuteReaderAsync(sql.ToString()))
            {
                return await reader.GetRows(r => new TSetting(r));
            }
        }

        public static Task<int> InsertTSetting(this SQLiteControl command, string group, string key, string value)
        {
            var sql = new StringBuilder();

            sql.Clear();
            sql.AppendLine($"INSERT INTO t_setting (group, key, value) VALUES (?, ?, ?)");

            var parameters = new DbParameter[]
            {
                Sqlite3Util.CreateParameter(DbType.String, group),
                Sqlite3Util.CreateParameter(DbType.String, key),
                Sqlite3Util.CreateParameter(DbType.String, value),
            };

            return command.ExecuteNonQueryAsync(sql.ToString(), parameters);
        }

    }
}
