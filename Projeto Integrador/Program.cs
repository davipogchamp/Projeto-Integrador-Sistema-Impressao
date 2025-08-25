using System;
using System.ComponentModel.Design;
using System.Data.SqlClient;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Projeto_Integrador;

Conexao db = new();
db.Conectar();
Menu(); 

void Menu()
{
    do
    {
        Console.Clear();
        Console.WriteLine("------- BEM-VINDO AO SISTEMA DE IMPRESSÕES -------");
        Console.WriteLine("");
        Console.WriteLine("[1] - Cadastrar Aluno");
        Console.WriteLine("[2] - Realizar impressão");
        Console.WriteLine("[3] - Comprar ficha");
        Console.WriteLine("[4] - Consultar saldo de alunos");
        Console.WriteLine("[5] - Consultar histórico de impressões");
        Console.WriteLine("[6] - Consultar histórico de compras");
        Console.WriteLine("[7] - Consultar codigo do aluno");
        Console.WriteLine("[8] - Sair");
        Console.WriteLine("");
        Console.WriteLine("--------------------------------------------------");
        Console.Write("Escolha uma opção: ");

        if (int.TryParse(Console.ReadLine(), out int opcaoMenu))
        {
            switch (opcaoMenu)
            {
                case 1:
                    CadastrarAluno(db);
                    break;
                case 2:
                    Imprimir(db);
                    break;
                case 3:
                    ComprarFichas(db);
                    break;
                case 4:
                    ConsultarSaldoAluno(db);
                    break;
                case 5:
                    ConsultarHistoricoImpressao(db);
                    break;
                case 6:
                    ConsultarHistoricoMovimentacao(db);
                    break;
                case 7:
                    ConsultarCodigoAluno(db);
                    break;
                case 8:
                    Console.Clear();
                    Console.WriteLine("Saindo...");
                    return;
                default:
                    Console.WriteLine("");
                    Console.WriteLine("Opção inválida. Tente novamente.");
                    break;
            }
        }
        else
        {
            Console.WriteLine("");
            Console.WriteLine("Entrada inválida. Por favor, insira um número.");
        }
        Console.WriteLine("");
        Console.WriteLine("Pressione qualquer tecla para continuar...");
        Console.ReadKey();
    } while (true);
}

void CadastrarAluno(Conexao db)
{
    Console.Clear();
    Console.WriteLine("------- CADASTRO DE ALUNO -------");
    Console.WriteLine("");
    Console.Write("Digite o nome do aluno: ");
    string? nome = Console.ReadLine();
    Console.Write("Digite o email do aluno: ");
    string? email = Console.ReadLine();
    Console.Write("Digite a data de nascimento do aluno (dd/mm/aaaa): ");
    string? dataNascimento = Console.ReadLine();
    //-----------Validar Data---------------------------------
    dataNascimento = Regex.Replace(dataNascimento, "[^0-9]", "");
    if (dataNascimento.Length != 8)
    {
        Console.WriteLine("");
        Console.WriteLine("Data de nascimento inserida inválida! Digite 8 número");
        return;
    }

    Console.Write("Digite o CPF do aluno: ");
    string? cpf = Console.ReadLine();
    //----------Validar Cpf----------------------
    cpf = Regex.Replace(cpf, "[^0-9]", "");
    if (cpf.Length != 11)
    {
        Console.WriteLine("");
        Console.WriteLine("CPF inválido! Digite 11 números.");
        return;
    }
    if (!ValidarCpf(cpf))
    {
        Console.WriteLine("");
        Console.WriteLine("CPF inválido! Digite um CPF válido.");
        return;
    }

    Console.Write("Digite o CEP do aluno: ");
    string? cep = Console.ReadLine();
    //------------Validar Cep----------------------------
    cep = Regex.Replace(cep, "[^0-9]", "");
    if (cep.Length != 8)
    {
        Console.WriteLine("");
        Console.WriteLine("CEP inválido! Digite 8 números.");
        return;
    }

    Console.Write("Digite o número do endereço: ");
    string? numero = Console.ReadLine();
    int saldoImpressao = 0;

    if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(dataNascimento) ||
        string.IsNullOrWhiteSpace(cpf) || string.IsNullOrWhiteSpace(cep) || string.IsNullOrWhiteSpace(numero))
    {
        Console.WriteLine("");
        Console.WriteLine("Todos os campos são obrigatórios.");
        return;
    }
    try
    {
        string sql = @"  
       INSERT INTO Alunos (nome, email, dataNascimento, cpf, cep, numero, saldoImpressoes)  
       VALUES (@Nome, @Email, @DataNascimento, @Cpf, @Cep, @Numero, @SaldoImpressoes)";

        using SqlCommand cmd = new(sql, db.conn);
        cmd.Parameters.AddWithValue("@Nome", nome);
        cmd.Parameters.AddWithValue("@Email", email);
        cmd.Parameters.AddWithValue("@DataNascimento", FormatarData(dataNascimento));
        cmd.Parameters.AddWithValue("@Cpf", FormatarCpf(cpf));
        cmd.Parameters.AddWithValue("@Cep", FormatarCep(cep));
        cmd.Parameters.AddWithValue("@Numero", numero);
        cmd.Parameters.AddWithValue("@SaldoImpressoes", saldoImpressao);
        cmd.ExecuteNonQuery();

        Console.Clear();
        Console.WriteLine("");
        Console.WriteLine("Aluno cadastrado com sucesso!");
    }
    catch (Exception e)
    {
        Console.WriteLine("");
        Console.WriteLine($"Erro ao cadastrar aluno: {e.Message}");
    }
}

