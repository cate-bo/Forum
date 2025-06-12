using Forum.Models;
using Forum.Views;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forum.ViewModels
{
    public class ThreadViewModel : BaseViewModel
    {
        private ForumContext _context;
        public Post OP { get; set; }
        public string OPsUsername { get; set; }
        public Models.Thread _thread { get; set; }
        public ThreadViewModel(Models.Thread thread, ForumContext context)
        {
            _context = context;
            View = new ThreadView(this);
            _thread = thread;

            
            OP = _context.Post.Where(a => a.PostId == _thread.Opid)
                .Include(b => b.Poster)
                .First();

            OPsUsername = OP.Poster.Username;
            var posts = _context.Post.Where(a => a.PredecessorId == OP.PostId);
        }

        public void FillPostList(int predecessorID)
        {
            var posts = _context.Post.Where(a => a.PredecessorId == predecessorID);
            foreach (Post post in posts)
            {
                
            }
        }
    }
}
