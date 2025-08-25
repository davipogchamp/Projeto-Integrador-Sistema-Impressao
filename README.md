CREATE TABLE Alunos(

codigo INT IDENTITY PRIMARY KEY,
nome VARCHAR(101),
email VARCHAR(101),
dataNascimento DATE,
cpf VARCHAR(15) UNIQUE,
cep VARCHAR(10),
numero INT,
saldoImpressoes INT,
)

CREATE TABLE Impressoes(
codigo INT IDENTITY PRIMARY KEY,
codigoAluno INT,
dataHora DATETIME,
quantidade INT,

CONSTRAINT FK_CodigoAluno_Impressoes
	FOREIGN KEY(codigoAluno) 
		REFERENCES Alunos(codigo) 
)

CREATE TABLE Compras( 
codigo INT IDENTITY PRIMARY KEY,
codigoAluno INT,
dataHora DATETIME,
quantidade INT,

CONSTRAINT FK_CodigoAluno_Compras
	FOREIGN KEY(codigoAluno) 
		REFERENCES Alunos(codigo) 
)
