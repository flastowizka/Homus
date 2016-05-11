using System.Collections.Generic;
using System.Web.Http;

namespace Hackathon.Models
{
    public class UsuarioView
    {
        public int TotalPontos { get; set; }

        public string NomeAssistente { get; set; }

        public string NomeUsuario { get; set; }

        public IList<ListaView> ListaViewL { get; set; }

        public UsuarioView()
        {
            ListaViewL = new List<ListaView>();
        }
    }
}