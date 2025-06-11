using Forum.Models;
using Forum.Views;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forum.ViewModels
{
    public class PostViewModel : BaseViewModel
    {
        public Post ThisPost { get; set; }
        private ThreadViewModel _parentViewModel;
        public PostViewModel(Post post, ThreadViewModel parentViewModel)
        {
            View = new PostView(this);
            ThisPost = post;
            _parentViewModel = parentViewModel;
        }
    }
}
