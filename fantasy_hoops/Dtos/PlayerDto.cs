using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fantasy_hoops.Models;

namespace fantasy_hoops.Dtos
{
    public class PlayerDto
    {
        public int PlayerId { get; set; }
        public int NbaId { get; set; }
        public string Position { get; set; }
        public string TeamColor { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string AbbrName { get; set; }
        public double FP { get; set; }
    }
}
