------------------------------------------------------------
-- CRIAÇÃO DO BANCO
------------------------------------------------------------
IF DB_ID('QuestionariosDb') IS NULL
BEGIN
    CREATE DATABASE QuestionariosDb;
END
GO

USE QuestionariosDb;
GO

------------------------------------------------------------
-- DROP TABELAS (DEV ONLY)
------------------------------------------------------------
IF OBJECT_ID('dbo.AggregatedResults', 'U') IS NOT NULL DROP TABLE dbo.AggregatedResults;
IF OBJECT_ID('dbo.ResponseItems', 'U')    IS NOT NULL DROP TABLE dbo.ResponseItems;
IF OBJECT_ID('dbo.Responses', 'U')        IS NOT NULL DROP TABLE dbo.Responses;
IF OBJECT_ID('dbo.Options', 'U')          IS NOT NULL DROP TABLE dbo.Options;
IF OBJECT_ID('dbo.Questions', 'U')        IS NOT NULL DROP TABLE dbo.Questions;
IF OBJECT_ID('dbo.Surveys', 'U')          IS NOT NULL DROP TABLE dbo.Surveys;
IF OBJECT_ID('dbo.Users', 'U')            IS NOT NULL DROP TABLE dbo.Users;
GO

------------------------------------------------------------
-- TABELAS
------------------------------------------------------------

CREATE TABLE dbo.Surveys (
    Id           UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_Surveys PRIMARY KEY,
    Title        NVARCHAR(200)    NOT NULL,
    StartAt      DATETIME2(0)     NOT NULL,
    EndAt        DATETIME2(0)     NOT NULL,
    IsClosed     BIT              NOT NULL CONSTRAINT DF_Surveys_IsClosed DEFAULT(0),
    CreatedAt    DATETIME2(0)     NOT NULL CONSTRAINT DF_Surveys_CreatedAt DEFAULT(SYSUTCDATETIME()),
    UpdatedAt    DATETIME2(0)     NULL
);
GO

CREATE INDEX IX_Surveys_Start_End
    ON dbo.Surveys (StartAt, EndAt);
GO

CREATE TABLE dbo.Questions (
    Id        UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_Questions PRIMARY KEY,
    SurveyId  UNIQUEIDENTIFIER NOT NULL,
    [Text]    NVARCHAR(500)    NOT NULL,
    [Order]   INT              NULL,
    CreatedAt DATETIME2(0)     NOT NULL CONSTRAINT DF_Questions_CreatedAt DEFAULT(SYSUTCDATETIME())
);
GO

ALTER TABLE dbo.Questions
ADD CONSTRAINT FK_Questions_Surveys
    FOREIGN KEY (SurveyId) REFERENCES dbo.Surveys (Id)
    ON DELETE CASCADE;
GO

CREATE INDEX IX_Questions_SurveyId
    ON dbo.Questions (SurveyId);
GO

CREATE TABLE dbo.Options (
    Id         UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_Options PRIMARY KEY,
    QuestionId UNIQUEIDENTIFIER NOT NULL,
    [Text]     NVARCHAR(200)    NOT NULL,
    [Order]    INT              NULL,
    CreatedAt  DATETIME2(0)     NOT NULL CONSTRAINT DF_Options_CreatedAt DEFAULT(SYSUTCDATETIME())
);
GO

ALTER TABLE dbo.Options
ADD CONSTRAINT FK_Options_Questions
    FOREIGN KEY (QuestionId) REFERENCES dbo.Questions (Id)
    ON DELETE CASCADE;
GO

CREATE INDEX IX_Options_QuestionId
    ON dbo.Options (QuestionId);
GO

CREATE TABLE dbo.Responses (
    Id         UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_Responses PRIMARY KEY,
    SurveyId   UNIQUEIDENTIFIER NOT NULL,
    AnsweredAt DATETIME2(0)     NOT NULL,
    CreatedAt  DATETIME2(0)     NOT NULL CONSTRAINT DF_Responses_CreatedAt DEFAULT(SYSUTCDATETIME())
);
GO

ALTER TABLE dbo.Responses
ADD CONSTRAINT FK_Responses_Surveys
    FOREIGN KEY (SurveyId) REFERENCES dbo.Surveys (Id)
    ON DELETE CASCADE;
GO

CREATE INDEX IX_Responses_SurveyId_AnsweredAt
    ON dbo.Responses (SurveyId, AnsweredAt);
