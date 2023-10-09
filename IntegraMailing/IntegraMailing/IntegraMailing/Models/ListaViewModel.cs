namespace IntegraMailing.Models
{
    public class ListaViewModel
    {
        public List<Linha> LinhaLista { get; set; }  = new List<Linha>();
        public int PaginaCounter { get; set; } = 1;
        public int MaxPaginaCounter { get; set; } = 1;
    }
}
