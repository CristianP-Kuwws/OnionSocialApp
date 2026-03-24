using LinkUpApp.Core.Application.ViewModels.Social.Posts;

namespace LinkUpApp.Core.Application.ViewModels.Social.Friendship
{
    public class FriendsPageViewModel
    {
        public List<FriendViewModel> Friends { get; set; } = new();
        public List<PostViewModel> FriendsPosts { get; set; } = new();
    }
}