void Imprimir(Conexao db)
{
    Console.Clear();
    Console.WriteLine("-------IMPRIMIR -------");

    //===========Aluno===================

    Console.Write("Digite o código do aluno: ");
    string? codigoAlunoInput = Console.ReadLine();
    if (!int.TryParse(codigoAlunoInput, out int codigoAluno) || codigoAluno <= 0)
    {
        Console.WriteLine("Código inválido. Deve ser um número maior que zero.");
        return;
    }

    string sqlSelectAluno = "SELECT codigo FROM Alunos WHERE codigo = @CodigoAluno";
    int codigoAlunoExistente = 0;
    using (SqlCommand cmdSelect = new(sqlSelectAluno, db.conn))
    {
        cmdSelect.Parameters.AddWithValue("@CodigoAluno", codigoAluno);

        using SqlDataReader reader = cmdSelect.ExecuteReader();
        if (reader.Read())
        {
            codigoAlunoExistente = reader.GetInt32(0);
        }
    }

    if (codigoAlunoExistente == 0)
    {
        Console.WriteLine("Aluno não encontrado.");
        return;
    }

    //===========Quantidade de Impressão===================

    Console.Write("Digite a quantidade de páginas que serão impressas: ");
    string? qntdImpressaoInserida = Console.ReadLine();
    if (!int.TryParse(qntdImpressaoInserida, out int qntdImpressao) || qntdImpressao <= 0)
    {
        Console.WriteLine("Quantidade inválida. Deve ser um número maior que zero.");
        return;
    }

    string sqlSelectSaldo = "SELECT saldoImpressoes FROM Alunos WHERE codigo = @CodigoAluno";
    int saldoAluno = 0;
    using (SqlCommand cmdSelectSaldo = new(sqlSelectSaldo, db.conn))
    {
        cmdSelectSaldo.Parameters.AddWithValue("@CodigoAluno", codigoAluno);

        using SqlDataReader readerSaldo = cmdSelectSaldo.ExecuteReader();
        if (readerSaldo.Read())
        {
            saldoAluno = readerSaldo.GetInt32(0);
        }
    }

    if (saldoAluno < qntdImpressao)
    {
        Console.WriteLine("Saldo insuficiente para realizar a impressão.");
        return;
    }
    int saldoRetirar = saldoAluno - qntdImpressao;
    Console.WriteLine($"Saldo atual: {saldoRetirar} páginas.");

    //===========Data e Hora===================

    DateTime now = DateTime.Now;
    string dataHora = now.ToString("yyyy-MM-ddTHH:mm:ss");

    //===============Atualizar Saldo=======================

    try
    {
        string sqlUpdateSaldoImprimir = @"UPDATE Alunos SET saldoImpressoes = @SaldoRetirar WHERE codigo = @CodigoAluno";
        using SqlCommand cmdUpdateSaldo = new(sqlUpdateSaldoImprimir, db.conn);
        cmdUpdateSaldo.Parameters.AddWithValue("@SaldoRetirar", saldoRetirar);
        cmdUpdateSaldo.Parameters.AddWithValue("@CodigoAluno", codigoAluno);
        cmdUpdateSaldo.ExecuteNonQuery();

        Console.WriteLine("Sua impressão foi concluída com sucesso!");
    }
    catch (Exception e)
    {
        Console.WriteLine($"Erro ao imprimir: {e.Message}");
    }

    //=======================Inserir no banco=====================================

    try
    {
        string sqlImprimir = @"INSERT INTO Impressoes (codigoAluno, dataHora, quantidade) 
                    VALUES (@CodigoAluno, @DataHora, @Quantidade)";
        using SqlCommand cmdInsertImprimir = new(sqlImprimir, db.conn);
        cmdInsertImprimir.Parameters.AddWithValue("@CodigoAluno", codigoAluno);
        cmdInsertImprimir.Parameters.AddWithValue("@DataHora", dataHora);
        cmdInsertImprimir.Parameters.AddWithValue("@Quantidade", qntdImpressao);
        cmdInsertImprimir.ExecuteNonQuery();
    }
    catch (Exception e)
    {
        Console.WriteLine($"Falha ao armazenar dados!!! {e.Message}");
    }
}

