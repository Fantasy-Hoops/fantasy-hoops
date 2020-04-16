using System;
using System.Collections.Generic;
using fantasy_hoops.Models;

namespace fantasy_hoops.Dtos
{
    public class NewsDto
    {
        public int Id { get; set; }
        public String Type { get; set; }
        public string Title { get; set; }
        public String Date { get; set; }
        public string hTeam { get; set; }
        public string vTeam { get; set; }
        public virtual List<String> Paragraphs { get; set; }
    }
}