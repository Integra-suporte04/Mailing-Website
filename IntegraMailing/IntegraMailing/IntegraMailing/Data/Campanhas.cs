using System.ComponentModel.DataAnnotations;
namespace IntegraMailing.Data
{
    public class Campanhas
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? user_name { get; set; }
        public string? Status { get; set; }
        public DateTime? Data_Sent { get; set; }
        public int ExecutionCount { get; set; }
        public bool Executed { get; set; }
        public bool Paused { get; set; }
        public float Evolution { get; set; }

    }
}