void ComprarFichas(Conexao db)
{
    Console.Clear();
    Console.WriteLine("------- COMPRA DE FICHA -------");
    Console.WriteLine("");
    Console.Write("Digite o código do aluno: ");
    string? codigoAlunoInput = Console.ReadLine();
    if (!int.TryParse(codigoAlunoInput, out int codigoAluno) || codigoAluno <= 0)
    {
        Console.WriteLine("");
        Console.WriteLine("Código inválido. Deve ser um número maior que zero.");
        return;
    }

    //----------------Aluno---------------

    string sqlSelectAluno = "SELECT codigo FROM Alunos WHERE codigo = @CodigoAluno";
    int codigoAlunoExistente = 0;
    using (SqlCommand cmdSelect = new(sqlSelectAluno, db.conn))
    {
        cmdSelect.Parameters.AddWithValue("@CodigoAluno", codigoAluno);

        using SqlDataReader reader = cmdSelect.ExecuteReader();
        if (reader.Read())
        {
            codigoAlunoExistente = reader.GetInt32(0);
        }
    }

    if (codigoAlunoExistente == 0)
    {
        Console.WriteLine("");
        Console.WriteLine("Aluno não encontrado.");
        return;
    }
    //-------------Data e Hora------------------------------

    DateTime now = DateTime.Now;
    string dataHora = now.ToString("yyyy-MM-ddTHH:mm:ss");

    //-------------Quantidade-------------------------------
    Console.Clear();
    do
    {
        Console.WriteLine("------ ESCOLHA O TICKTEC -----");
        Console.WriteLine("");
        Console.WriteLine("Digite a quantidade de tickets que deseja comprar:");
        Console.WriteLine("Tickets válidos: 25 e 50:");

        string? ticket = Console.ReadLine(); // Use nullable string to handle potential null values
        if (ticket == "25" || ticket == "50")
        {
            int quantidadeTickets = int.Parse(ticket);
            Console.WriteLine("");
            Console.WriteLine($"Você escolheu comprar {quantidadeTickets} tickets.");
            Console.WriteLine("Pressione qualquer tecla para confirmar a compra...");
            Console.ReadKey();

            string sqlUpdateSaldo = @"UPDATE Alunos SET saldoImpressoes = saldoImpressoes + @Quantidade WHERE codigo = @CodigoAluno";
            using var cmdUpdateSaldo = new SqlCommand(sqlUpdateSaldo, db.conn);
            cmdUpdateSaldo.Parameters.AddWithValue("@Quantidade", quantidadeTickets);
            cmdUpdateSaldo.Parameters.AddWithValue("@CodigoAluno", codigoAluno);
            cmdUpdateSaldo.ExecuteNonQuery();

    //----------------------Inserir no banco----------------------------

            try
            {
                string sqlInsertComprar = @"INSERT INTO Compras (codigoAluno, dataHora, quantidade)
                                            VALUES (@CodigoAluno, @DataHora, @Quantidade)";
                using var cmdInsertComprar = new SqlCommand(sqlInsertComprar, db.conn); // Simplify 'new' expression and use Microsoft.Data.SqlClient
                cmdInsertComprar.Parameters.AddWithValue("@CodigoAluno", codigoAluno);
                cmdInsertComprar.Parameters.AddWithValue("@DataHora", dataHora);
                cmdInsertComprar.Parameters.AddWithValue("@Quantidade", ticket);
                cmdInsertComprar.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("");
                Console.WriteLine($"Falha ao inserir dados no banco {e.Message}");
            }
            Console.WriteLine("");
            Console.WriteLine("Compra realizada com sucesso!");
            break;
        }
        else
        {
            Console.WriteLine("");
            Console.WriteLine("Opção inválida. (Tickets válidos: 25 e 50)");
        }
    } while (false);
}

