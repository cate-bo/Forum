using Forum.Commands;
using Forum.Models;
using Forum.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Forum.ViewModels
{
    public class TopicViewModel : BaseViewModel
    {
        public Topic CurrentTopic { get; set; }
        private ForumContext _context;
        private BaseViewModel _currentContent;
        private TopicView _view;
        public RelayCommand ListTopics { get; set; }
        public TopicViewModel(Topic topic, ForumContext context)
        {
            ListTopics = new RelayCommand(ShowTopicList);
            View = new TopicView(this);
            CurrentTopic = topic;
            _context = context;
            _view = (TopicView)View;
            _currentContent = this;
            
            ShowTopicList();
        }

        private void ShowTopicList()
        {
            _view.grd_topicView.Children.Remove(_currentContent.View);
            _currentContent = new ThreadListViewModel(CurrentTopic, _context, this);
            _view.grd_topicView.Children.Add(_currentContent.View);

            Grid.SetColumn(_currentContent.View, 0);
            Grid.SetRow(_currentContent.View, 1);
        }

        public void ViewSelectedThread(Models.Thread thread)
        {
            _view.grd_topicView.Children.Remove(_currentContent.View);
            _currentContent = new ThreadViewModel(thread, _context);
            _view.grd_topicView.Children.Add(_currentContent.View);

            Grid.SetColumn(_currentContent.View, 0);
            Grid.SetRow(_currentContent.View, 1);
        }
    }
}
