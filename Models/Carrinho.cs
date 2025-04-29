namespace devSteamAPI.Models
{
    public class Carrinho
    {
        public Guid CarrinhoId { get; set; }
        public Guid? UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
        public DateTime DataCriacao { get; set; }
        public bool? Finalizado { get; set; }
        public DateTime? DataFinalizacao { get; set; }
        public decimal ValorTotal { get; set; }

    }
}
