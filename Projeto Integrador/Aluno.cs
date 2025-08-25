using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projeto_Integrador
{
    public class Aluno
    {
        public int Codigo { get; set; }
        public required string Nome { get; set; }
        public required string Email { get; set; }
        public required DateOnly DataNascimento { get; set; }
        public required string  Cpf { get; set; }
        public required string  Cep { get; set; }
        public required int Numero { get; set; }
        public required int SaldoImpressoes { get; set; }
    }   
}