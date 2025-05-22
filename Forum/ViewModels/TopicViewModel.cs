using Forum.Models;
using Forum.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Forum.ViewModels
{
    public class TopicViewModel : BaseViewModel
    {
        public Topic CurrentTopic { get; set; }
        public ObservableCollection<Models.Thread> Threads { get; set; } = new ObservableCollection<Models.Thread>();
        private ForumContext _context;
        public TopicViewModel(Topic topic, ForumContext context)
        {
            View = new TopicView(this);
            CurrentTopic = topic;
            _context = context;
            MessageBox.Show("amogus");
            foreach (Models.Thread thread in CurrentTopic.Thread) //TODO thread in database isnt in Currenttopic.Thread idk why
            {
                Threads.Add(thread);
                MessageBox.Show(thread.Title);
            }
        }
    }
}
