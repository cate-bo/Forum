using Forum.Commands;
using Forum.Models;
using Forum.Views;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Forum.ViewModels
{
    public class TopicListViewModel : BaseViewModel
    {
        public RelayCommandWithParameter SelectThread { get; set; }
        public ObservableCollection<Models.Thread> Threads { get; set; } = new ObservableCollection<Models.Thread>();
        private ForumContext _context;
        private TopicViewModel _parent;
        private Topic CurrentTopic { get; set; }

        public TopicListViewModel(Topic topic, ForumContext context, TopicViewModel parent)
        {
            _parent = parent;
            View = new TopicListView(this);
            SelectThread = new RelayCommandWithParameter(ViewSelectedThread);
            _context = context;

            CurrentTopic = topic;

            var threads = _context.Thread
                .Where(a => a.TopicId == CurrentTopic.TopicId);
            foreach (Models.Thread thread in threads)
            {
                Threads.Add(thread);
            }
        }

        

        private void ViewSelectedThread(object thread)
        {
            _parent.ViewSelectedThread((Models.Thread)thread);
        }
    }
}
