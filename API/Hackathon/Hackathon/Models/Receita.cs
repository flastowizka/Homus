using System.Collections.Generic;

namespace Hackathon.Models
{
    public class Receita  : BaseModel
    {
        public string Nome { get; set; }

        public string Nivel { get; set; }

        public string TempoPreparo { get; set; }

        public virtual string ModoPreparo { get; set; }

        public virtual IList<ReceitaIngrediente> ReceitaIngredientes { get; set; }
    }
}