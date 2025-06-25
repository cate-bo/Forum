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
    public class ThreadListViewModel : BaseViewModel
    {
        public RelayCommandWithParameter SelectThread { get; set; }
        public ObservableCollection<Models.Thread> Threads { get; set; } = new ObservableCollection<Models.Thread>();
        private ForumContext _context;
        private TopicViewModel _parent;
        private Topic CurrentTopic { get; set; }
        public RelayCommand ClickNewThreadButton { get; set; }

        //thread creation stuff
        private string _titleInput;

        public string TitleInput
        {
            get { return _titleInput; }
            set
            {
                _titleInput = value;
                OnpropertyChanged(nameof(TitleInput));
            }
        }

        private string _descriptionInput;

        public string DescriptionInput
        {
            get { return _descriptionInput; }
            set
            {
                _descriptionInput = value;
                OnpropertyChanged(nameof(DescriptionInput));
            }
        }
        public RelayCommand ClickCreateThread { get; set; }

        private string _threadCreationErrormessage;

        public string ThreadCreationErrormessage
        {
            get { return _threadCreationErrormessage; }
            set
            {
                _threadCreationErrormessage = value;
                OnpropertyChanged(nameof(ThreadCreationErrormessage));
            }
        }


        public ThreadListViewModel(Topic topic, ForumContext context, TopicViewModel parent)
        {
            _parent = parent;
            View = new ThreadListView(this);
            SelectThread = new RelayCommandWithParameter(ViewSelectedThread);
            _context = context;
            ClickNewThreadButton = new RelayCommand(OpenCreateThreadWindow);

            CurrentTopic = topic;

            RefreshThreadList();

            //treadcreationwindow
            MainViewModel.CreateThreadWindow.Closing += CreateThreadWindow_Closing;
            TitleInput = "";
            DescriptionInput = "";
            ThreadCreationErrormessage = "";
            ClickCreateThread = new RelayCommand(AttemtThreadCreation);
            //this has to after initializing relaycommand
            MainViewModel.CreateThreadWindow.DataContext = this;


            if (!MainViewModel.IsLoggedIn)
            {
                ((ThreadListView)View).grd_main.Children.Remove(((ThreadListView)View).NewThreadButton);
            }
        }

        private void RefreshThreadList()
        {
            var threads = _context.Thread
                .Where(a => a.TopicId == CurrentTopic.TopicId)
                .Include(b => b.Op);
            foreach (Models.Thread thread in threads)
            {
                Threads.Add(thread);
            }
        }

        private void CreateThreadWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!MainViewModel.ClosingApp)
            {
                e.Cancel = true;
                TitleInput = "";
                DescriptionInput = "";
                ThreadCreationErrormessage = "";
                MainViewModel.CreateThreadWindow.Hide();
            }
        }

        private void ViewSelectedThread(object thread)
        {
            _parent.ViewSelectedThread((Models.Thread)thread);
        }

        private void AttemtThreadCreation()
        {
            if(TitleInput.Length < 1)
            {
                ThreadCreationErrormessage = "title cannot be empty";
            }
            else if(DescriptionInput.Length < 1)
            {
                ThreadCreationErrormessage = "description cannot be empty";
            }
            else
            {
                Post tempPost = new Post();
                Models.Thread tempThread = new Models.Thread();
                tempPost.Poster = MainViewModel.LoggedInUser;
                tempPost.Text = DescriptionInput;
                _context.Add(tempPost);
                _context.SaveChanges();
                tempThread.Title = TitleInput;
                tempThread.Op = tempPost;
                tempThread.Topic = CurrentTopic;
                _context.Add(tempThread);
                _context.SaveChanges();
                MainViewModel.CreateThreadWindow.Close();
                RefreshThreadList();
            }
        }

        private void OpenCreateThreadWindow()
        {
            MainViewModel.CreateThreadWindow.Show();
        }
    }
}
