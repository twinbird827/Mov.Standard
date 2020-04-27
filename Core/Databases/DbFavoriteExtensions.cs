using Mov.Standard.Nico.Models;
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
    public static class DbFavoriteExtensions
    {
        public static async Task<int> InsertTFavoriteAsync(this SQLiteControl command, NicoMylistModel mylist)
        {
            var sql = new StringBuilder();

            sql.Clear();
            sql.AppendLine($"INSERT INTO t_favorite (");
            sql.AppendLine($"   mylist, tick");
            sql.AppendLine($")");
            sql.AppendLine($"VALUES (");
            sql.AppendLine($"   ?, ?");
            sql.AppendLine($")");

            var parameters = new[]
            {
                Sqlite3Util.CreateParameter(DbType.String, mylist.MylistId),
                Sqlite3Util.CreateParameter(DbType.Int64, mylist.ConfirmDate.Ticks),
            };

            return await command.ExecuteNonQueryAsync(sql.ToString(), parameters);
        }

        public static async Task<int> DeleteTFavoriteAsync(this SQLiteControl command)
        {
            var sql = new StringBuilder();

            sql.Clear();
            sql.AppendLine($"DELETE FROM t_favorite");

            return await command.ExecuteNonQueryAsync(sql.ToString());
        }

        public static async Task<int> DeleteTFavoriteAsync(this SQLiteControl command, string mylist)
        {
            var sql = new StringBuilder();

            sql.Clear();
            sql.AppendLine($"DELETE FROM t_favorite");
            sql.AppendLine($"WHERE  mylist = ?");

            var parameters = new[]
            {
                Sqlite3Util.CreateParameter(DbType.String, mylist),
            };

            return await command.ExecuteNonQueryAsync(sql.ToString(), parameters);
        }

        public static async Task<IEnumerable<TFavorite>> SelectTFavoriteAsync(this SQLiteControl command)
        {
            var sql = new StringBuilder();

            sql.Clear();
            sql.AppendLine($"SELECT mylist, tick");
            sql.AppendLine($"FROM   t_favorite");

            using (var reader = await command.ExecuteReaderAsync(sql.ToString()))
            {
                return await reader.GetRows(r => new TFavorite(r));
            }
        }

    }
}
