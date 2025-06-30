using Forum.Commands;
using Forum.Models;
using Forum.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;

namespace Forum.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private ForumContext _context;
        private MainWindow _window;
        //currently displayed UserControl
        public BaseViewModel CurrentMainContent { get; set; }

        //user and login stuff
        public static User LoggedInUser;
        public static bool IsLoggedIn;
        private bool _isPopupOpen;
        private LoginMenu _loginMenu;
        private UserMenu _userMenu;
        public static LoginWindow LoginWindow;
        public static bool ClosingApp;

        private string _usernameInput;

        public string UsernameInput
        {
            get { return _usernameInput; }
            set
            {
                _usernameInput = value;
                OnpropertyChanged(nameof(UsernameInput));
            }
        }

        

        public RelayCommand LoginAttemtClick { get; set; }
        private System.Windows.Visibility _loginAttemtErrormessage;

        public System.Windows.Visibility LoginAttemtErrormessage
        {
            get { return _loginAttemtErrormessage; }
            set
            {
                _loginAttemtErrormessage = value;
                OnpropertyChanged(nameof(LoginAttemtErrormessage));
            }
        }


        public bool IsPopupOpen
        {
            get { return _isPopupOpen; }
            set
            {
                _isPopupOpen = value;
                OnpropertyChanged(nameof(IsPopupOpen));
            }
        }

        //user creation stuff
        public static CreateUserWindow CreateUserWindow;
        private string _newUsername;

        public string NewUsername
        {
            get { return _newUsername; }
            set
            {
                _newUsername = value;
                OnpropertyChanged(nameof(NewUsername));
            }
        }

       

        public RelayCommand CreateAttemtClick { get; set; }

       

        private string _createAttemtErrorMessage;

        public string CreateAttemtErrorMessage
        {
            get { return _createAttemtErrorMessage; }
            set
            {
                _createAttemtErrorMessage = value;
                OnpropertyChanged(nameof(CreateAttemtErrorMessage));
            }
        }

        //topic creation stuff
        private string _newTitle;

        public string NewTitle
        {
            get { return _newTitle; }
            set
            {
                _newTitle = value;
                OnpropertyChanged(nameof(NewTitle));
            }
        }

        private string _newDescription;

        public string NewDescription
        {
            get { return _newDescription; }
            set
            {
                _newDescription = value;
                OnpropertyChanged(nameof(NewDescription));
            }
        }

        public RelayCommand NewTopicAttemtClick { get; set; }

        public static CreateTopicWindow CreateTopicWindow;

        private string _createTopicAttemtErrormessage;

        public string CreateTopicAttemtErrormessage
        {
            get { return _createTopicAttemtErrormessage; }
            set
            {
                _createTopicAttemtErrormessage = value;
                OnpropertyChanged(nameof(CreateTopicAttemtErrormessage));
            }
        }

        //threadcreationwindow
        public static CreateThreadWindow CreateThreadWindow;

        //topiclist
        public ObservableCollection<Topic> Topics { get; set; } = new ObservableCollection<Topic>();

        //Commands
        public RelayCommandWithParameter SelectTopic { get; set; }
        public RelayCommand GoHome { get; set; }
        public RelayCommand ClickLogin { get; set; }
        public RelayCommand ClickLogout { get; set; }
        public RelayCommand ClickFollowView { get; set; }
        public RelayCommand ClickCreateUser { get; set; }
        public RelayCommand ClickNewTopic { get; set; }
        public RelayCommand OpenPopupButtonPressed { get; set; }
        public RelayCommand ClickMyThreadsView { get; set; }

        public MainViewModel(ForumContext context)
        {
            //Command shit
            SelectTopic = new RelayCommandWithParameter(ViewSelectedTopic);
            GoHome = new RelayCommand(Home);
            OpenPopupButtonPressed = new RelayCommand(OpenPopup);
            ClickLogin = new RelayCommand(OpenLoginWindow);
            ClickLogout = new RelayCommand(Logout);
            ClickFollowView = new RelayCommand(ViewFollowedThreads);
            ClickCreateUser = new RelayCommand(OpenCreateUserWindow);
            ClickNewTopic = new RelayCommand(OpenCreateTopicWindow);
            ClickMyThreadsView = new RelayCommand(ViewMyThreads);

            //stuff
            CurrentMainContent = this;
            _context = context;
            _window = new MainWindow(this, context);
            _window.Closing += CloseMainWindow;


            //login stuff
            _loginMenu = new LoginMenu(this);
            _window.popupthing.Child = _loginMenu;
            ClosingApp = false;

            LoginWindow = new LoginWindow(this);
            LoginWindow.Closing += CloseLoginWindow;
            LoginAttemtClick = new RelayCommand(AttemtLogin);
            LoginAttemtErrormessage = Visibility.Hidden;

            //user creation stuff
            CreateUserWindow = new CreateUserWindow(this);
            CreateUserWindow.Closing += CloseCreateUserWindow;
            CreateAttemtClick = new RelayCommand(AttemtUserCreation);
            NewUsername = "";
            CreateUserWindow.passw1.Password = "";
            CreateUserWindow.passw2.Password = "";
            CreateAttemtErrorMessage = "";

            //topic creation stuff
            CreateTopicWindow = new CreateTopicWindow(this);
            CreateTopicWindow.Closing += CloseCreateTopicWindow;
            NewTopicAttemtClick = new RelayCommand(AttemtTopicCreation);
            NewTitle = "";
            NewDescription = "";
            CreateTopicAttemtErrormessage = "";

            //Threadcreationwindow
            CreateThreadWindow = new CreateThreadWindow();
            CreateThreadWindow.Closing += CreateThreadWindow_Closing;

            //other stuff
            _window.Show();
            RefreshTopicList();
            Logout();
            Home();
        }

        private void CreateThreadWindow_Closing(object? sender, CancelEventArgs e)
        {
            if (!ClosingApp)
            {
                e.Cancel = true;
            }
        }

        private void ViewMyThreads()
        {
            IsPopupOpen = false;
            _window.grd_main.Children.Remove(CurrentMainContent.View);
            CurrentMainContent = new MyThreadsViewModel(_context, LoggedInUser, this);

            _window.grd_main.Children.Add(CurrentMainContent.View);
            Grid.SetColumn(CurrentMainContent.View, 0);
            Grid.SetRow(CurrentMainContent.View, 1);
        }

        private void RefreshTopicList()
        {
            Topics.Clear();
            foreach (Topic topic in _context.Topic)
            {
                Topics.Add(topic);
            }
        }

        public void Viewthread(Models.Thread thread)
        {
            _window.grd_main.Children.Remove(CurrentMainContent.View);
            CurrentMainContent = new ThreadViewModel(thread, _context);

            _window.grd_main.Children.Add(CurrentMainContent.View);
            Grid.SetColumn(CurrentMainContent.View, 0);
            Grid.SetRow(CurrentMainContent.View, 1);
        }

        private void AttemtTopicCreation()
        {
            if(NewTitle.Length < 1 || NewDescription.Length < 1)
            {
                CreateTopicAttemtErrormessage = "title and description cannot be empty";
            }
            else if(_context.Topic.Where(a => a.Title ==  NewTitle).Count() > 0)
            {
                CreateTopicAttemtErrormessage = "topic already exists";
            }
            else
            {
                Topic temp = new Topic();
                temp.Title = NewTitle;
                temp.Description = NewDescription;
                _context.Add(temp);
                _context.SaveChanges();
                CreateTopicWindow.Close();
                RefreshTopicList();
            }
            NewTitle = "";
            NewDescription = "";
            CreateTopicWindow.TitleInput.Focus();
        }

        private void CloseCreateTopicWindow(object? sender, CancelEventArgs e)
        {
            if (!ClosingApp)
            {
                e.Cancel = true;
                NewTitle = "";
                NewDescription = "";
                CreateTopicAttemtErrormessage = "";
                CreateTopicWindow.Hide();
            }
        }

        private void OpenCreateTopicWindow()
        {
            CreateTopicWindow.Show();
            CreateTopicWindow.TitleInput.Focus();
        }

        private void CloseCreateUserWindow(object? sender, CancelEventArgs e)
        {
            if (!ClosingApp)
            {
                e.Cancel = true;
                NewUsername = "";
                CreateUserWindow.passw1.Password = "";
                CreateUserWindow.passw2.Password = "";
                CreateAttemtErrorMessage = "";
                CreateUserWindow.Hide();
            }
        }

        private void AttemtUserCreation()
        {
            if(NewUsername.Length < 1)
            {
                CreateAttemtErrorMessage = "username cannot be empty";
            }
            else if(_context.User.Where(a => a.Username == NewUsername).Count() > 0)
            {
                CreateAttemtErrorMessage = "username already taken";
                NewUsername = "";
            }
            else if(CreateUserWindow.passw1.Password != CreateUserWindow.passw2.Password)
            {
                CreateAttemtErrorMessage = "passwords don't match";
            }
            else
            {
                User temp = new User();
                temp.Username = NewUsername;
                temp.Password = CreateUserWindow.passw1.Password;
                _context.Add(temp);
                _context.SaveChanges();
                Login(temp);

                NewUsername = "";
                CreateUserWindow.passw1.Password = "";
                CreateUserWindow.passw2.Password = "";
                CreateAttemtErrorMessage = "";
                CreateUserWindow.Hide();
            }

            CreateUserWindow.passw1.Password = "";
            CreateUserWindow.passw2.Password = "";
        }

        private void ViewFollowedThreads()
        {
            IsPopupOpen = false;
            _window.grd_main.Children.Remove(CurrentMainContent.View);
            CurrentMainContent = new FollowViewModel(_context, LoggedInUser, this);

            _window.grd_main.Children.Add(CurrentMainContent.View);
            Grid.SetColumn(CurrentMainContent.View, 0);
            Grid.SetRow(CurrentMainContent.View, 1);
        }

        private void Logout()
        {
            IsLoggedIn = false;
            _window.LoginDisplay.Text = "no user logged in";
            _window.popupthing.Child = _loginMenu;
            IsPopupOpen = false;
            LoginWindow.Close();
            CreateUserWindow.Close();
            CreateTopicWindow.Close();
            CreateThreadWindow.Close();
            Home();
        }

        private void AttemtLogin()
        {
            User temp = new User();
            try
            {
                temp = _context.User.Where(a => a.Username == UsernameInput)
                    .Include(b => b.Thread)
                    .First();
            }
            catch (Exception ex)
            {
                UsernameInput = "";
                LoginWindow.pwordbox.Password = "";
                LoginAttemtErrormessage = Visibility.Visible;
                return;
            }
            if (temp.Password == LoginWindow.pwordbox.Password)
            {
                Login(temp);
                LoginWindow.Close();
            }
            else
            {
                UsernameInput = "";
                LoginWindow.pwordbox.Password = "";
                LoginAttemtErrormessage = Visibility.Visible;
            }
        }

        private void Login(User user)
        {
            LoggedInUser = user;
            //TODO update top right corner proper solution
            _window.LoginDisplay.Text = LoggedInUser.Username;
            _window.popupthing.Child = new UserMenu(this);
            IsLoggedIn = true;
            Home();
        }

        private void CloseMainWindow(object? sender, CancelEventArgs e)
        {
            ClosingApp = true;
            LoginWindow.Close();
            CreateUserWindow.Close();
            CreateTopicWindow.Close();
            CreateThreadWindow.Close();
        }

        private void CloseLoginWindow(object? sender, CancelEventArgs e)
        {
            if(!ClosingApp)
            {
                e.Cancel = true;
                LoginAttemtErrormessage = Visibility.Hidden;
                UsernameInput = "";
                LoginWindow.pwordbox.Password = "";
                LoginWindow.Hide();
            }
        }

        private void OpenLoginWindow()
        {
            LoginWindow.Close();
            CreateUserWindow.Close();
            CreateTopicWindow.Close();
            CreateThreadWindow.Close();
            LoginWindow.Show();
            LoginWindow.UsernameInputField.Focus();
            IsPopupOpen = false;
        }

        private void OpenCreateUserWindow()
        {
            LoginWindow.Close();
            CreateUserWindow.Close();
            CreateTopicWindow.Close();
            CreateThreadWindow.Close();
            CreateUserWindow.Show();
            CreateUserWindow.UsernameInputField.Focus();
            IsPopupOpen = false;
        }

        private void OpenPopup()
        {
            IsPopupOpen = !IsPopupOpen;
        }

        private void Home()
        {
            _window.grd_main.Children.Remove(CurrentMainContent.View);
            View = new MainView(this);
            CurrentMainContent = this;

            _window.grd_main.Children.Add(CurrentMainContent.View);
            Grid.SetColumn(CurrentMainContent.View, 0);
            Grid.SetRow(CurrentMainContent.View, 1);
        }

        private void ViewSelectedTopic(object thing)
        {
            _window.grd_main.Children.Remove(CurrentMainContent.View);
            CurrentMainContent = new TopicViewModel((Topic)thing, _context);
            _window.grd_main.Children.Add(CurrentMainContent.View);
            Grid.SetColumn(CurrentMainContent.View, 0);
            Grid.SetRow(CurrentMainContent.View, 1);
        }
    }
}
