using System;
using System.Collections.Generic;

namespace IntegrationTest.Models
{
    public class Card : ITrelloEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public bool Closed { get; set; }
        public string IdList { get; set; }
        public string IdBoard { get; set; }
        public DateTime? Due { get; set; }
        public int IdShort { get; set; }
        public string Url { get; set; }
        public string ShortUrl { get; set; }
        public double Pos { get; set; }
        public DateTime DateLastActivity { get; set; }
        public List<string> IdMembers { get; set; }
    }
}