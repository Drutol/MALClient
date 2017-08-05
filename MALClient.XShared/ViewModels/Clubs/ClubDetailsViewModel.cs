using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MALClient.Models.Enums;
using MALClient.Models.Models;
using MALClient.Models.Models.Anime;
using MALClient.Models.Models.MalSpecific;
using MALClient.XShared.Comm.MagicalRawQueries.Clubs;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils;

namespace MALClient.XShared.ViewModels.Clubs
{
    public class ClubDetailsViewModel : ViewModelBase
    {
        private bool _loading;
        private ClubDetailsPageNavArgs _lastArgs;
        private MalClubDetails _details;
        private bool _loadingComments;
        private string _commentInput;
        private ObservableCollection<MalClubComment> _comments;
        private int _currentCommentsPage = 1;
        private bool _moreCommentsButtonVisibility;
        private ICommand _navigateUserCommand;
        private ICommand _deleteCommentCommand;
        private ObservableCollection<MalUser> _members;
        private bool _loadMoreUsersButtonVisibility;
        private ICommand _navigateAnimeDetailsCommand;
        private ICommand _navigateMangaDetailsCommand;

        public bool Loading
        {
            get { return _loading; }
            set
            {
                _loading = value;
                RaisePropertyChanged();
            }
        }

        public bool LoadingComments
        {
            get { return _loadingComments; }
            set
            {
                _loadingComments = value;
                RaisePropertyChanged();
            }
        }

        public string CommentInput
        {
            get { return _commentInput; }
            set
            {
                _commentInput = value;
                RaisePropertyChanged();
            }
        }