GO

CREATE TABLE dbo.ResponseItems (
    Id          UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_ResponseItems PRIMARY KEY,
    ResponseId  UNIQUEIDENTIFIER NOT NULL,
    QuestionId  UNIQUEIDENTIFIER NOT NULL,
    OptionId    UNIQUEIDENTIFIER NOT NULL
);
GO

ALTER TABLE dbo.ResponseItems
ADD CONSTRAINT FK_ResponseItems_Responses
    FOREIGN KEY (ResponseId) REFERENCES dbo.Responses (Id)
    ON DELETE CASCADE;
GO

ALTER TABLE dbo.ResponseItems
ADD CONSTRAINT FK_ResponseItems_Questions
    FOREIGN KEY (QuestionId) REFERENCES dbo.Questions (Id);
GO

ALTER TABLE dbo.ResponseItems
ADD CONSTRAINT FK_ResponseItems_Options
    FOREIGN KEY (OptionId) REFERENCES dbo.Options (Id);
GO

CREATE INDEX IX_ResponseItems_ResponseId
    ON dbo.ResponseItems (ResponseId);
GO

CREATE INDEX IX_ResponseItems_Question_Option
    ON dbo.ResponseItems (QuestionId, OptionId);
GO

CREATE TABLE dbo.AggregatedResults (
    SurveyId   UNIQUEIDENTIFIER NOT NULL,
    QuestionId UNIQUEIDENTIFIER NOT NULL,
    OptionId   UNIQUEIDENTIFIER NOT NULL,
    Votes      INT              NOT NULL CONSTRAINT DF_AggregatedResults_Votes DEFAULT(0),
    CONSTRAINT PK_AggregatedResults PRIMARY KEY (SurveyId, QuestionId, OptionId)
);
GO

ALTER TABLE dbo.AggregatedResults
ADD CONSTRAINT FK_AggregatedResults_Surveys
    FOREIGN KEY (SurveyId) REFERENCES dbo.Surveys (Id)
    ON DELETE CASCADE;
GO

ALTER TABLE dbo.AggregatedResults
ADD CONSTRAINT FK_AggregatedResults_Questions
    FOREIGN KEY (QuestionId) REFERENCES dbo.Questions (Id)
    ON DELETE CASCADE;
GO

ALTER TABLE dbo.AggregatedResults
ADD CONSTRAINT FK_AggregatedResults_Options
    FOREIGN KEY (OptionId) REFERENCES dbo.Options (Id)
    ON DELETE CASCADE;
GO

CREATE TABLE dbo.Users (
    Id           UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_Users PRIMARY KEY,
    [Name]       NVARCHAR(150)    NOT NULL,
    Email        NVARCHAR(200)    NOT NULL,
    PasswordHash VARBINARY(64)    NOT NULL,
    CreatedAt    DATETIME2(0)     NOT NULL CONSTRAINT DF_Users_CreatedAt DEFAULT(SYSUTCDATETIME())
);
GO

CREATE UNIQUE INDEX IX_Users_Email
    ON dbo.Users (Email);
GO

------------------------------------------------------------
-- DADOS DE EXEMPLO – 1 SURVEY, 5 PERGUNTAS
------------------------------------------------------------

DECLARE @SurveyId UNIQUEIDENTIFIER = '11111111-1111-1111-1111-111111111111';

DECLARE @Q1 UNIQUEIDENTIFIER = '22222222-2222-2222-2222-222222222221';
DECLARE @Q2 UNIQUEIDENTIFIER = '22222222-2222-2222-2222-222222222222';
DECLARE @Q3 UNIQUEIDENTIFIER = '22222222-2222-2222-2222-222222222223';
DECLARE @Q4 UNIQUEIDENTIFIER = '22222222-2222-2222-2222-222222222224';
DECLARE @Q5 UNIQUEIDENTIFIER = '22222222-2222-2222-2222-222222222225';

DECLARE @O11 UNIQUEIDENTIFIER = '33333333-3333-3333-3333-333333333331';
DECLARE @O12 UNIQUEIDENTIFIER = '33333333-3333-3333-3333-333333333332';
DECLARE @O13 UNIQUEIDENTIFIER = '33333333-3333-3333-3333-333333333333';

