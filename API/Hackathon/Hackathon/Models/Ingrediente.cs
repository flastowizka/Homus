using System.Collections.Generic;

namespace Hackathon.Models
{
    public class Ingrediente : BaseModel
    {
        public virtual string Nome { get; set; }

        public IList<string> KeyWord { get; set; }

        public Ingrediente(int codigo, string nome, List<string> kew)
        {
            this.Nome = nome;
            this.Codigo = codigo;
            this.KeyWord = kew;
        }
    }
}