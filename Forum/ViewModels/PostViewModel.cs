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
        public PostViewModel(Post post)
        {
            View = new PostView(this);
            ThisPost = post;
        }
    }
}
