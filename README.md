CREATE DATABASE SistemaImpressoes

CREATE TABLE Alunos(

[Codigo] INT IDENTITY PRIMARY KEY,
[Nome] VARCHAR(101), [Email] VARCHAR(101),
[DataNascimento] DATE,
[Cpf] VARCHAR(15),
[Cep] VARCHAR(10),
[Numero] INT,
[SaldoImpressoes] INT,
)


CREATE TABLE Impressoes(
[Codigo] INT IDENTITY PRIMARY KEY,
[CodigoAluno] INT,
[DataHora] DATETIME,
[Quantidade] INT,

CONSTRAINT FK_CodigoAluno_Impressoes
	FOREIGN KEY([CodigoAluno])
		REFERENCES Alunos([Codigo])
) 

CREATE TABLE Compras(
[Codigo] INT IDENTITY PRIMARY KEY,
[CodigoAluno] INT,
[DataHora] DATETIME,
[Quantidade] INT,

CONSTRAINT FK_CodigoAluno_Compras
	FOREIGN KEY([CodigoAluno])
		REFERENCES Alunos([Codigo])
)

SELECT * FROM Impressoes

