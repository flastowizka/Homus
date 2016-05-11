namespace Hackathon.Models
{
    public class ReceitaIngrediente
    {
        public virtual Ingrediente Ingrediente { get; set; }

        public string IngredientePreparo { get; set; }

        public virtual string TipoValoracao { get; set; }

        public virtual double Quantidade { get; set; }
    }
}