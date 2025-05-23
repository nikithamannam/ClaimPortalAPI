USE [ClaimPortal2]
GO
/****** Object:  Table [dbo].[Contact_Us_Details]    Script Date: 5/16/2025 5:32:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Contact_Us_Details](
	[Contact_Us_DetailsId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NULL,
	[PolicyNumber] [varchar](20) NOT NULL,
	[FirstName] [varchar](20) NOT NULL,
	[LastName] [varchar](20) NOT NULL,
	[DateOfBirth] [date] NOT NULL,
	[ClaimNumber] [varchar](20) NOT NULL,
	[Email] [varchar](100) NULL,
	[PhoneNumber] [varchar](20) NULL,
	[Request] [varchar](2000) NULL,
	[ContactUsTypeName] [varchar](100) NULL,
	[CreatedDate] [datetime] NULL,
	[ClaimType] [nvarchar](20) NOT NULL,
	[IsAtTravel] [bit] NULL,
	[IsSeekingMedicalCare] [bit] NULL,
	[IsTripRelated] [bit] NULL,
	[FileName] [nvarchar](255) NULL,
	[ContentType] [nvarchar](100) NULL,
	[FileData] [varbinary](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Documents]    Script Date: 5/16/2025 5:32:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Documents](
	[DocumentId] [int] IDENTITY(1,1) NOT NULL,
	[FileName] [nvarchar](255) NULL,
	[ContentType] [nvarchar](100) NULL,
	[FileData] [varbinary](max) NULL,
	[UploadedOn] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[DocumentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[User_Profile]    Script Date: 5/16/2025 5:32:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User_Profile](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] [varchar](20) NOT NULL,
	[LastName] [varchar](20) NOT NULL,
	[Email] [varchar](100) NOT NULL,
	[PhoneNumber] [varchar](20) NULL,
	[DateOfBirth] [date] NULL,
	[Password] [varchar](255) NOT NULL,
	[CreatedDate] [datetime] NULL,
	[Address] [varchar](100) NULL,
	[Other_Phone] [varchar](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Contact_Us_Details] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Contact_Us_Details] ADD  DEFAULT ('') FOR [ClaimType]
GO
ALTER TABLE [dbo].[Documents] ADD  DEFAULT (getdate()) FOR [UploadedOn]
GO
ALTER TABLE [dbo].[User_Profile] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Contact_Us_Details]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[User_Profile] ([UserId])
GO
/****** Object:  StoredProcedure [dbo].[EditUserDetails]    Script Date: 5/16/2025 5:32:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 CREATE PROCEDURE [dbo].[EditUserDetails]
(
    @UserId INT,
    @FirstName VARCHAR(20),
    @LastName VARCHAR(20),
    @DateOfBirth DATE,
    @PhoneNumber VARCHAR(20) = NULL,
    @Email VARCHAR(50), -- You can increase this length if needed
    @Address VARCHAR(100),
    @Other_Phone VARCHAR(20)
)
AS
BEGIN
    UPDATE User_Profile 
    SET
        FirstName = @FirstName,
        LastName = @LastName,
        DateOfBirth = @DateOfBirth,
        PhoneNumber = @PhoneNumber,
        Other_Phone = @Other_Phone,
        Address = @Address,
        Email = @Email
    WHERE UserId = @UserId
END;
GO
/****** Object:  StoredProcedure [dbo].[GetUserByEmail]    Script Date: 5/16/2025 5:32:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetUserByEmail]

    @Email NVARCHAR(100)

AS

BEGIN

    SELECT FirstName, LastName, Email, PhoneNumber, DateOfBirth

    FROM User_Profile

    WHERE Email = @Email

END
GO
/****** Object:  StoredProcedure [dbo].[GetUserDetails]    Script Date: 5/16/2025 5:32:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetUserDetails](

@Email varchar(100)  null

)

AS

BEGIN

	SELECT 

	UserId,

	PolicyNumber,

	FirstName,

	LastName,

	DateOfBirth,

	ClaimNumber,	

	Email,

	PhoneNumber,

	Request,	

	ContactUsTypeName,

	CreatedDate

	FROM Contact_Us_Details WHERE  Email= @Email;

END;
GO
/****** Object:  StoredProcedure [dbo].[InsertDocument]    Script Date: 5/16/2025 5:32:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[InsertDocument]

    @FileName NVARCHAR(255),

    @ContentType NVARCHAR(100),

    @FileData VARBINARY(MAX)

AS

BEGIN

    INSERT INTO Documents (FileName, ContentType, FileData)

    VALUES (@FileName, @ContentType, @FileData)

END

GO
/****** Object:  StoredProcedure [dbo].[InsertIntoContactUsDetails]    Script Date: 5/16/2025 5:32:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[InsertIntoContactUsDetails] 

(

    @PolicyNumber VARCHAR(20),

    @FirstName VARCHAR(20),

    @LastName VARCHAR(20),

    @Email VARCHAR(100),

    @PhoneNumber VARCHAR(20),

    @DateOfBirth DATE,

    @ClaimNumber VARCHAR(20),

    @ContactUsTypeName VARCHAR(100),

    @Request VARCHAR(2000),

    @CreatedDate DATETIME = NULL,
 
    -- From Claims table

    @ClaimType NVARCHAR(20),

    @IsAtTravel BIT = NULL,

    @IsSeekingMedicalCare BIT = NULL,

    @IsTripRelated BIT = NULL,
 
    -- From Documents table

    @FileName NVARCHAR(255) = NULL,

    @ContentType NVARCHAR(100) = NULL,

    @FileData VARBINARY(MAX) = NULL

)

AS

BEGIN

    SET NOCOUNT ON;
 
    DECLARE @UserId INT;
 
    SELECT @UserId = UserId

    FROM User_Profile

    WHERE Email = @Email;
 
    -- Insert into Contact_Us_Details

    INSERT INTO Contact_Us_Details (

        UserId,

        PolicyNumber,

        FirstName,

        LastName,

        DateOfBirth,

        ClaimNumber,

        Email,

        PhoneNumber,

        Request,

        ContactUsTypeName,

        CreatedDate,

        ClaimType,

        IsAtTravel,

        IsSeekingMedicalCare,

        IsTripRelated

    )

    VALUES (

        @UserId,

        @PolicyNumber,

        @FirstName,

        @LastName,

        @DateOfBirth,

        @ClaimNumber,

        @Email,

        @PhoneNumber,

        @Request,

        @ContactUsTypeName,

        ISNULL(@CreatedDate, GETDATE()),

        @ClaimType,

        @IsAtTravel,

        @IsSeekingMedicalCare,

        @IsTripRelated

    );
 
    -- Insert into Documents if file data exists

    IF @FileName IS NOT NULL AND @FileData IS NOT NULL

    BEGIN

        INSERT INTO Contact_Us_Details (FileName, ContentType, FileData)

        VALUES (@FileName, @ContentType, @FileData);

    END

END
GO
/****** Object:  StoredProcedure [dbo].[LoginUser]    Script Date: 5/16/2025 5:32:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[LoginUser]
    @Email NVARCHAR(100),
    @Password NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1 FROM User_Profile WHERE Email = @Email AND Password = @Password
    )
    BEGIN
        SELECT 
            UserId,
            FirstName,
            LastName,
            Email,
            PhoneNumber,
            DateOfBirth,
            Address
        FROM User_Profile
        WHERE Email = @Email AND Password = @Password;

        RETURN 1;
    END
    ELSE
    BEGIN
        RETURN 0;
    END
END
GO
/****** Object:  StoredProcedure [dbo].[RegisterUser]    Script Date: 5/16/2025 5:32:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[RegisterUser] (

    @Email VARCHAR(255) = NULL,

    @PhoneNumber VARCHAR(20) = NULL,

    @FirstName VARCHAR(20) = NULL,

    @LastName VARCHAR(20) = NULL,

    @Password VARCHAR(255) = NULL,

    @DateOfBirth DATE = NULL

	)

AS

BEGIN

    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM User_Profile WHERE Email = @Email)

    BEGIN

        SELECT 'Email already exists.' AS Message;

        RETURN;        

    END

    INSERT INTO User_Profile 

        (Email, PhoneNumber, FirstName, LastName, Password, DateOfBirth) 

    VALUES 

        (@Email, @PhoneNumber, @FirstName, @LastName, @Password, @DateOfBirth);

    SELECT 'User registered successfully.' AS Message;

END;
GO
