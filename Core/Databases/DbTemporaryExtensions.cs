using My.Core.Databases.SQLite;
using My.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mov.Standard.Core.Databases
{
    public static class DbTemporaryExtensions
    {
        public static async Task<int> InsertTTemporaryAsync(this SQLiteControl command, string video)
        {
            var sql = new StringBuilder();

            sql.Clear();
            sql.AppendLine($"INSERT INTO t_temporary (");
            sql.AppendLine($"   video, tick");
            sql.AppendLine($")");
            sql.AppendLine($"VALUES (");
            sql.AppendLine($"   ?, ?");
            sql.AppendLine($")");

            var parameters = new[]
            {
                Sqlite3Util.CreateParameter(DbType.String, video),
                Sqlite3Util.CreateParameter(DbType.Int64, DateTime.Now.Ticks),
            };

            return await command.ExecuteNonQueryAsync(sql.ToString(), parameters);
        }

        public static async Task<IEnumerable<TTemporary>> SelectTTemporaryAsync(this SQLiteControl command)
        {
            var sql = new StringBuilder();

            sql.Clear();
            sql.AppendLine($"SELECT tick, video");
            sql.AppendLine($"FROM   t_temporary");

            using (var reader = await command.ExecuteReaderAsync(sql.ToString()))
            {
                return await reader.GetRows(r => new TTemporary(r));
            }
        }

        public static async Task<int> DeleteTTemporaryAsync(this SQLiteControl command, string video)
        {
            var sql = new StringBuilder();

            sql.Clear();
            sql.AppendLine($"DELETE FROM t_temporary");
            sql.AppendLine($"WHERE  video      = ?");

            var parameters = new[]
            {
                Sqlite3Util.CreateParameter(DbType.String, video),
            };

            return await command.ExecuteNonQueryAsync(sql.ToString(), parameters);
        }

    }
}
