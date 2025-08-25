using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projeto_Integrador
{
    internal class HistoricoImpressoes
    {
        public int codigo { get; set; }
        public int codImpressao { get; set; }
        public int codAluno { get; set; }
        public DateTime dataHora { get; set; }
        public int quantidade { get; set; }

        internal static void Add(HistoricoImpressoes historicoImpressoes)
        {
            throw new NotImplementedException();
        }
    }
}
