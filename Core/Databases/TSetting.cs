using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mov.Standard.Core.Databases
{
    public class TSetting
    {
        public TSetting(string group, string key, string value)
        {
            Group = group;
            Key = key;
            Value = value;
        }

        public TSetting(DbDataReader reader) : this(reader.GetString(0), reader.GetString(1), reader.GetString(2))
        {

        }

        public string Group { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }
    }
}
