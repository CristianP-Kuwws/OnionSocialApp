namespace LinkUpApp.Core.Application.Dtos.Social
{
    public class FriendshipDto
    {
        public int Id { get; set; }
        public required string User1Id { get; set; }
        public required string User2Id { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
