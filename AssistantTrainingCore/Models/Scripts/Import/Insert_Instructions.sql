
INSERT INTO [dbo].[Instructions]
           ([Name]
           ,[Number]
           ,[Version]
           ,[TimeOfCreation]
           ,[TimeOfModification]
           ,[Tag]
           )
SELECT [NazwaInstrukcji],[NumerInstrukcji],[Wersja],GETDATE(),GETDATE(),[NazwaProduktu]
FROM
(
  SELECT 
		 [NazwaInstrukcji]
		,[NumerInstrukcji]
		,[Wersja]
		,[NazwaProduktu]
  FROM [Trainings].[dbo].[InstructionsTemp]
  Where [NazwaInstrukcji] IS NOT NULL
) tab

--DELETE FROM Instructions
--DBCC CHECKIDENT ('[Instructions]', RESEED, 0);
--GO