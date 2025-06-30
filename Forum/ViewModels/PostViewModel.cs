using Forum.Commands;
using Forum.Models;
using Forum.Views;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Forum.ViewModels
{
    public class PostViewModel : BaseViewModel
    {
        public Post ThisPost { get; set; }
        private ThreadViewModel _parentViewModel;
        public TreeViewItem PostTreeViewItem { get; set; }
        public RelayCommand ClickReply { get; set; }

        public ObservableCollection<TreeViewItem> Replies { get; set; }
        public PostViewModel(Post post, ThreadViewModel parentViewModel)
        {
            View = new PostView(this);
            Replies = new ObservableCollection<TreeViewItem>();
            ThisPost = post;
            _parentViewModel = parentViewModel;
            PostTreeViewItem = new TreeViewItem();
            PostTreeViewItem.ItemsSource = Replies;
            PostTreeViewItem.Selected += PostTreeViewItem_Selected;
            //PostTreeViewItem.DataContext = this;
            PostTreeViewItem.Header = View;
            ClickReply = new RelayCommand(ReplyTo);
            if(!MainViewModel.IsLoggedIn)
            {
                ((PostView)View).grd_main.RowDefinitions[2].Height = new System.Windows.GridLength(0);
            }
        }

        private void PostTreeViewItem_Selected(object sender, System.Windows.RoutedEventArgs e)
        {
            PostTreeViewItem.IsSelected = false;
            PostTreeViewItem.IsExpanded = !PostTreeViewItem.IsExpanded;
        }

        private void ReplyTo()
        {
            _parentViewModel.ChangeReplyRecipient(this);
        }
    }
}
