
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
DROP TABLE Review
DROP TABLE "User"

CREATE TABLE "User" (
  "ID" INT IDENTITY PRIMARY KEY,
  "Email" NVARCHAR(200) NOT NULL,
  "VendorID" NVARCHAR(50) NOT NULL UNIQUE,
  );

CREATE TABLE "Review" (
  "ID" INT IDENTITY PRIMARY KEY,
  "Name" NVARCHAR(1000) NOT NULL,
  "Text" TEXT NOT NULL,
  "Date" DateTime NOT NULL,
  "Slug" NVARCHAR(200) NOT NULL,
  "AmazonID" NVARCHAR(200) NOT NULL,
  "UserID" INT FOREIGN KEY REFERENCES "User"(ID) NOT NULL
);

SELECT ID, Email, VendorID FROM "User" 

SELECT ID, Name, Date FROM Review

INSERT INTO "User" (Email, VendorID) VALUES ('sam@example.com', '12345');
INSERT INTO "User" (Email, VendorID) VALUES ('sam@example.com', '123456');

