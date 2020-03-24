using Mov.Standard.Models;
using Mov.Standard.Nico.Components;
using Mov.Standard.Nico.Models;
using My.Core;
using My.Wpf.Core;
using StatefulModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mov.Standard.Nico.Workspaces
{
    public class NicoRankingViewModel : NicoWorkspaceViewModel
    {
        public NicoRankingViewModel()
        {
            Videos = new ObservableSynchronizedCollection<NicoVideoModel>();
            Views = Videos.ToSyncedSynchronizationContextCollection(
                video => new NicoVideoViewModel(video),
                System.Threading.SynchronizationContext.Current
            );

            Loaded += (sender, e) =>
            {
                Genres = new ObservableCollection<ComboboxItemModel>(
                    MovModel.Instance.Combos
                        .Where(combo => combo.Group == "rank_genre")
                );
                SelectedGenre = Genres.First();

                Periods = new ObservableCollection<ComboboxItemModel>(
                    MovModel.Instance.Combos
                        .Where(combo => combo.Group == "rank_period")
                );
                SelectedPeriod = Periods.First();

                _isLoading = false;

                Reload();
            };

            Disposed += (sender, e) =>
            {
                _isLoading = true;

                SelectedGenre = null;
                SelectedPeriod = null;
                Genres.Clear();
                Periods.Clear();
            };
        }

        public override string Title => "Ranking";

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

        public ObservableSynchronizedCollection<NicoVideoModel> Videos
        {
            get => _Videos;
            set => SetProperty(ref _Videos, value);
        }
        public ObservableSynchronizedCollection<NicoVideoModel> _Videos;

        public SynchronizationContextCollection<NicoVideoViewModel> Views
        {
            get { return _Views; }
            set { SetProperty(ref _Views, value); }
        }
        private SynchronizationContextCollection<NicoVideoViewModel> _Views;

        private bool _isLoading = true;

        private async void Reload()
        {
            if (_isLoading)
            {
                return;
            }

            Videos.Clear();
            Videos.AddRange(
                await NicoUtil.GetRanking(SelectedGenre.Value, "all", SelectedPeriod.Value)
            );
        }
    }
}
