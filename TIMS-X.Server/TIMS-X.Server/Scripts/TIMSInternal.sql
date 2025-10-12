
-- Create TIMSInternal database
IF NOT EXISTS (SELECT [name]  FROM master.dbo.sysdatabases WHERE [name] = 'TIMSInternal')
BEGIN
CREATE DATABASE [TIMSInternal]
END 
use TIMSInternal
GO


-- Create TimsServer table to keep track of the servers.
CREATE TABLE dbo.TimsServer
(
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name]     varchar(64) NOT NULL,
	[Address] varchar(64) NOT NULL,
	[Inactive] bit NOT NULL DEFAULT(0),
	[DateCreated] datetime NOT NULL DEFAULT GETDATE(),
	CONSTRAINT [PK_TimsServer] PRIMARY KEY CLUSTERED (ID),
    CONSTRAINT [UQ_TimsServer_Name] UNIQUE (Name),
    CONSTRAINT [UQ_TimsServer_Address] UNIQUE (Address),
)
go

-- Create SupportUser table to keep track of the support users.
CREATE TABLE dbo.SupportUser
(
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name]     varchar(64) NOT NULL,
	[Password] varchar(256) NULL,
	[Email] varchar(128) NOT NULL,
	[Inactive] bit NOT NULL DEFAULT(0),
	[DateCreated] datetime NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_SupportUser] PRIMARY KEY CLUSTERED (ID),
    CONSTRAINT [UQ_SupportUser_Email] UNIQUE (Email)
)
go

-- Create Customer table to keep track of our customers.
CREATE TABLE dbo.Customer
(
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Inactive] bit NOT NULL DEFAULT(0),
	[Name] 				nvarchar(128),
	[OfficeCode]        varchar(16),
	[ServerId]          int,
	[Database]          varchar(128),
	[SqlUser]  nvarchar(100) not null default(''),
	[SqlPassword]  nvarchar(100) not null default(''),
	[TimeZoneId]          int,
	[Notes]             nvarchar(256),
	[DateCreated]		datetime NOT NULL DEFAULT GETDATE(),
	[DateUpdated]		datetime NOT NULL,
	[UpdatedBy]         int,
    CONSTRAINT [PK_Customer] PRIMARY KEY CLUSTERED (ID),
    CONSTRAINT [UQ_Customer_OfficeCode] UNIQUE (OfficeCode),
    CONSTRAINT [FK_Customer_TimsServer] FOREIGN KEY (ServerId) REFERENCES TimsServer(ID),
    CONSTRAINT [FK_Customer_User] FOREIGN KEY (UpdatedBy) REFERENCES SupportUser(ID)
)
go

-- Create Vendor table to keep track of third party vendors.
CREATE TABLE dbo.Vendor
(
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name]     varchar(64) NOT NULL,
	[ApiKey] varchar(256) NOT NULL,
	[Inactive] bit NOT NULL DEFAULT(0),
	[DateCreated] datetime NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_Vendor] PRIMARY KEY CLUSTERED (ID),
    CONSTRAINT [UQ_Vendor_Name] UNIQUE (Name),
    CONSTRAINT [UQ_Vendor_ApiKey] UNIQUE (ApiKey)
)
go

-- Create ApiUrl table to keep track of API endpoints available to third party vendors.
CREATE TABLE dbo.ApiUrl
(
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Url] varchar(128) NOT NULL,
	[Description]     varchar(256) NULL,
	[Inactive] bit NOT NULL DEFAULT(0),
	[DateCreated] datetime NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_ApiUrl] PRIMARY KEY CLUSTERED (ID),
    CONSTRAINT [UQ_ApiUrl_Url] UNIQUE (Url)
)
go

-- Create VendorPermission table to group API endpoints into sensible permission entities.
CREATE TABLE dbo.VendorPermission
(
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name]     varchar(64) NOT NULL,
	[Description]     varchar(256) NULL,
	[Inactive] bit NOT NULL DEFAULT(0),
	[DateCreated] datetime NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_VendorPermission] PRIMARY KEY CLUSTERED (ID),
    CONSTRAINT [UQ_VendorPermission_Name] UNIQUE (Name)
)
go

-- Create VendorPermissionApiUrl junction table to allow permissions to have multiple API endpoints.
CREATE TABLE dbo.VendorPermissionApiUrl
(
	[PermissionId]		int,
	[ApiUrlId]			int,
	CONSTRAINT [PK_VendorPermissionApiUrlReference] PRIMARY KEY CLUSTERED (PermissionId, ApiUrlId),
	CONSTRAINT [FK_VPAU_Permission] FOREIGN KEY (PermissionId) REFERENCES VendorPermission(ID),
	CONSTRAINT [FK_VPAU_ApiUrl] FOREIGN KEY (ApiUrlId) REFERENCES ApiUrl(ID)
)
go

-- Create DefaultVendorPermission junction table to allow vendors to have multiple default permissions.
CREATE TABLE dbo.DefaultVendorPermission
(
	[VendorId]		int,
	[PermissionId]  int,
	CONSTRAINT [PK_DefaultVendorPermission] PRIMARY KEY CLUSTERED (VendorId, PermissionId),
	CONSTRAINT [FK_DVP_Vendor] FOREIGN KEY (VendorId) REFERENCES Vendor(ID),
	CONSTRAINT [FK_DVP_Permission] FOREIGN KEY (PermissionId) REFERENCES VendorPermission(ID)
)
go

-- Create CustomerVendorPermission junction table to which grants the vendor a specific permission against a specific customer database.
CREATE TABLE dbo.CustomerVendorPermission
(
	[CustomerId]	 int,
	[VendorId]		 int,
	[PermissionId]   int,
	CONSTRAINT [PK_CustomerVendorPermission] PRIMARY KEY CLUSTERED (CustomerId, VendorId, PermissionId),
	CONSTRAINT [FK_CVP_Customer] FOREIGN KEY (CustomerId) REFERENCES Customer(ID),
	CONSTRAINT [FK_CVP_Vendor] FOREIGN KEY (VendorId) REFERENCES Vendor(ID),
	CONSTRAINT [FK_CVP_Permission] FOREIGN KEY (PermissionId) REFERENCES VendorPermission(ID)
)
go


-- Create FormLink table to keep track of patient form links
CREATE TABLE dbo.FormLink
(
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CustomerId] int,
	[Url]     varchar(6) NOT NULL,
	[UserId] int NOT NULL,
	[FormType] int NOT NULL,
	[PatientId] int NOT NULL,
	[Submitted] bit NOT NULL,
	[DateCreated] datetime NOT NULL DEFAULT GETDATE(),
	CONSTRAINT [PK_FormLink] PRIMARY KEY CLUSTERED (ID),
    CONSTRAINT [UQ_FormLink_Url] UNIQUE (Url),
    CONSTRAINT [FK_FormLink_Customer] FOREIGN KEY (CustomerId) REFERENCES Customer(ID)
)
go



-- Create support users. Default password is cu$support1
INSERT INTO SupportUser(Name, Email, Password) VALUES('', '', '')
