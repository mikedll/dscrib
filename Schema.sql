
-- create DScrib2 database, first.

-- You can create a login with this.
--USE [master]
--GO
--CREATE LOGIN [dscrib2dev] WITH PASSWORD=N'backintime', DEFAULT_DATABASE=[DScrib2], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
--GO

--USE [DScrib2]
--GO
--CREATE USER [dscrib2dev] FOR LOGIN [dscrib2dev]
--GO
--USE [DScrib2]
--GO
--ALTER USER [dscrib2dev] WITH DEFAULT_SCHEMA=[dbo]
--GO

--USE [DScrib2]
--GO
--EXEC sp_addrolemember N'db_owner', N'dscrib2dev'
--GO


-- Schema.
CREATE TABLE "User" (
  "ID" INT PRIMARY KEY,
  "Email" NVARCHAR,
  "VendorID" NVARCHAR,
  );

CREATE TABLE "Reviews" (
  "ID" INT PRIMARY KEY,
  "Text" NVARCHAR,
  "UserID" INT FOREIGN KEY REFERENCES "User"(ID)
);
