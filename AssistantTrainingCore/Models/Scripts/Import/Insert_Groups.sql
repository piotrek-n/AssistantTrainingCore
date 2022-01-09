USE [Trainings]
GO
select * from [dbo].[Groups]
--DBCC CHECKIDENT ('[Groups]', RESEED, 0);
--GO

INSERT INTO [dbo].[Groups] (GroupName,TimeOfCreation,TimeOfModification,Tag)
SELECT [Grupa],GETDATE(),GETDATE(),''
FROM
(
SELECT DISTINCT [Grupa]
FROM [dbo].[InstructionsTemp]
) tab