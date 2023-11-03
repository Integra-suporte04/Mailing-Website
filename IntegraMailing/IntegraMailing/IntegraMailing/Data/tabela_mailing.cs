using System.ComponentModel.DataAnnotations;

namespace IntegraMailing.Data
{
    public class tabela_mailing
    {
        [Key]
        public int Id { get; set; }
        public string? numero { get; set; }
        public string? status { get; set; }
        public DateTime hora_tentativa_1 { get; set; }
        public DateTime hora_tentativa_2 { get; set; }
        public DateTime hora_tentativa_3 { get; set; }
        public int campanha_Id { get; set; }

    }
}
