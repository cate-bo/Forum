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
        public Post OP { get; set; }
        public string OPsUsername { get; set; }
        public Models.Thread _thread { get; set; }
        public Post ReplyingTo { get; set; }
        public RelayCommand ClickXReply { get; set; }
        private ReplyToDisplayView _replyToDisplayView;
        public ThreadViewModel(Models.Thread thread, ForumContext context)
        {
            _context = context;
            View = new ThreadView(this);
            _thread = thread;

            ClickXReply = new RelayCommand(ResetReplyRecipient);

            _replyToDisplayView = new ReplyToDisplayView(this);
            Grid.SetRow(_replyToDisplayView, 3);
            _replyToDisplayView.Margin = new System.Windows.Thickness(20, 0, 0, 0);

            
            OP = _context.Post.Where(a => a.PostId == _thread.Opid)
                .Include(b => b.Poster)
                .First();

            OPsUsername = OP.Poster.Username;
            //hide textinput stuff when not logged in
            if (!MainViewModel.IsLoggedIn)
            {
                ((ThreadView)View).grd_main.RowDefinitions[3].Height = new System.Windows.GridLength(0);
            }
            RefreshPostList();
            ResetReplyRecipient();
        }

        public void RefreshPostList()
        {
            var posts = _context.Post.Where(a => a.PredecessorId == OP.PostId).Include(b => b.Poster);
            List<Post> _posts = new List<Post>();
            foreach (Post post in posts)
            {
                _posts.Add(post);
            }
            ObservableCollection<TreeViewItem> collection = new ObservableCollection<TreeViewItem>();
            ((ThreadView)View).PostList.ItemsSource = collection;
            foreach (Post post in _posts)
            {
                PostViewModel temp = new PostViewModel(post, this);
                collection.Add(temp.PostTreeViewItem);
                FillReplies(post.PostId, temp.PostTreeViewItem);
            }
        }

        private void FillReplies(int predecessorID, TreeViewItem predecessor)
        {
            var posts = _context.Post.Where(a => a.PredecessorId == predecessorID).Include(b => b.Poster);
            List<Post> _posts = new List<Post>();
            foreach(Post post in posts)
            {
                _posts.Add(post);
            }
            if(_posts.Count > 0)
            {
                ObservableCollection<TreeViewItem> collection = new ObservableCollection<TreeViewItem>();
                predecessor.ItemsSource = collection;
                foreach(Post post in _posts)
                {
                    PostViewModel temp = new PostViewModel(post, this);
                    collection.Add(temp.PostTreeViewItem);
                    FillReplies(post.PostId, temp.PostTreeViewItem);
                }
            }
        }

        private void ChangeReplyRecipient(Post post)
        {
            ReplyingTo = post;
            if(ReplyingTo == OP)
            {
                ((ThreadView)View).grd_main.Children.Remove(_replyToDisplayView);
            }
            else
            {
                ((ThreadView)View).grd_main.Children.Add(_replyToDisplayView);
            }
        }

        private void ResetReplyRecipient()
        {
            ChangeReplyRecipient(OP);
        }
    }
}
