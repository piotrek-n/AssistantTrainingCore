

DECLARE @ID INT
DECLARE @Name NVARCHAR(MAX)
DECLARE @Number NVARCHAR(MAX)
DECLARE @Version NVARCHAR(MAX)
DECLARE @Group VARCHAR(MAX)
DECLARE @GroupID INT

declare kur SCROLL cursor for 
select ID,[Name],Number,[Version]   FROM [Trainings].[dbo].[Instructions]
OPEN kur;
FETCH NEXT FROM kur INTO @ID,@Name,@Number,@Version;
WHILE @@FETCH_STATUS=0
    BEGIN
    PRINT @ID;

SELECT @Group =[Grupa]
FROM [Trainings].[dbo].[InstructionsTemp] 
where 
NumerInstrukcji IS NOT NULL
AND [NazwaInstrukcji] = @Name
AND Wersja = @Version


SELECT @GroupID = [ID]
FROM [Trainings].[dbo].[Groups]
Where [GroupName] = @Group

     INSERT INTO [dbo].[InstructionGroups]
           ([TimeOfCreation]
           ,[TimeOfModification]
           ,[GroupId]
           ,[InstructionId])
     VALUES
           (getdate()
           ,getdate()
           ,@GroupID
           ,@ID)
     
	FETCH NEXT FROM kur INTO @ID,@Name,@Number,@Version;
    END
CLOSE kur   
DEALLOCATE kur