using Forum.Commands;
using Forum.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Forum.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly ForumContext _context;
        private MainWindow _window;

        public ObservableCollection<Topic> Topics { get; set; } = new ObservableCollection<Topic>();

        public RelayCommandWithParameter SelectTopic { get; set; }

        public MainViewModel(ForumContext context)
        {
            //Command shit
            SelectTopic = new RelayCommandWithParameter(ViewTopic);

            
            _context = context;
            _window = new MainWindow(this, context);
            _window.Show();
            foreach(Topic topic in context.Topic)
            {
                Topics.Add(topic);
            }

            
        }

        private void ViewTopic(object thing)
        {
            MessageBox.Show(((Topic)thing).Title);
        }
    }
}
