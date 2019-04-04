

-- Schema.
DROP TABLE "Reviews";
DROP TABLE "Users";

CREATE TABLE "Users" (
  "ID" bigserial PRIMARY KEY,
  "Email" character varying NOT NULL,
  "VendorID" character varying UNIQUE NOT NULL
);

CREATE TABLE "Reviews" (
  "ID" bigserial PRIMARY KEY,
  "Name" character varying NOT NULL,
  "Text" CHARACTER VARYING DEFAULT '' NOT NULL,
  "Date" timestamp NOT NULL,
  "Slug" character varying NOT NULL,
  "AmazonID" character varying NOT NULL,
  "UserID" INTEGER NOT NULL REFERENCES "Users"("ID")
);

-- For localdev.
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO localdev;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public to localdev;
