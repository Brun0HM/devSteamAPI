namespace devSteamAPI.Models
{
    public class Cupom
    {
        public Guid CupomId { get; set; }
        public string Nome { get; set; }
        public int Desconto { get; set; }
        public DateTime? DataValidade { get; set; }
        public bool? Ativo { get; set; }
        public DateTime? DataCriacao { get; set; }
        public int? LimiteUso { get; set; }
    }
}
