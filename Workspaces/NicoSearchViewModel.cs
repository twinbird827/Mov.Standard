using Mov.Standard.Core;
using Mov.Standard.Core.Nico;
using Mov.Standard.Windows;
using Mov.Standard.Workspaces.Base;
using My.Core;
using My.Wpf.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mov.Standard.Workspaces
{
    public class NicoSearchViewModel : WorkspaceViewModel
    {
        public override string Title => MenuType.NicoSearch.GetLabel();

        public NicoSearchViewModel()
        {
            AddLoadTask(() =>
            {
                Videos = new ObservableCollection<NicoVideoViewModel>();

                Genres = new ObservableCollection<ComboboxItemModel>(
                    NicoUtil.Combos
                        .Where(combo => combo.Group == "rank_genre")
                );
                SelectedGenre = Genres.FirstOrDefault(genre => genre.Value == AppSetting.Instance.NicoRankingGenre) ?? Genres.First();

                Periods = new ObservableCollection<ComboboxItemModel>(
                    NicoUtil.Combos
                        .Where(combo => combo.Group == "rank_period")
                );
                SelectedPeriod = Periods.FirstOrDefault(period => period.Value == AppSetting.Instance.NicoRankingPeriod) ?? Periods.First();

                _initialize = false;

                Reload();
            });

            Disposed += (sender, e) =>
            {
                AppSetting.Instance.NicoRankingGenre = SelectedGenre.Value;
                AppSetting.Instance.NicoRankingPeriod = SelectedPeriod.Value;
                AppSetting.Instance.Save();

                Videos.ForEach(x => x.Dispose());
                Videos.ClearOnUI();
            };
        }

        public ObservableCollection<ComboboxItemModel> Genres
        {
            get => _Genres;
            set => SetProperty(ref _Genres, value);
        }
        public ObservableCollection<ComboboxItemModel> _Genres;

        public ComboboxItemModel SelectedGenre
        {
            get => _SelectedGenre;
            set { if (SetProperty(ref _SelectedGenre, value)) Reload(); }
        }
        public ComboboxItemModel _SelectedGenre;

        public ObservableCollection<ComboboxItemModel> Periods
        {
            get => _Periods;
            set => SetProperty(ref _Periods, value);
        }
        public ObservableCollection<ComboboxItemModel> _Periods;

        public ComboboxItemModel SelectedPeriod
        {
            get => _SelectedPeriod;
            set { if (SetProperty(ref _SelectedPeriod, value)) Reload(); }
        }
        public ComboboxItemModel _SelectedPeriod;

        public ObservableCollection<NicoVideoViewModel> Videos
        {
            get => _Videos;
            set => SetProperty(ref _Videos, value);
        }
        public ObservableCollection<NicoVideoViewModel> _Videos;

        private bool _initialize = true;

        private async void Reload()
        {
            if (_initialize) return;

            Videos.Clear();
            await NicoUtil
                .GetRanking(SelectedGenre.Value, "all", SelectedPeriod.Value)
                .ContinueWith(x => Videos.AddRangeOnUI(x.Result.Select(video => new NicoVideoViewModel(video, true))));
        }
    }
}
