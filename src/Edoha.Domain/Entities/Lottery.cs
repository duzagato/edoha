using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Edoha.Shared.Annotations.Numerical;

namespace Edoha.Domain.Entities
{
    [Table("Lottery", Schema = "RIFAS")]
    public class Lottery
    {
        public int IdLottery { get; set; }
        public string NameLottery { get; set; }
        public int NumTicketsTicketbookLottery { get; set; }
        public int NumTicketbooksLottery { get; set; }
        public decimal PriceTicketLottery { get; set; }
        public bool DoubleChanceLottery { get; set; }
        public DateTime InsertionDateLottery { get; set; }
    }
}
