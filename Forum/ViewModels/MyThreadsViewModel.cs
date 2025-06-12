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

namespace Forum.ViewModels
{
    public class MyThreadsViewModel : BaseViewModel
    {
        private ForumContext _context;
        private User _loggedInUser;
        private MainViewModel _parent;
        public ObservableCollection<Models.Thread> Threads { get; set; }
        public RelayCommandWithParameter ClickThread { get; set; }
        public MyThreadsViewModel(ForumContext context, User loggedInUser, MainViewModel parent)
        {
            _context = context;
            _loggedInUser = loggedInUser;
            _parent = parent;
            Threads = new ObservableCollection<Models.Thread>();
            ClickThread = new RelayCommandWithParameter(ViewThread);

            View = new MyThreadsView(this);

            var threads = _context.Thread.Where(a => a.Op.PosterId == _loggedInUser.UserId).Include(b => b.Op);
            foreach(Models.Thread thread in threads)
            {
                Threads.Add(thread);
            }
        }

        private void ViewThread(object thread)
        {
            _parent.Viewthread((Models.Thread)thread);
        }
    }
}
