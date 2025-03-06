using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Edoha.Domain.Entities
{
    [Table("Lottery", Schema = "RIFAS")]
    public class Lottery
    {
        public int IdLottery { get; set; }

        [Required(ErrorMessage = "O nome da Rifa é obrigatório.")]
        public string NameLottery { get; set; }

        [Required(ErrorMessage = "É obrigatório definir a quantidade de bilhetes em um talão.")]
        public int NumTicketsTicketbookLottery { get; set; }

        [Required(ErrorMessage = "É obrigatório definir uma quantidade de talões para a Rifa.")]
        public int NumTicketbooksLottery { get; set; }

        [Required(ErrorMessage = "É obrigatório definir um preço para cada bilhete da Rifa.")]
        public decimal PriceTicketLottery { get; set; }

        [Required(ErrorMessage = "É obrigatório definir se cada bilhete dará dois números.")]
        public bool DoubleChanceLottery { get; set; }

        public DateTime InsertionDateLottery { get; set; }
    }
}
