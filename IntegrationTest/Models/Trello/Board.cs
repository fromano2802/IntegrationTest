namespace IntegrationTest.Models
{
    public class Board : ITrelloEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public bool Closed { get; set; }
        public string IdOrganization { get; set; }
    }
}