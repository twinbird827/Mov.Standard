using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mov.Standard.Core.Databases
{
    public class TFavorite
    {
        public TFavorite(string mylist, long tick)
        {
            Mylist = mylist;
            Tick = tick;
        }

        public TFavorite(DbDataReader reader) : this(reader.GetString(0), reader.GetInt64(1))
        {

        }

        public string Mylist { get; set; }

        public long Tick
        {
            get => Date.Ticks;
            set => Date = new DateTime(value);
        }

        public DateTime Date { get; set; }

    }
}
