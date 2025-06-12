using Forum.Commands;
using Forum.Models;
using Forum.Views;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Forum.ViewModels
{
    public class FollowViewModel : BaseViewModel
    {
        public ObservableCollection<Models.Thread> Threads { get; set; }
        private ForumContext _context;
        private User _user;
        private MainViewModel _parent;

        public RelayCommandWithParameter ClickThread { get; set; }
        public RelayCommandWithParameter ClickUnfollow { get; set; }
        public FollowViewModel(ForumContext context, User loggedInUser, MainViewModel parent)
        {
            View = new FollowView(this);
            _context = context;
            _user = loggedInUser;
            _parent = parent;
            Threads = new ObservableCollection<Models.Thread>();
            ClickThread = new RelayCommandWithParameter(ViewThread);
            ClickUnfollow = new RelayCommandWithParameter(UnfollowThread);

            var threads = _context.User.Where(a => a.UserId == _user.UserId)
                .Include(b => b.Thread)
                .ThenInclude(c => c.Op)
                .ThenInclude(d => d.Poster)
                .First().Thread;
            

            foreach(Models.Thread thread in threads)
            {
                Threads.Add(thread);
            }
        }

        private void ViewThread(object thread)
        {
            _parent.Viewthread((Models.Thread)thread);
        }

        private void UnfollowThread(object thread)
        {
            //TODO fix this in view
            MessageBox.Show(((Models.Thread)thread).Title);
            _context.Thread.Remove((Models.Thread) thread);
            _context.SaveChanges();
            Threads.Remove((Models.Thread) thread);
        }
    }
}
