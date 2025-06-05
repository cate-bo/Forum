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

        //user and login stuff
        private User LoggedInUser;
        private bool _isLoggedIn;
        private bool _isPopupOpen;
        private LoginMenu _loginMenu;
        private UserMenu _userMenu;
        private LoginWindow _loginWindow;
        private bool _closingApp;
        public string UsernameInput { get; set; }
        public string PasswordInput { get; set; }
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


        public ObservableCollection<Topic> Topics { get; set; } = new ObservableCollection<Topic>();

        //Commands
        public RelayCommandWithParameter SelectTopic { get; set; }
        public RelayCommand GoHome { get; set; }
        public RelayCommand ClickLogin { get; set; }
        public RelayCommand ClickLogout { get; set; }
        public RelayCommand ClickFollowView { get; set; }

        public RelayCommand OpenPopupButtonPressed { get; set; }

        public MainViewModel(ForumContext context)
        {
            //Command shit
            SelectTopic = new RelayCommandWithParameter(ViewSelectedTopic);
            GoHome = new RelayCommand(Home);
            OpenPopupButtonPressed = new RelayCommand(OpenPopup);
            ClickLogin = new RelayCommand(OpenLoginWindow);
            ClickLogout = new RelayCommand(Logout);
            ClickFollowView = new RelayCommand(ViewFollowedThreads);


            _currentMainContent = this;
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

            _window.Show();
            foreach (Topic topic in context.Topic)
            {
                Topics.Add(topic);
            }

            Home();
        }

        private void ViewFollowedThreads()
        {
            IsPopupOpen = false;
            _window.grd_main.Children.Remove(_currentMainContent.View);
            _currentMainContent = new FollowViewModel(_context, LoggedInUser);

            _window.grd_main.Children.Add(_currentMainContent.View);
            Grid.SetColumn(_currentMainContent.View, 0);
            Grid.SetRow(_currentMainContent.View, 1);
        }

        private void Logout()
        {
            _isLoggedIn = false;
            _window.LoginDisplay.Text = "no user logged in";
            _window.popupthing.Child = _loginMenu;
            IsPopupOpen = false;
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
                LoggedInUser = temp;
                //TODO update top right corner proper solution
                _window.LoginDisplay.Text = LoggedInUser.Username;
                _window.popupthing.Child = new UserMenu(this);
                _isLoggedIn = true;
                _loginWindow.Close();
            }
            else
            {
                UsernameInput = "";
                PasswordInput = "";
                LoginAttemtErrormessage = Visibility.Visible;
            }
        }

        private void CloseMainWindow(object? sender, CancelEventArgs e)
        {
            _closingApp = true;
            _loginWindow.Close();
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

        private void OpenPopup()
        {
            IsPopupOpen = !IsPopupOpen;
        }

        private void Home()
        {
            _window.grd_main.Children.Remove(_currentMainContent.View);
            View = new MainView(this);
            _currentMainContent = this;

            _window.grd_main.Children.Add(_currentMainContent.View);
            Grid.SetColumn(_currentMainContent.View, 0);
            Grid.SetRow(_currentMainContent.View, 1);
        }

        private void ViewSelectedTopic(object thing)
        {
            _window.grd_main.Children.Remove(_currentMainContent.View);
            _currentMainContent = new TopicViewModel((Topic)thing, _context);
            _window.grd_main.Children.Add(_currentMainContent.View);
            Grid.SetColumn(_currentMainContent.View, 0);
            Grid.SetRow(_currentMainContent.View, 1);
        }
    }
}
