using My.Core.Databases.SQLite;
using My.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mov.Standard.Core.Database
{
    public static class DbNicoExtensions
    {
        public static async Task<IEnumerable<TNicoTemporary>> SelectNicoTemporary(this SQLiteControl command)
        {
            var sql = new StringBuilder();

            sql.Clear();
            sql.AppendLine($"SELECT video_id, tick FROM t_nico_temporary ORDER BY tick DESC");

            using (var reader = await command.ExecuteReaderAsync(sql.ToString()))
            {
                return await reader.GetRows(r => new TNicoTemporary(r));
            }
        }

        public static async Task MergeNicoTemporary(this SQLiteControl command, string videoid, DateTime date)
        {
            var sql = new StringBuilder();

            sql.Clear();
            sql.AppendLine($"INSERT OR REPLACE INTO t_nico_temporary (");
            sql.AppendLine($"    video_id, tick");
            sql.AppendLine($")");
            sql.AppendLine($"VALUES (");
            sql.AppendLine($"    ?, ?");
            sql.AppendLine($")");

            var parameters = new[]
            {
                Sqlite3Util.CreateParameter(DbType.String, videoid),
                Sqlite3Util.CreateParameter(DbType.Int64, date.Ticks),
            };

            await command.ExecuteNonQueryAsync(sql.ToString(), parameters);
        }

        public static async Task DeleteNicoTemporary(this SQLiteControl command, string videoid)
        {
            var sql = new StringBuilder();

            sql.Clear();
            sql.AppendLine($"DELETE FROM t_nico_temporary WHERE video_id = ?");

            var parameters = new[]
            {
                Sqlite3Util.CreateParameter(DbType.String, videoid),
            };

            await command.ExecuteNonQueryAsync(sql.ToString(), parameters);
        }

        public static async Task<IEnumerable<VNicoHistory>> SelectNicoHistory(this SQLiteControl command)
        {
            var sql = new StringBuilder();

            sql.Clear();
            sql.AppendLine($"SELECT video_id, tick, cnt FROM v_nico_history ORDER BY tick DESC");

            using (var reader = await command.ExecuteReaderAsync(sql.ToString()))
            {
                return await reader.GetRows(r => new VNicoHistory(r));
            }
        }

        public static async Task MergeNicoHistory(this SQLiteControl command, string videoid)
        {
            var sql = new StringBuilder();

            sql.Clear();
            sql.AppendLine($"INSERT OR REPLACE INTO t_nico_history (");
            sql.AppendLine($"    video_id, tick");
            sql.AppendLine($")");
            sql.AppendLine($"VALUES (");
            sql.AppendLine($"    ?, ?");
            sql.AppendLine($")");

            var parameters = new[]
            {
                Sqlite3Util.CreateParameter(DbType.String, videoid),
                Sqlite3Util.CreateParameter(DbType.Int64, DateTime.Now.Ticks),
            };

            await command.ExecuteNonQueryAsync(sql.ToString(), parameters);
        }

        public static async Task DeleteNicoHistory(this SQLiteControl command, string videoid)
        {
            var sql = new StringBuilder();

            sql.Clear();
            sql.AppendLine($"DELETE FROM t_nico_history WHERE video_id = ?");

            var parameters = new[]
            {
                Sqlite3Util.CreateParameter(DbType.String, videoid),
            };

            await command.ExecuteNonQueryAsync(sql.ToString(), parameters);
        }

        public static async Task<IEnumerable<TNicoFavorite>> SelectNicoFavorite(this SQLiteControl command)
        {
            var sql = new StringBuilder();

            sql.Clear();
            sql.AppendLine($"SELECT mylist_id, tick FROM t_nico_favorite ORDER BY tick DESC");

            using (var reader = await command.ExecuteReaderAsync(sql.ToString()))
            {
                return await reader.GetRows(r => new TNicoFavorite(r));
            }
        }

        public static async Task MergeNicoFavorite(this SQLiteControl command, string mylistid, DateTime date)
        {
            var sql = new StringBuilder();

            sql.Clear();
            sql.AppendLine($"INSERT OR REPLACE INTO t_nico_favorite (");
            sql.AppendLine($"    mylist_id, tick");
            sql.AppendLine($")");
            sql.AppendLine($"VALUES (");
            sql.AppendLine($"    ?, ?");
            sql.AppendLine($")");

            var parameters = new[]
            {
                Sqlite3Util.CreateParameter(DbType.String, mylistid),
                Sqlite3Util.CreateParameter(DbType.Int64, date.Ticks),
            };

            await command.ExecuteNonQueryAsync(sql.ToString(), parameters);
        }

        public static async Task DeleteNicoFavorite(this SQLiteControl command, string mylistid)
        {
            var sql = new StringBuilder();

            sql.Clear();
            sql.AppendLine($"DELETE FROM t_nico_favorite WHERE mylist_id = ?");

            var parameters = new[]
            {
                Sqlite3Util.CreateParameter(DbType.String, mylistid),
            };

            await command.ExecuteNonQueryAsync(sql.ToString(), parameters);
        }


    }
}
