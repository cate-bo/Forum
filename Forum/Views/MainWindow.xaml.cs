using Forum.Models;
using Forum.ViewModels;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Forum
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ForumContext _context;
        private Image _logo = new Image();
        private BitmapImage _logo_bmi = new BitmapImage();
        private Button _loginOrOut;
        private Button _settings;
        public MainWindow(MainViewModel viewModel, ForumContext context)
        {
            //stuff
            InitializeComponent();
            _context = context;
            DataContext = viewModel;

            //initialize ui
            WindowState = WindowState.Maximized;

            _logo_bmi = new BitmapImage();
            _logo_bmi.BeginInit();
            _logo_bmi.UriSource = new Uri(@"\Views\Assets\logo_standard.png", UriKind.Relative); //TODO this doesnt work
            _logo_bmi.EndInit();
            _logo.Source = _logo_bmi;
            

            grd_main.ColumnDefinitions.Add(new ColumnDefinition());
            grd_main.RowDefinitions.Add(new RowDefinition());
            grd_main.RowDefinitions.Add(new RowDefinition());

            grd_main.Children.Add(_logo);

            Grid.SetColumn(_logo, 0);

            Grid.SetRow(_logo, 0);
        }


    }
}