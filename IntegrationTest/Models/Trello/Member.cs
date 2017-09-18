namespace IntegrationTest.Models
{
    public class Member : ITrelloEntity
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Bio { get; set; }
        public string Url { get; set; }
        public string AvatarHash { get; set; }
        public string Status { get; set; }
        public string Initials { get; set; }
    }
}