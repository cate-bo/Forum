using Forum.Commands;
using Forum.Models;
using Forum.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Forum.ViewModels
{
    public class ThreadViewModel : BaseViewModel
    {
        private ForumContext _context;
        public PostViewModel OP { get; set; }
        public string OPsUsername { get; set; }
        public Models.Thread CurrentThread { get; set; }
        public ObservableCollection<TreeViewItem> Replies { get; set; }

        private PostViewModel _replyingTo;

        public PostViewModel ReplyingTo
        {
            get { return _replyingTo; }
            set
            {
                _replyingTo = value;
                OnpropertyChanged(nameof(ReplyingTo));
            }
        }

        private string _replyText;

        public string ReplyText
        {
            get { return _replyText; }
            set
            {
                _replyText = value;
                OnpropertyChanged(nameof(ReplyText));
            }
        }

        private string _followButtonContent;

        public string FollowOrUnfollowButtonContent
        {
            get { return _followButtonContent; }
            set
            {
                _followButtonContent = value;
                OnpropertyChanged(nameof(FollowOrUnfollowButtonContent));
            }
        }

        public RelayCommand ClickFollowOrUnfollow { get; set; }

        public RelayCommand ClickPostButton { get; set; }

        public RelayCommand ClickXReply { get; set; }
        private ReplyToDisplayView _replyToDisplayView;
        public ThreadViewModel(Models.Thread thread, ForumContext context)
        {
            _context = context;
            View = new ThreadView(this);
            CurrentThread = thread;

            ReplyText = "";

            ClickPostButton = new RelayCommand(PostReply);
            ClickXReply = new RelayCommand(ResetReplyRecipient);
            ClickFollowOrUnfollow = new RelayCommand(FollowOrUnfollowThread);


            _replyToDisplayView = new ReplyToDisplayView(this);
            Grid.SetRow(_replyToDisplayView, 3);
            _replyToDisplayView.Margin = new System.Windows.Thickness(20, 0, 0, 0);
            _replyToDisplayView.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            
            OP = new PostViewModel(_context.Post.Where(a => a.PostId == CurrentThread.Opid)
                .Include(b => b.Poster)
                .First()
                , this);

            OPsUsername = OP.ThisPost.Poster.Username;
            //hide textinput and follow button when not logged in
            if (!MainViewModel.IsLoggedIn)
            {
                ((ThreadView)View).grd_main.RowDefinitions[3].Height = new System.Windows.GridLength(0);
                ((ThreadView)View).grd_main.Children.Remove(((ThreadView)View).FollowOrUnFollowButton);
            }
            else
            {
                if (MainViewModel.LoggedInUser.Thread.Contains(CurrentThread))
                {
                    FollowOrUnfollowButtonContent = "unfollow";
                }
                else
                {
                    FollowOrUnfollowButtonContent = "follow";
                }
            }
            RefreshPostList();
            ResetReplyRecipient();
        }

        public void RefreshPostList()
        {
            var posts = _context.Post.Where(a => a.PredecessorId == OP.ThisPost.PostId).Include(b => b.Poster);
            List<Post> _posts = new List<Post>();
            foreach (Post post in posts)
            {
                _posts.Add(post);
            }
            Replies = new ObservableCollection<TreeViewItem>();
            ((ThreadView)View).PostList.ItemsSource = Replies;
            foreach (Post post in _posts)
            {
                PostViewModel temp = new PostViewModel(post, this);
                Replies.Add(temp.PostTreeViewItem);
                FillReplies(post.PostId, temp);
            }
        }

        private void FillReplies(int predecessorID, PostViewModel predecessor)
        {
            var posts = _context.Post.Where(a => a.PredecessorId == predecessorID).Include(b => b.Poster);
            List<Post> _posts = new List<Post>();
            foreach(Post post in posts)
            {
                _posts.Add(post);
            }
            if(_posts.Count > 0)
            {
                foreach(Post post in _posts)
                {
                    PostViewModel temp = new PostViewModel(post, this);
                    predecessor.Replies.Add(temp.PostTreeViewItem);
                    FillReplies(post.PostId, temp);
                }
            }
        }

        public void ChangeReplyRecipient(PostViewModel post)
        {
            ReplyingTo = post;
            if(ReplyingTo == OP)
            {
                ((ThreadView)View).grd_main.Children.Remove(_replyToDisplayView);
            }
            else
            {
                if (!((ThreadView)View).grd_main.Children.Contains(_replyToDisplayView))
                {
                    ((ThreadView)View).grd_main.Children.Add(_replyToDisplayView);
                }
            }
        }

        private void ResetReplyRecipient()
        {
            ChangeReplyRecipient(OP);
        }

        private void PostReply()
        {
            if(ReplyText.Length > 0)
            {
                Post reply = new Post();
                reply.Poster = MainViewModel.LoggedInUser;
                reply.Predecessor = ReplyingTo.ThisPost;
                reply.Text = ReplyText;
                _context.Add(reply);
                _context.SaveChanges();
                ReplyText = "";
                PostViewModel temp = new PostViewModel(reply, this);
                if(ReplyingTo == OP)
                {
                    Replies.Add(temp.PostTreeViewItem);
                }
                else
                {
                    ReplyingTo.Replies.Add(temp.PostTreeViewItem);
                }
                ResetReplyRecipient();
            }
        }

        private void FollowOrUnfollowThread()
        {
            if (MainViewModel.LoggedInUser.Thread.Contains(CurrentThread))
            {
                MainViewModel.LoggedInUser.Thread.Remove(CurrentThread);
                _context.SaveChanges();
                FollowOrUnfollowButtonContent = "follow";
            }
            else
            {
                MainViewModel.LoggedInUser.Thread.Add(CurrentThread);
                _context.SaveChanges();
                FollowOrUnfollowButtonContent = "unfollow";
            }
        }
    }
}
