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


Update AspNetRoles SET NormalizedName = UPPER(Name);
#https://gist.github.com/hieuhani/1039de6b9681714782a38c5a30fe4c35

CREATE TABLE [dbo].[AspNetRoleClaims] (
    [Id]         INT            IDENTITY (1, 1) NOT NULL,
    [RoleId]     NVARCHAR (128) NOT NULL,
    [ClaimType]  NVARCHAR (MAX) NULL,
    [ClaimValue] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[AspNetRoles] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_AspNetRoleClaims_RoleId]
    ON [dbo].[AspNetRoleClaims]([RoleId] ASC);
    
    
##PUBLISH
- GO TO Rider and next run: "prod": "rimraf ../wwwroot/dist && webpack --mode production --progress"
- Execute dotnet publish 