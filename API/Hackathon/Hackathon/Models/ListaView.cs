using System.Collections;
using System.Collections.Generic;

namespace Hackathon.Models
{
    public class ListaView
    {
        public int Codigo { get; set; }

        public string Nome { get; set; }

        public bool ListaFinalizada { get; set; }

        public IList<IngredienteView> IngredienteL { get; set; }

        public ListaView()
        {
            IngredienteL = new List<IngredienteView>();
        }
    }
}