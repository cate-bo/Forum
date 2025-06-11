using Forum.Models;
using Forum.Views;
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
        public MyThreadsViewModel(ForumContext context, User loggedInUser, MainViewModel parent)
        {
            _context = context;
            _loggedInUser = loggedInUser;
            _parent = parent;

            View = new MyThreadsView(this);
        }
    }
}