        public bool MoreCommentsButtonVisibility
        {
            get { return _moreCommentsButtonVisibility; }
            set
            {
                _moreCommentsButtonVisibility = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<MalClubComment> Comments
        {
            get { return _comments; }
            set
            {
                _comments = value;
                RaisePropertyChanged();
                RaisePropertyChanged(() => NoCommentsEmptyNoticeVisibility);
            }
        }

        public ObservableCollection<MalUser> Members
        {
            get { return _members; }
            set
            {
                _members = value;
                RaisePropertyChanged();
            }
        }

        public bool LoadMoreUsersButtonVisibility
        {
            get { return _loadMoreUsersButtonVisibility; }
            set
            {
                _loadMoreUsersButtonVisibility = value;
                RaisePropertyChanged();
            }
        }

        public bool NoCommentsEmptyNoticeVisibility => !Comments?.Any() ?? true;
        public bool NoAnimeRelationsEmptyNoticeVisibility => !AnimeRelations?.Any() ?? true;
        public bool NoMangaRelationsEmptyNoticeVisibility => !AnimeRelations?.Any() ?? true;

        public MalClubDetails Details
        {
            get { return _details; }
            set
            {
                _details = value;
                
                _currentCommentsPage = 1;
                MoreCommentsButtonVisibility = value?.RecentComments?.Any() ?? false;
                LoadMoreUsersButtonVisibility = value != null && value.MembersPeek.Count > 4;
                RaisePropertyChanged();
                if (value == null)
                {
                    Members = null;
                    Comments = null;
                    return;
                }
                Comments = new ObservableCollection<MalClubComment>(value.RecentComments);
                Comments.CollectionChanged += CommentsOnCollectionChanged;

                Members = new ObservableCollection<MalUser>(value.MembersPeek);


                RaisePropertyChanged(() => GeneralInfo);
                RaisePropertyChanged(() => Officers);
                RaisePropertyChanged(() => AnimeRelations);
                RaisePropertyChanged(() => MangaRelations);
                RaisePropertyChanged(() => CharacterRelations);
                RaisePropertyChanged(() => NoAnimeRelationsEmptyNoticeVisibility);
                RaisePropertyChanged(() => NoMangaRelationsEmptyNoticeVisibility);

            }
        }


        private void CommentsOnCollectionChanged(object sender,
            NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            switch (notifyCollectionChangedEventArgs.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    Details.RecentComments.Add(notifyCollectionChangedEventArgs.NewItems[0] as MalClubComment);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    Details.RecentComments.Remove(notifyCollectionChangedEventArgs.OldItems[0] as MalClubComment);
                    break;
            }
            RaisePropertyChanged(() => NoCommentsEmptyNoticeVisibility);
        }


        //Workaround for xaml being unable to bind to values of value type tuples
        public List<Tuple<string, string>> GeneralInfo => Details?.GeneralInfo
            .Select(tuple => new Tuple<string, string>(tuple.name, tuple.value)).ToList();

        public List<Tuple<string, string>> Officers => Details?.Officers
            .Select(tuple => new Tuple<string, string>(tuple.role, tuple.user)).ToList();

        public List<Tuple<string, string>> AnimeRelations => Details?.AnimeRelations
            .Select(tuple => new Tuple<string, string>(tuple.title, tuple.id)).ToList();

        public List<Tuple<string, string>> MangaRelations => Details?.MangaRelations
            .Select(tuple => new Tuple<string, string>(tuple.title, tuple.id)).ToList();

        public List<Tuple<string, string>> CharacterRelations => Details?.CharacterRelations
            .Select(tuple => new Tuple<string, string>(tuple.name, tuple.id)).ToList();


        public async void NavigatedTo(ClubDetailsPageNavArgs args)
        {
            if (args.Equals(_lastArgs))
                return;
            Details = null;
            _lastArgs = args;

            Loading = true;
            Details = await MalClubDetailsQuery.GetClubDetails(args.Id);
            Loading = false;

        }

        public async void Reload()
        {
            Loading = true;
            Details = await MalClubDetailsQuery.GetClubDetails(_lastArgs.Id);
            Loading = false;
        }


        public ICommand PostCommentCommand => new RelayCommand(async () =>
        {
            LoadingComments = true;

            if (await MalClubQueries.PostComment(Details.Id, CommentInput))
            {
                Comments.Add(new MalClubComment
                {
                    Content = CommentInput,
                    Date = "Just Now",
                    User = new MalUser
                    {
                        ImgUrl = $"https://myanimelist.cdn-dena.com/images/userimages/{Credentials.Id}.jpg",
                        Name = Credentials.UserName
                    }
                });
            }

            LoadingComments = false;
        });

        public ICommand DeleteCommentCommand => _deleteCommentCommand ?? (_deleteCommentCommand = new RelayCommand<MalClubComment>(async comment =>
        {
            LoadingComments = true;

            if (await MalClubQueries.RemoveComment(Details.Id, comment.Id))
            {
                Comments.Remove(comment);
            }

            LoadingComments = false;
        }));

        public ICommand ReloadCommentsCommand => new RelayCommand(async () =>
        {
            LoadingComments = true;

            var details = await MalClubDetailsQuery.GetClubDetails(_lastArgs.Id, true);

            Details.RecentComments = details.RecentComments;
            Comments = new ObservableCollection<MalClubComment>(Details.RecentComments);
            Comments.CollectionChanged += CommentsOnCollectionChanged;

            LoadingComments = false;
        });

        public ICommand LoadMoreMembersCommand => new RelayCommand(async () =>
        {
            LoadMoreUsersButtonVisibility = false;
            var users = await MalClubDetailsQuery.GetMoreUsers(_lastArgs.Id);

            if(users != null && users.Any())
                  Members = new ObservableCollection<MalUser>(Members.Union(users));


        });

        public ICommand LoadMoreCommentsCommand => new RelayCommand(async () =>
        {
            LoadingComments = true;

            var comments = await MalClubDetailsQuery.GetClubComments(Details.Id, _currentCommentsPage++);
            if (comments != null && comments.Any())
            {
                foreach (var malClubComment in comments)
                {
                    Comments.Add(malClubComment);
                }
            }
            else
            {
                MoreCommentsButtonVisibility = false;
            }

            LoadingComments = false;
        });

        public ICommand NavigateUserCommand => _navigateUserCommand ?? (_navigateUserCommand =
                                                   new RelayCommand<MalUser>(
                                                       user =>
                                                       {
                                                           ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageClubDetails, _lastArgs);
                                                           ViewModelLocator.GeneralMain.Navigate(PageIndex.PageProfile,
                                                               new ProfilePageNavigationArgs
                                                               {
                                                                   AllowBackNavReset = false,
                                                                   TargetUser = user.Name
                                                               });
                                                       }));

        public ICommand NavigateForumCommand => new RelayCommand(() =>
        {
            ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageClubDetails,_lastArgs);
            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageForumIndex,
                new ForumsBoardNavigationArgs(_lastArgs.Id,Details.Name));
        });


        public ICommand NavigateAnimeDetailsCommand
            =>
                _navigateAnimeDetailsCommand ??
                (_navigateAnimeDetailsCommand =
                    new RelayCommand<Tuple<string,string>>(
                        entry =>
                        {
                            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageAnimeDetails,
                                new AnimeDetailsPageNavigationArgs(int.Parse(entry.Item2), entry.Item1, null, null, _lastArgs) { Source = PageIndex.PageClubDetails });
                        }));

        public ICommand NavigateMangaDetailsCommand
            =>
                _navigateMangaDetailsCommand ??
                (_navigateMangaDetailsCommand =
                    new RelayCommand<Tuple<string, string>>(
                        entry =>
                        {
                            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageAnimeDetails,
                                new AnimeDetailsPageNavigationArgs(int.Parse(entry.Item2), entry.Item1, null, null, _lastArgs) { Source = PageIndex.PageClubDetails, AnimeMode = false});
                        }));

        public ICommand LeaveClubCommand => new RelayCommand(async () =>
        {
            Loading = true;
            await MalClubQueries.LeaveClub(Details.Id);
            ViewModelLocator.ClubIndex.MyClubs.Remove(
                ViewModelLocator.ClubIndex.MyClubs.First(entry => entry.Id == Details.Id));
            Reload();
        });

        public ICommand JoinClubCommand => new RelayCommand(async () =>
        {
            Loading = true;
            await MalClubQueries.JoinClub(Details.Id);
            ViewModelLocator.ClubIndex.ReloadMyClubs();
            Reload();
        });

    }
}
