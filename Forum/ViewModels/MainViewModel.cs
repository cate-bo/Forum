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

namespace Forum.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly ForumContext _context;
        private MainWindow _window;
        //currently displayed UserControl
        public BaseViewModel CurrentMainContent { get; set; }

        //user and login stuff
        private User LoggedInUser;
        public static bool IsLoggedIn;
        private bool _isPopupOpen;
        private LoginMenu _loginMenu;
        private UserMenu _userMenu;
        private LoginWindow _loginWindow;
        private bool _closingApp;

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

        private string _passwordInput;

        public string PasswordInput
        {
            get { return _passwordInput; }
            set
            {
                _passwordInput = value;
                OnpropertyChanged(nameof(PasswordInput));
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
        private CreateUserWindow _createUserWindow;
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

        private string _newPassword1;

        public string NewPassword1
        {
            get { return _newPassword1; }
            set
            {
                _newPassword1 = value;
                OnpropertyChanged(nameof(NewPassword1));
            }
        }

        private string _newPassword2;

        public string NewPassword2
        {
            get { return _newPassword2; }
            set
            {
                _newPassword2 = value;
                OnpropertyChanged(nameof(NewPassword2));
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

        private CreateTopicWindow _createTopicWindow;

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
            _closingApp = false;

            _loginWindow = new LoginWindow(this);
            _loginWindow.Closing += CloseLoginWindow;
            LoginAttemtClick = new RelayCommand(AttemtLogin);
            LoginAttemtErrormessage = Visibility.Hidden;
            Logout();

            //user creation stuff
            _createUserWindow = new CreateUserWindow(this);
            _createUserWindow.Closing += CloseCreateUserWindow;
            CreateAttemtClick = new RelayCommand(AttemtUserCreation);
            NewUsername = "";
            NewPassword1 = "";
            NewPassword2 = "";
            CreateAttemtErrorMessage = "";

            //topic creation stuff
            _createTopicWindow = new CreateTopicWindow(this);
            _createTopicWindow.Closing += CloseCreateTopicWindow;
            NewTopicAttemtClick = new RelayCommand(AttemtTopicCreation);
            NewTitle = "";
            NewDescription = "";
            CreateTopicAttemtErrormessage = "";

            //other stuff
            _window.Show();
            RefreshTopicList();


            Home();
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
                _createTopicWindow.Close();
                RefreshTopicList();
            }
            NewTitle = "";
            NewDescription = "";
            _createTopicWindow.TitleInput.Focus();
        }

        private void CloseCreateTopicWindow(object? sender, CancelEventArgs e)
        {
            if (!_closingApp)
            {
                e.Cancel = true;
                NewTitle = "";
                NewDescription = "";
                CreateTopicAttemtErrormessage = "";
                _createTopicWindow.Hide();
            }
        }

        private void OpenCreateTopicWindow()
        {
            _createTopicWindow.Show();
            _createTopicWindow.TitleInput.Focus();
        }

        private void CloseCreateUserWindow(object? sender, CancelEventArgs e)
        {
            if (!_closingApp)
            {
                e.Cancel = true;
                NewUsername = "";
                NewPassword1 = "";
                NewPassword2 = "";
                CreateAttemtErrorMessage = "";
                _createUserWindow.Hide();
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
            else if(NewPassword1 != NewPassword2)
            {
                CreateAttemtErrorMessage = "passwords don't match";
            }
            else
            {
                User temp = new User();
                temp.Username = NewUsername;
                temp.Password = NewPassword1;
                _context.Add(temp);
                _context.SaveChanges();
                Login(temp);

                NewUsername = "";
                NewPassword1 = "";
                NewPassword2 = "";
                CreateAttemtErrorMessage = "";
                _createUserWindow.Hide();
            }

            NewPassword1 = "";
            NewPassword2 = "";
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
            Home();
        }

        private void AttemtLogin()
        {
            User temp = new User();
            try
            {
                temp = _context.User.Where(a => a.Username == UsernameInput).First();
            }
            catch (Exception ex)
            {
                UsernameInput = "";
                PasswordInput = "";
                LoginAttemtErrormessage = Visibility.Visible;
                return;
            }
            if (temp.Password == PasswordInput)
            {
                Login(temp);
                _loginWindow.Close();
            }
            else
            {
                UsernameInput = "";
                PasswordInput = "";
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
            _closingApp = true;
            _loginWindow.Close();
            _createUserWindow.Close();
            _createTopicWindow.Close();
        }

        private void CloseLoginWindow(object? sender, CancelEventArgs e)
        {
            if(!_closingApp)
            {
                e.Cancel = true;
                LoginAttemtErrormessage = Visibility.Hidden;
                UsernameInput = "";
                PasswordInput = "";
                _loginWindow.Hide();
            }
        }

        private void OpenLoginWindow()
        {
            _loginWindow.Show();
            _loginWindow.UsernameInputField.Focus();
            IsPopupOpen = false;
        }

        private void OpenCreateUserWindow()
        {
            _createUserWindow.Show();
            _createUserWindow.UsernameInputField.Focus();
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
