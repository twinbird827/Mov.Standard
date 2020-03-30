using Mov.Standard.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mov.Standard.Core.Databases
{
    public class TVideoHistory
    {
        public TVideoHistory(long tick, string video, int status)
        {
            Tick = tick;
            VideoId = video;
            Status = (VideoStatus)status;
        }

        public TVideoHistory(DbDataReader reader) : this(reader.GetInt64(0), reader.GetString(1), reader.GetInt32(2))
        {

        }

        public long Tick
        {
            get => Date.Ticks;
            set => Date = new DateTime(value);
        }

        public DateTime Date { get; set; }

        public string VideoId { get; set; }

        public VideoStatus Status { get; set; }
    }
}
