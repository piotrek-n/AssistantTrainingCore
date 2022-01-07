https://stackoverflow.com/questions/58016646/sql-exeption-invalid-column-name-normalizedusername

I had 4 missing columns where NormalizedUserName was the important one (random datatypes - TBC):

NormalizedUserName nvarchar(256), null
NormalizedEmail nvarchar(256), null
LockoutEnd datetime, null
ConcurrencyStamp nvarchar(256), null
To be able to log in, I had run the update to fill NormalizedUserName column like below:

update AspNetUsers
   set NormalizedUserName = UPPER(Email)
where NormalizedUserName is null


        migrationBuilder.AddColumn<string>(
            name: "NormalizedName",
            table: "AspNetRoles",
            type: "nvarchar(256)",
            maxLength: 256,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "ConcurrencyStamp",
            table: "AspNetRoles",
            type: "nvarchar(max)",
            nullable: true);


Script:

ALTER TABLE AspNetUsers
ADD NormalizedUserName varchar(255) NULL;

ALTER TABLE AspNetUsers
ADD ConcurrencyStamp varchar(255) NULL;

ALTER TABLE AspNetUsers
ADD NormalizedEmail varchar(255) NULL;

ALTER TABLE AspNetUsers
ADD LockoutEnd datetime NULL;

update AspNetUsers
   set NormalizedUserName = UPPER(Email)
where NormalizedUserName is null

---

ALTER TABLE AspNetRoles
ADD NormalizedName varchar(255) NULL;

ALTER TABLE AspNetRoles
ADD ConcurrencyStamp varchar(max) NULL;