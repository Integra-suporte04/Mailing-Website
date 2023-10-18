using System.ComponentModel.DataAnnotations;

namespace IntegraMailing.Data
{
    public class Empresas
    {
        [Key]
        public int Id { get; set; }
        public string? Nome { get; set; }  
    }
}
