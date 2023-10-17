using IntegraMailing.Data;

namespace IntegraMailing.Models
{
    public class ListaViewModel
    {
        public List<Campanhas> LinhaLista { get; set; }  = new List<Campanhas>();
        public int PaginaCounter { get; set; } = 1;
        public int MaxPaginaCounter { get; set; } = 1;
    }
}
