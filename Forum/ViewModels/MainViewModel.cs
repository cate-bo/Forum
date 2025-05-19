using Forum.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Forum.ViewModels
{
    public class MainViewModel
    {
        private readonly ForumContext _context;

        public MainViewModel(ForumContext context)
        {
            _context = context;
        }
    }
}
