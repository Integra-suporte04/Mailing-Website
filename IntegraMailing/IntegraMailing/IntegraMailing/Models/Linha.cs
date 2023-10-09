namespace IntegraMailing.Models
{
    public class Linha
    {
        public string Nome { get; set; }
        public DateTime Data { get; set; }
        public string Status { get; set; }

        public Linha(string Nome, DateTime Data, string Status)
        {
            this.Nome = Nome;
            this.Data = Data;
            this.Status = Status;
        }
    }
}
