using My.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mov.Standard.Core.Database
{
    public class TNicoFavorite
    {
        public TNicoFavorite(string id, long tick)
        {
            MylistId = id;
            Tick = tick;
        }

        public TNicoFavorite(DbDataReader reader) : this(reader.Get<string>(0), reader.Get<long>(1))
        {

        }

        public string MylistId { get; set; }

        public long Tick { get => Date.Ticks; set => Date = new DateTime(value); }

        public DateTime Date { get; set; }

        public override bool Equals(object obj)
        {
            var target = obj as TNicoFavorite;
            if (target != null)
            {
                return MylistId == target.MylistId;
            }
            else
            {
                return base.Equals(obj);
            }
        }

        public override int GetHashCode()
        {
            return MylistId.GetHashCode();
        }

    }
}