DECLARE @O21 UNIQUEIDENTIFIER = '44444444-4444-4444-4444-444444444441';
DECLARE @O22 UNIQUEIDENTIFIER = '44444444-4444-4444-4444-444444444442';
DECLARE @O23 UNIQUEIDENTIFIER = '44444444-4444-4444-4444-444444444443';

DECLARE @O31 UNIQUEIDENTIFIER = '55555555-5555-5555-5555-555555555551';
DECLARE @O32 UNIQUEIDENTIFIER = '55555555-5555-5555-5555-555555555552';
DECLARE @O33 UNIQUEIDENTIFIER = '55555555-5555-5555-5555-555555555553';

DECLARE @O41 UNIQUEIDENTIFIER = '66666666-6666-6666-6666-666666666661';
DECLARE @O42 UNIQUEIDENTIFIER = '66666666-6666-6666-6666-666666666662';
DECLARE @O43 UNIQUEIDENTIFIER = '66666666-6666-6666-6666-666666666663';

DECLARE @O51 UNIQUEIDENTIFIER = '77777777-7777-7777-7777-777777777771';
DECLARE @O52 UNIQUEIDENTIFIER = '77777777-7777-7777-7777-777777777772';
DECLARE @O53 UNIQUEIDENTIFIER = '77777777-7777-7777-7777-777777777773';

DECLARE @UserAdmin UNIQUEIDENTIFIER = '99999999-1111-2222-3333-444444444444';
DECLARE @UserEditor UNIQUEIDENTIFIER = '99999999-1111-2222-3333-444444444445';

-- USERS (admin + editor)
INSERT INTO dbo.Users (Id, [Name], Email, PasswordHash)
VALUES
(@UserAdmin, 'Administrador', 'admin@questionarios.com', HASHBYTES('SHA2_256', 'Admin@123')),
(@UserEditor, 'Editor', 'editor@questionarios.com', HASHBYTES('SHA2_256', 'Editor@123'));

-- Survey
INSERT INTO dbo.Surveys (Id, Title, StartAt, EndAt, IsClosed)
VALUES (
    @SurveyId,
    N'Pesquisa Eleitoral 2026 - Intenção de voto para Prefeito',
    DATEADD(DAY, -1, SYSUTCDATETIME()),
    DATEADD(DAY, 7, SYSUTCDATETIME()),
    0
);

-- PERGUNTAS (total 5)
INSERT INTO dbo.Questions (Id, SurveyId, [Text], [Order])
VALUES
(@Q1, @SurveyId, N'Se a eleição para prefeito fosse hoje, em quem você votaria?', 1),
(@Q2, @SurveyId, N'Você pretende comparecer para votar nas próximas eleições?', 2),
(@Q3, @SurveyId, N'Qual faixa etária você se encontra?', 3),
(@Q4, @SurveyId, N'Qual é o seu nível de interesse em política?', 4),
(@Q5, @SurveyId, N'Com que frequência você acompanha notícias sobre política?', 5);

-- OPÇÕES Q1 (intenção de voto)
INSERT INTO dbo.Options (Id, QuestionId, [Text], [Order])
VALUES
(@O11, @Q1, N'Candidato A', 1),
(@O12, @Q1, N'Candidato B', 2),
(@O13, @Q1, N'Branco/Nulo/Não sabe', 3);

-- OPÇÕES Q2 (comparecimento)
INSERT INTO dbo.Options (Id, QuestionId, [Text], [Order])
VALUES
(@O21, @Q2, N'Sim', 1),
(@O22, @Q2, N'Não', 2),
(@O23, @Q2, N'Não sabe', 3);

-- OPÇÕES Q3 (faixa etária)
INSERT INTO dbo.Options (Id, QuestionId, [Text], [Order])
VALUES
(@O31, @Q3, N'16-24 anos', 1),
(@O32, @Q3, N'25-39 anos', 2),
(@O33, @Q3, N'40 anos ou mais', 3);

-- OPÇÕES Q4 (interesse em política)
INSERT INTO dbo.Options (Id, QuestionId, [Text], [Order])
VALUES
(@O41, @Q4, N'Alto', 1),
(@O42, @Q4, N'Médio', 2),
(@O43, @Q4, N'Baixo', 3);

-- OPÇÕES Q5 (frequência notícias)
INSERT INTO dbo.Options (Id, QuestionId, [Text], [Order])
VALUES
(@O51, @Q5, N'Todos os dias', 1),
(@O52, @Q5, N'Algumas vezes na semana', 2),
(@O53, @Q5, N'Raramente/Nunca', 3);

