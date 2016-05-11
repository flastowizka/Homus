using System.Collections.Generic;

namespace Hackathon.Models
{
    public class Promocao
    {
        public int Codigo { get; set; }

        public string Nome { get; set; }

        public IList<Ingrediente> IngredienteL { get; set; }

        public Promocao()
        {
            this.IngredienteL = new List<Ingrediente>();
        }
    }
}