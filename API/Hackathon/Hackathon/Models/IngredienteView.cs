namespace Hackathon.Models
{
    public class IngredienteView
    {
        public int Codigo { get; set; }

        public Ingrediente Ingrediente { get; set; }

        public double Quantidade { get; set; }

        public string TipoValoracao { get; set; }

        public bool Promocao { get; set; }

        public bool Tenho { get; set; }

        public string IngredienteNome { get; set; }

        public string CodigoBarra { get; set; }

        public string CodigoBarraInfo { get; set; }
    }
}