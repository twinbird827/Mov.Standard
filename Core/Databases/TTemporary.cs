using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mov.Standard.Core.Databases
{
    public class TTemporary
    {
        public TTemporary(DbDataReader reader) : this(reader.GetInt64(0), reader.GetString(1))
        {

        }

        public TTemporary(long tick, string video)
        {
            Tick = tick;
            VideoId = video;
        }

        public long Tick
        {
            get => Date.Ticks;
            set => Date = new DateTime(value);
        }

        public DateTime Date { get; set; }

        public string VideoId { get; set; }
    }
}