------------------------------------------------------------
-- ALGUMAS RESPOSTAS MANUAIS (2 RESPONDENTES)
------------------------------------------------------------
DECLARE @R1 UNIQUEIDENTIFIER = '88888888-8888-8888-8888-888888888881';
DECLARE @R2 UNIQUEIDENTIFIER = '88888888-8888-8888-8888-888888888882';

INSERT INTO dbo.Responses (Id, SurveyId, AnsweredAt)
VALUES
(@R1, @SurveyId, DATEADD(MINUTE, -30, SYSUTCDATETIME())),
(@R2, @SurveyId, DATEADD(MINUTE, -10, SYSUTCDATETIME()));

-- R1
INSERT INTO dbo.ResponseItems (Id, ResponseId, QuestionId, OptionId)
VALUES
(NEWID(), @R1, @Q1, @O11),
(NEWID(), @R1, @Q2, @O21),
(NEWID(), @R1, @Q3, @O32),
(NEWID(), @R1, @Q4, @O41),
(NEWID(), @R1, @Q5, @O51);

-- R2
INSERT INTO dbo.ResponseItems (Id, ResponseId, QuestionId, OptionId)
VALUES
(NEWID(), @R2, @Q1, @O12),
(NEWID(), @R2, @Q2, @O23),
(NEWID(), @R2, @Q3, @O33),
(NEWID(), @R2, @Q4, @O42),
(NEWID(), @R2, @Q5, @O52);

------------------------------------------------------------
-- 1000 RESPOSTAS FAKE (para teste de performance)
------------------------------------------------------------
DECLARE @i INT = 1;
DECLARE @Total INT = 1000;

WHILE @i <= @Total
BEGIN
    DECLARE @RespId UNIQUEIDENTIFIER = NEWID();
    DECLARE @OffsetMinutes INT = ABS(CHECKSUM(NEWID())) % 10000;

    INSERT INTO dbo.Responses (Id, SurveyId, AnsweredAt)
    VALUES (@RespId, @SurveyId, DATEADD(MINUTE, -@OffsetMinutes, SYSUTCDATETIME()));

    -- Para cada pergunta, escolhe uma opção random
    -- Q1
    INSERT INTO dbo.ResponseItems (Id, ResponseId, QuestionId, OptionId)
    SELECT TOP 1 NEWID(), @RespId, @Q1, Id
    FROM dbo.Options
    WHERE QuestionId = @Q1
    ORDER BY NEWID();

    -- Q2
    INSERT INTO dbo.ResponseItems (Id, ResponseId, QuestionId, OptionId)
    SELECT TOP 1 NEWID(), @RespId, @Q2, Id
    FROM dbo.Options
    WHERE QuestionId = @Q2
    ORDER BY NEWID();

    -- Q3
    INSERT INTO dbo.ResponseItems (Id, ResponseId, QuestionId, OptionId)
    SELECT TOP 1 NEWID(), @RespId, @Q3, Id
    FROM dbo.Options
    WHERE QuestionId = @Q3
    ORDER BY NEWID();

    -- Q4
    INSERT INTO dbo.ResponseItems (Id, ResponseId, QuestionId, OptionId)
    SELECT TOP 1 NEWID(), @RespId, @Q4, Id
    FROM dbo.Options
    WHERE QuestionId = @Q4
    ORDER BY NEWID();

    -- Q5
    INSERT INTO dbo.ResponseItems (Id, ResponseId, QuestionId, OptionId)
    SELECT TOP 1 NEWID(), @RespId, @Q5, Id
    FROM dbo.Options
    WHERE QuestionId = @Q5
    ORDER BY NEWID();

    SET @i += 1;
END
GO

------------------------------------------------------------
-- POPULAR AggregatedResults COM BASE NAS RESPOSTAS
------------------------------------------------------------
TRUNCATE TABLE dbo.AggregatedResults;
GO

INSERT INTO dbo.AggregatedResults (SurveyId, QuestionId, OptionId, Votes)
SELECT
    r.SurveyId,
    ri.QuestionId,
    ri.OptionId,
    COUNT(*) AS Votes
FROM dbo.Responses r
JOIN dbo.ResponseItems ri ON ri.ResponseId = r.Id
GROUP BY r.SurveyId, ri.QuestionId, ri.OptionId;
GO
