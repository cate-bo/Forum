using Forum.Models;
using Forum.ViewModels;
using Forum.Views;
using System.Collections.ObjectModel;
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

        public MainWindow(MainViewModel viewModel, ForumContext context)
        {
            //stuff
            InitializeComponent();
            _context = context;
            DataContext = viewModel;
            WindowState = WindowState.Maximized;

            
        }


    }
}