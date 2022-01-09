BULK INSERT InstructionsTemp
FROM 'C:\CSVData\Zeszyt1.csv'
WITH
(
    FIRSTROW = 2,
    FIELDTERMINATOR = ';',  --CSV field delimiter
    ROWTERMINATOR = '\n',   --Use to shift the control to next row
    TABLOCK
)