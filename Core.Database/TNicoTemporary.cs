using My.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mov.Standard.Core.Database
{
    public class TNicoTemporary
    {
        public TNicoTemporary(string id, long tick)
        {
            VideoId = id;
            Tick = tick;
        }

        public TNicoTemporary(DbDataReader reader) : this (reader.Get<string>(0), reader.Get<long>(1))
        {

        }

        public string VideoId { get; set; }

        public long Tick { get => Date.Ticks; set => Date = new DateTime(value); }

        public DateTime Date { get; set; }

        public override bool Equals(object obj)
        {
            var target = obj as TNicoTemporary;
            if (target != null)
            {
                return VideoId == target.VideoId;
            }
            else
            {
                return base.Equals(obj);
            }
        }

        public override int GetHashCode()
        {
            return VideoId.GetHashCode();
        }

    }
}
