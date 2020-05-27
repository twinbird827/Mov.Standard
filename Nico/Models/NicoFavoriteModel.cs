using Mov.Standard.Core.Databases;
using My.Core;
using My.Core.Databases.SQLite;
using My.Wpf.Core;
using StatefulModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mov.Standard.Nico.Models
{
    public class NicoFavoriteModel : BindableBase
    {
        private NicoFavoriteModel()
        {

        }

        public static NicoFavoriteModel Instance { get; private set; } = new NicoFavoriteModel();

        public ObservableSynchronizedCollection<NicoMylistModel> Mylists
        {
            get => _Mylists;
            set => SetProperty(ref _Mylists, value);
        }
        private ObservableSynchronizedCollection<NicoMylistModel> _Mylists;

        public IntervalTimer Timer { get; private set; }

        public async Task Initialize(SQLiteControl command)
        {
            var favorites = await command.SelectTFavoriteAsync();
            var mylists = await favorites.Select(favorite => NicoMylistModel.CreateAsync(favorite, "0")).WhenAll();
            Mylists = new ObservableSynchronizedCollection<NicoMylistModel>(mylists);

            Timer = new IntervalTimer();
            Timer.Interval = TimeSpan.FromMinutes(5);
            Timer.Add(Timer_Tick);
            Timer.Start();
        }

        private async Task Timer_Tick()
        {
            foreach (var mylist in Mylists)
            {
                await mylist.ReloadVideos();
            }

            var deletes = Mylists
                .Where(mylist => !mylist.Videos.Any())
                .Select(mylist => mylist.MylistId)
                .ToArray();

            foreach (var delete in deletes)
            {
                Mylists.Remove(Mylists.First(mylist => mylist.MylistId == delete));
            }

            foreach (var video in Mylists.SelectMany(mylist => mylist.Videos
                    .Where(v => mylist.ConfirmDate < v.StartTime)
                    .Where(v => !NicoTemporaryModel.Instance.Videos.Any(tmp => tmp.VideoId == v.VideoId))
                ))
            {
                await NicoUtil.AddVideo(video);
            }

            foreach (var mylist in Mylists)
            {
                mylist.ConfirmDate = mylist.Videos.MaxOrDefault(video => video.StartTime, DateTime.Now);
            }

            using (var command = await DbUtil.GetControl())
            {
                await command.BeginTransaction();
                await command.DeleteTFavoriteAsync();
                foreach (var mylist in Mylists)
                {
                    await command.InsertTFavoriteAsync(mylist);
                }
                await command.Commit();
            }
        }

        public async Task AddFavorite(string url)
        {
            var id = NicoUtil.ToNicolistId(url);
            if (!Exists(id))
            {
                var mylist = await NicoMylistModel.CreateAsync(id, "0");

                Mylists.Add(mylist);

                using (var command = await DbUtil.GetControl())
                {
                    await command.BeginTransaction();
                    await command.InsertTFavoriteAsync(mylist);
                    await command.Commit();
                }
            }
        }

        public async Task DeleteFavorite(string url)
        {
            var id = NicoUtil.ToNicolistId(url);
            if (Exists(id))
            {
                Mylists.Remove(Mylists.First(mylist => mylist.MylistId == id));

                using (var command = await DbUtil.GetControl())
                {
                    await command.BeginTransaction();
                    await command.DeleteTFavoriteAsync(id);
                    await command.Commit();
                }
            }
        }

        public bool Exists(string url)
        {
            return Mylists.Any(mylist => mylist.MylistId == NicoUtil.ToNicolistId(url));
        }
    }
}