void ConsultarSaldoAluno(Conexao db)
{
    Console.Clear();
    Console.WriteLine("------- CONSULTAR SALDO ALUNO -------");
    Console.WriteLine("");
    Console.Write("Digite o código do aluno: ");
    string? codigoAlunoInserido = Console.ReadLine();
    if (!int.TryParse(codigoAlunoInserido, out int codigoAluno) || codigoAluno <= 0)
    {
        Console.WriteLine("");
        Console.WriteLine("Código inválido. Deve ser um número maior que zero.");
        return;
    }

    string sqlSelectAluno = "SELECT codigo FROM Alunos WHERE codigo = @CodigoAluno";
    int codigoAlunoExistente = 0;
    using (SqlCommand cmdSelect = new(sqlSelectAluno, db.conn))
    {
        cmdSelect.Parameters.AddWithValue("@CodigoAluno", codigoAluno);

        using SqlDataReader reader = cmdSelect.ExecuteReader();
        if (reader.Read())
        {
            codigoAlunoExistente = reader.GetInt32(0);
        }
    }

    if (codigoAlunoExistente == 0)
    {
        Console.WriteLine("");
        Console.WriteLine("Aluno não encontrado.");
        return;
    }
    string sqlSelectSaldo = "SELECT SaldoImpressoes, Nome FROM Alunos WHERE Codigo = @CodigoAluno";

    int saldoAluno = 0;
    string nomeAluno = "";
    using (SqlCommand cmdSelectSaldo = new(sqlSelectSaldo, db.conn))
    {
        cmdSelectSaldo.Parameters.AddWithValue("@CodigoAluno", codigoAluno);
        using SqlDataReader readerSaldo = cmdSelectSaldo.ExecuteReader();
        if (readerSaldo.Read())
        {
            saldoAluno = readerSaldo.GetInt32(0);
            nomeAluno = readerSaldo.GetString(1);
        }
    }
    if (saldoAluno < 0)
    {
        Console.WriteLine("");
        Console.WriteLine("Sem saldo.");
    }
    else
    {
        Console.WriteLine("");
        Console.WriteLine($"Saldo atual do aluno: {nomeAluno} é de {saldoAluno} páginas.");
    }
}

