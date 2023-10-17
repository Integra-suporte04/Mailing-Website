using System.ComponentModel.DataAnnotations;

namespace IntegraMailing.Data
{
    public class tabela_mailing
    {
        [Key]
        public int Id { get; set; }
        public string? numero;
        public int campanha_Id { get; set; }

    }
}
