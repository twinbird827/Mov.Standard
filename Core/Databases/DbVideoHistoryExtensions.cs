using Mov.Standard.Models;
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
    public static class DbVideoHistoryExtensions
    {
        public static async Task<TVideoHistory> InsertTVideoHistoryAsync(this SQLiteControl command, string video, VideoStatus status)
        {
            var sql = new StringBuilder();

            sql.Clear();
            sql.AppendLine($"INSERT INTO t_video_history (");
            sql.AppendLine($"   tick, video, status");
            sql.AppendLine($")");
            sql.AppendLine($"VALUES (");
            sql.AppendLine($"   ?, ?, ?");
            sql.AppendLine($")");

            var now = DateTime.Now.Ticks;
            var parameters = new[]
            {
                Sqlite3Util.CreateParameter(DbType.Int64, now),
                Sqlite3Util.CreateParameter(DbType.String, video),
                Sqlite3Util.CreateParameter(DbType.Int32, (int)status),
            };

            await command.ExecuteNonQueryAsync(sql.ToString(), parameters);

            return new TVideoHistory(
                now, video, (int)status 
            );
        }

        public static async Task<IEnumerable<TVideoHistory>> SelectTVideoHistoryAsync(this SQLiteControl command)
        {
            var sql = new StringBuilder();

            sql.Clear();
            sql.AppendLine($"SELECT tick, video, status");
            sql.AppendLine($"FROM   t_video_history");

            using (var reader = await command.ExecuteReaderAsync(sql.ToString()))
            {
                return await reader.GetRows(r => new TVideoHistory(r));
            }
        }
    }
}
