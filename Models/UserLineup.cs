using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace fantasy_hoops.Models
{
    public class UserLineup
    {
        public int ID { get; set; }
        [Column(TypeName = "Date")]
        public DateTime Date { get; set; }
        public string UserID { get; set; }
        public int PgID { get; set; }
        public int SgID { get; set; }
        public int SfID { get; set; }
        public int PfID { get; set; }
        public int CID { get; set; }
        public double FP { get; set; }
        public bool IsCalculated { get; set; }

        [ForeignKey("UserID")]
        public virtual User User { get; set; }
        [ForeignKey("PgID")]
        public virtual Player Pg { get; set; }
        [ForeignKey("SgID")]
        public virtual Player Sg { get; set; }
        [ForeignKey("SfID")]
        public virtual Player Sf { get; set; }
        [ForeignKey("PfID")]
        public virtual Player Pf { get; set; }
        [ForeignKey("CID")]
        public virtual Player C { get; set; }
    }
}