void ConsultarHistoricoImpressao(Conexao db)
{
    Console.Clear();
    Console.WriteLine("------ CONSULTAR HISTORICO DE IMPRESSOES ------");
    Console.WriteLine("");
    Console.WriteLine("[1] - Consultar histórico de impressões de alunos");
    Console.WriteLine("[2] - Consultar o histórico das 10 últimas de impressões");
    Console.WriteLine("[3] - Voltar");

    if (int.TryParse(Console.ReadLine(), out int opcaoHistoricoImp)) // Removed unnecessary semicolon
    {
        switch (opcaoHistoricoImp)
        {


//==================Consultar histórico de impressões de alunos========================


            case 1:
                Console.Clear();
                Console.WriteLine("------- CONSULTAR HISTORICO DE IMPRESSOES DO ALUNO ------");
                Console.WriteLine("");
                Console.Write("Digite o código do aluno: ");
                string? codigoAlunoInserido = Console.ReadLine();
                if (!int.TryParse(codigoAlunoInserido, out int codigoAluno) || codigoAluno <= 0)
                {
                    Console.WriteLine("");
                    Console.WriteLine("Código inválido. Deve ser um número maior que zero.");
                    return;
                }

                string sqlSelectAluno = "SELECT codigoAluno FROM Impressoes WHERE codigoAluno = @CodigoAluno";
                int codigoAlunoExistente = 0;
                using (SqlCommand cmdSelect = new(sqlSelectAluno, db.conn))
                {
                    cmdSelect.Parameters.AddWithValue("@CodigoAluno", codigoAluno);

                    using SqlDataReader reader = cmdSelect.ExecuteReader();
                    if (reader.Read())
                    {
                        codigoAlunoExistente = reader.GetInt32(0);
                    }
                }

                if (codigoAlunoExistente == 0)
                {
                    Console.WriteLine("");
                    Console.WriteLine("Aluno não encontrado ou sem histórico recente.");
                    return;
                }

                try
                {
                    string sqlSelectHistImp = "SELECT codigo, dataHora, quantidade FROM Impressoes WHERE codigoAluno = @CodigoAluno";
                    using (SqlCommand cmdSelectHistImp = new(sqlSelectHistImp, db.conn))
                    {
                        cmdSelectHistImp.Parameters.AddWithValue("@CodigoAluno", codigoAluno);

                        using SqlDataReader reader = cmdSelectHistImp.ExecuteReader();
                        while (reader.Read())
                        {
                            int codigoImpressao = reader.GetInt32(0);
                            string dataImpressao = reader.GetDateTime(1).ToString("yyyy-MM-dd HH:mm:ss");
                            int quantidadeImpressao = reader.GetInt32(2);

                            Console.WriteLine("");
                            Console.WriteLine($"Código da impressão: {codigoImpressao} | Data e Hora: {dataImpressao} | Quantidade: {quantidadeImpressao}");
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Erro ao consultar histórico: {e.Message}");
                }
                break;


//==================Consultar as 10 últimas impressões========================


            case 2:
                Console.Clear();
                Console.WriteLine("------- CONSULTAR AS 10 ÚLTIMAS IMPRESSÕES -------");
                try
                {
                    string sqlSelectTop10 = "SELECT TOP 10 * FROM Impressoes ORDER BY dataHora DESC";
                    using (SqlCommand cmdSelectLast10 = new(sqlSelectTop10, db.conn))
                    {
                        using SqlDataReader reader = cmdSelectLast10.ExecuteReader();
                        while (reader.Read())
                        {
                            int codigoImpressao = reader.GetInt32(0);
                            int codigoAlunoTop10 = reader.GetInt32(1);
                            string dataImpressao = reader.GetDateTime(2).ToString("yyyy-MM-dd HH:mm:ss");
                            int quantidadeImpressao = reader.GetInt32(3);

                            Console.WriteLine("");
                            Console.WriteLine($"Código da impressão: {codigoImpressao} | Código do aluno: {codigoAlunoTop10} | Data e Hora: {dataImpressao} | Quantidade: {quantidadeImpressao}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao consultar histórico: {ex.Message}");
                }
                break;

//====================Voltar Menu===================================

            case 3:
                Console.WriteLine("");
                Console.WriteLine("Voltando ao menu principal...");
                break;

            default:
                Console.WriteLine("");
                Console.WriteLine("Opção inválida. Tente novamente.");
                break;
        }
    }
    else
    {
        Console.WriteLine("");
        Console.WriteLine("Entrada inválida. Por favor, insira um número.");
    }
}

void ConsultarHistoricoMovimentacao(Conexao db)
    {
    Console.Clear();
    Console.WriteLine("------- CONSULTAR HISTORICO DE IMPRESSOES -------");
    Console.WriteLine("");
    Console.WriteLine("[1] - Consultar histórico de compras de alunos");
    Console.WriteLine("[2] - Consultar o histórico das 10 últimas de compras");
    Console.WriteLine("[3] - Voltar");

    if (int.TryParse(Console.ReadLine(), out int opcaoHistoricoImp)) 
    {
        switch (opcaoHistoricoImp)
        {


            //==================Consultar histórico de Compras de alunos========================


            case 1:
                Console.Clear();
                Console.WriteLine("------- CONSULTAR HISTORICO DE IMPRESSOES DO ALUNO -------");
                Console.WriteLine("");
                Console.Write("Digite o código do aluno: ");
                string? codigoAlunoInserido = Console.ReadLine();
                if (!int.TryParse(codigoAlunoInserido, out int codigoAluno) || codigoAluno <= 0)
                {
                    Console.WriteLine("");
                    Console.WriteLine("Código inválido. Deve ser um número maior que zero.");
                    return;
                }

                string sqlSelectAluno = "SELECT codigoAluno FROM Impressoes WHERE codigoAluno = @CodigoAluno";
                int codigoAlunoExistente = 0;
                using (SqlCommand cmdSelect = new(sqlSelectAluno, db.conn))
                {
                    cmdSelect.Parameters.AddWithValue("@CodigoAluno", codigoAluno);

                    using SqlDataReader reader = cmdSelect.ExecuteReader();
                    if (reader.Read())
                    {
                        codigoAlunoExistente = reader.GetInt32(0);
                    }
                }

                if (codigoAlunoExistente == 0)
                {
                    Console.WriteLine("");
                    Console.WriteLine("Aluno não encontrado ou sem histórico recente.");
                    return;
                }

                try
                {
                    string sqlSelectHistImp = "SELECT codigo, dataHora, quantidade FROM Compras WHERE codigoAluno = @CodigoAluno";
                    using (SqlCommand cmdSelectHistImp = new(sqlSelectHistImp, db.conn))
                    {
                        cmdSelectHistImp.Parameters.AddWithValue("@CodigoAluno", codigoAluno);

                        using SqlDataReader reader = cmdSelectHistImp.ExecuteReader();
                        while (reader.Read())
                        {
                            int codigoCompras = reader.GetInt32(0);
                            string dataCompras = reader.GetDateTime(1).ToString("yyyy-MM-dd HH:mm:ss");
                            int quantidadeCompras = reader.GetInt32(2);

                            Console.WriteLine("");
                            Console.WriteLine($"Código da impressão: {codigoCompras} | Data e Hora: {dataCompras} | Quantidade: {quantidadeCompras}");
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("");
                    Console.WriteLine($"Erro ao consultar histórico: {e.Message}");
                }
                break;


            //==================Consultar as 10 últimas Compras========================


            case 2:
                Console.Clear();
                Console.WriteLine("------- CONSULTAR AS 10 ÚLTIMAS IMPRESSÕES -------");
                try
                {
                    string sqlSelectTop10 = "SELECT TOP 10 * FROM Compras ORDER BY dataHora DESC";
                    using (SqlCommand cmdSelectLast10 = new(sqlSelectTop10, db.conn))
                    {
                        using SqlDataReader reader = cmdSelectLast10.ExecuteReader();
                        while (reader.Read())
                        {
                            int codigoCompras = reader.GetInt32(0);
                            int codigoAlunoCompras = reader.GetInt32(1);
                            string dataCompras = reader.GetDateTime(2).ToString("yyyy-MM-dd HH:mm:ss");
                            int quantidadeCompras = reader.GetInt32(3);

                            Console.WriteLine("");
                            Console.WriteLine($"Código da impressão: {codigoCompras} | Código do aluno: {codigoAlunoCompras} | Data e Hora: {dataCompras} | Quantidade: {quantidadeCompras}");
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("");
                    Console.WriteLine($"Erro ao consultar histórico: {e.Message}");
                }
                break;

            //====================Voltar Menu===================================

            case 3:
                Console.WriteLine("");
                Console.WriteLine("Voltando ao menu principal...");
                break;

            default:
                Console.WriteLine("");
                Console.WriteLine("Opção inválida. Tente novamente.");
                break;
        }
    }
    else
    {
        Console.WriteLine("");
        Console.WriteLine("Entrada inválida. Por favor, insira um número.");
    }
}

void ConsultarCodigoAluno(Conexao db)
{
    {
        Console.Clear();
        Console.WriteLine("------- CONSULTAR CÓDIGO DO ALUNO -------");
        Console.WriteLine("");
        Console.Write("Digite o nome do aluno: ");
        string? nomeAluno = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(nomeAluno))
        {
            Console.WriteLine("");
            Console.WriteLine("Nome inválido. Deve ser preenchido.");
            return;
        }

        try
        {
            string sqlSelectCodigo = "SELECT codigo, nome, cpf FROM Alunos WHERE nome LIKE @Nome";
            using (SqlCommand cmdSelect = new(sqlSelectCodigo, db.conn))
            {
                cmdSelect.Parameters.AddWithValue("@Nome", "%" + nomeAluno + "%");

                using SqlDataReader reader = cmdSelect.ExecuteReader();
                if (reader.Read())
                {
                    int codigoAluno = reader.GetInt32(0);
                    string nome = reader.GetString(1);
                    string cpf = reader.GetString(2);

                    Console.WriteLine("");
                    Console.WriteLine($"Código do Aluno: {codigoAluno} | Nome: {nome} | CPF: {cpf}");
                }
                else
                {
                    Console.WriteLine("");
                    Console.WriteLine("Aluno não encontrado.");
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("");
            Console.WriteLine($"Erro ao consultar código do aluno: {e.Message}");
        }
    }

}

static bool ValidarCpf(string cpf)
{
    int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
    int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
    string tempCpf;
    string digito;
    int soma;
    int resto;
    cpf = cpf.Trim();
    cpf = cpf.Replace(".", "").Replace("-", "");
    if (cpf.Length != 11)
        return false;
    tempCpf = cpf.Substring(0, 9);
    soma = 0;

    for (int i = 0; i < 9; i++)
        soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];
    resto = soma % 11;
    if (resto < 2)
        resto = 0;
    else
        resto = 11 - resto;
    digito = resto.ToString();
    tempCpf = tempCpf + digito;
    soma = 0;
    for (int i = 0; i < 10; i++)
        soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];
    resto = soma % 11;
    if (resto < 2)
        resto = 0;
    else
        resto = 11 - resto;
    digito = digito + resto.ToString();
    return cpf.EndsWith(digito);
}

static bool ValidarCep(string cep)
{
    if (string.IsNullOrWhiteSpace(cep))
        return false;
    cep = Regex.Replace(cep, "[^0-9]", "");
        return cep.Length == 8;
}
	
string FormatarCpf(string cpf)
{
    cpf = Regex.Replace(cpf, "[^0-9]", "");
    if (cpf.Length == 11)
        return Convert.ToUInt64(cpf).ToString(@"000\.000\.000\-00");
    return cpf;
}

string FormatarCep(string cep)
{
    cep = Regex.Replace(cep, "[^0-9]", "");
    if (cep.Length == 8)
        return Convert.ToUInt64(cep).ToString(@"00000\-000");
    return cep;
}

static string FormatarData(string dataNascimento)
{
    dataNascimento = Regex.Replace(dataNascimento, "[^0-9]", "");
    if (dataNascimento.Length == 8)
    {
        if (DateTime.TryParseExact(dataNascimento, "ddMMyyyy",
            CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime data))
        {
            return data.ToString("dd/MM/yyyy"); 
        }
    }

    return "Data inválida!";
}    
   
