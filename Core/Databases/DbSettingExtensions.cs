using My.Core.Databases.SQLite;
using My.Core.Extensions;
using System;
using System.Collections.Generic;
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

    }
}
