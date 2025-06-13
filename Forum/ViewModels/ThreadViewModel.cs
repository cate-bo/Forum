using Forum.Models;
using Forum.Views;
using Microsoft.EntityFrameworkCore;
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
        public ThreadViewModel(Models.Thread thread, ForumContext context)
        {
            _context = context;
            View = new ThreadView(this);
            _thread = thread;

            
            OP = _context.Post.Where(a => a.PostId == _thread.Opid)
                .Include(b => b.Poster)
                .First();

            OPsUsername = OP.Poster.Username;
            var posts = _context.Post.Where(a => a.PredecessorId == OP.PostId);
            //hide textinput stuff when not logged in
            if (!MainViewModel.IsLoggedIn)
            {
                ((ThreadView)View).grd_main.RowDefinitions[3].Height = new System.Windows.GridLength(0);
            }
        }

        public void RefreshPostList(int predecessorID)
        {
            var posts = _context.Post.Where(a => a.PredecessorId == predecessorID);
            ObservableCollection<TreeViewItem> collection = new ObservableCollection<TreeViewItem>();
            ((ThreadView)View).PostList.ItemsSource = collection;
            foreach (Post post in posts)
            {
                PostViewModel temp = new PostViewModel(post, this);
                collection.Add(temp.PostTreeViewItem);
                //TODO fill replies
            }
        }

        private void FillReplies(int predecessorID, TreeViewItem predecessor)
        {
            //TODO
        }
    }
}
