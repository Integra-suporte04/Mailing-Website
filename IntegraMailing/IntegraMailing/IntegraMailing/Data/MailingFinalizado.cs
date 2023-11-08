namespace IntegraMailing.Data
{
    public class MailingFinalizado
    {
        public int Id { get; set; }
        public string? numero { get; set; }
        public string? status_1 { get; set; }
        public DateTime? hora_tentativa_1 { get; set; }
        public string? status_2 { get; set; }
        public DateTime? hora_tentativa_2 { get; set; }
        public string? status_3 { get; set; }
        public DateTime? hora_tentativa_3 { get; set; }
        public string? statusFinal { get; set; }
        public int campanha_id { get; set; }
    }
}
