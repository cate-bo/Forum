using Forum.Commands;
using Forum.Models;
using Forum.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Forum.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly ForumContext _context;
        private MainWindow _window;
        //currently displayed UserControl
        private BaseViewModel _currentMainContent;

        public ObservableCollection<Topic> Topics { get; set; } = new ObservableCollection<Topic>();

        public RelayCommandWithParameter SelectTopic { get; set; }
        public RelayCommand GoHome { get; set; }

        public MainViewModel(ForumContext context)
        {
            //Command shit
            SelectTopic = new RelayCommandWithParameter(ViewSelectedTopic);
            GoHome = new RelayCommand(Home);

            _currentMainContent = this;
            _context = context;
            _window = new MainWindow(this, context);
            
            _window.Show();
            foreach(Topic topic in context.Topic)
            {
                Topics.Add(topic);
            }

            Home();
        }

        private void Home()
        {
            _window.grd_main.Children.Remove(_currentMainContent.View);
            View = new MainView(this);
            _currentMainContent = this;

            _window.grd_main.Children.Add(View);
            Grid.SetColumn(View, 0);
            Grid.SetRow(View, 1);
        }

        private void ViewSelectedTopic(object thing)
        {
            _window.grd_main.Children.Remove(_currentMainContent.View);
            _currentMainContent = new TopicViewModel((Topic) thing, _context);
            _window.grd_main.Children.Add(_currentMainContent.View);
            Grid.SetColumn(_currentMainContent.View, 0);
            Grid.SetRow(_currentMainContent.View, 1);
        }
    }
}
