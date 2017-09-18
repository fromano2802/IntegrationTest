using System;

namespace IntegrationTest.Models
{
    public class CommentCardAction : ITrelloEntity
    {
        public string Id { get; set; }
        public string IdMemberCreator { get; set; }
        public DateTime Date { get; set; }

        public ActionData Data { get; set; }

        public class ActionData
        {
            public string Text { get; set; }
            public DateTime? DateLastEdited { get; set; }
        }
    }
}