using Mov.Standard.Core.Databases;
using Mov.Standard.Models;
using My.Core;
using My.Core.Databases.SQLite;
using My.Wpf.Core;
using StatefulModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mov.Standard.Nico.Models
{
    public class NicoVideoHistoryModel : BindableBase
    {
        private NicoVideoHistoryModel()
        {
            BeginDate = DateTime.Now;
        }

        public static NicoVideoHistoryModel Instance { get; private set; } = new NicoVideoHistoryModel();

        private DateTime BeginDate { get; }

        public ObservableSynchronizedCollection<TVideoHistory> Histories
        {
            get => _Histories;
            set => SetProperty(ref _Histories, value);
        }
        private ObservableSynchronizedCollection<TVideoHistory> _Histories;

        public async Task Initialize(SQLiteControl command)
        {
            Histories = new ObservableSynchronizedCollection<TVideoHistory>(
                await command.SelectTVideoHistoryAsync()
            );
        }

        public async Task AddVideoHistory(string url, VideoStatus status)
        {
            var id = NicoUtil.ToVideoId(url);

            using (var command = await DbUtil.GetControl())
            {
                await command.BeginTransaction();
                Histories.Add(await command.InsertTVideoHistoryAsync(id, status));
                await command.Commit();
            }
        }

        public bool IsSee(string url)
        {
            return Histories.Any(history => history.VideoId == NicoUtil.ToVideoId(url) && history.Status == VideoStatus.See);
        }

        public bool IsNew(string url)
        {
            return Histories.Any(history => BeginDate < history.Date && history.VideoId == NicoUtil.ToVideoId(url) && history.Status == VideoStatus.New);
        }

        public bool Exists(string url)
        {
            return Histories.Any(history => history.VideoId == NicoUtil.ToVideoId(url));
        }
    }
}
