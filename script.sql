USE [MetsubouJinrai]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 4/28/2021 4:28:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Category]    Script Date: 4/28/2021 4:28:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Category](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](max) NULL,
 CONSTRAINT [PK_Category] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[OrderDetails]    Script Date: 4/28/2021 4:28:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderDetails](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrderId] [int] NOT NULL,
	[ProductId] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[Price] [decimal](18, 2) NOT NULL,
 CONSTRAINT [PK_OrderDetails] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Orders]    Script Date: 4/28/2021 4:28:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Orders](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrderTotal] [decimal](18, 2) NOT NULL,
	[OrderPlaced] [datetime2](7) NOT NULL,
	[UserId] [bigint] NOT NULL,
 CONSTRAINT [PK_Orders] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Product]    Script Date: 4/28/2021 4:28:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Product](
	[ProductId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[Price] [decimal](18, 2) NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL,
	[DateUpdated] [datetime2](7) NOT NULL,
	[ProductPicture] [nvarchar](max) NULL,
 CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED 
(
	[ProductId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProductCategories]    Script Date: 4/28/2021 4:28:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductCategories](
	[ProductID] [int] NOT NULL,
	[CategoryID] [int] NOT NULL,
 CONSTRAINT [PK_ProductCategories] PRIMARY KEY CLUSTERED 
(
	[ProductID] ASC,
	[CategoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RoleClaims]    Script Date: 4/28/2021 4:28:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RoleClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RoleId] [bigint] NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_RoleClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Roles]    Script Date: 4/28/2021 4:28:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Roles](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[Name] [nvarchar](256) NULL,
	[NormalizedName] [nvarchar](256) NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
 CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserClaims]    Script Date: 4/28/2021 4:28:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [bigint] NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_UserClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserLogins]    Script Date: 4/28/2021 4:28:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserLogins](
	[LoginProvider] [nvarchar](450) NOT NULL,
	[ProviderKey] [nvarchar](450) NOT NULL,
	[ProviderDisplayName] [nvarchar](max) NULL,
	[UserId] [bigint] NOT NULL,
 CONSTRAINT [PK_UserLogins] PRIMARY KEY CLUSTERED 
(
	[LoginProvider] ASC,
	[ProviderKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserRoles]    Script Date: 4/28/2021 4:28:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserRoles](
	[UserId] [bigint] NOT NULL,
	[RoleId] [bigint] NOT NULL,
 CONSTRAINT [PK_UserRoles] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Users]    Script Date: 4/28/2021 4:28:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[FullName] [nvarchar](max) NULL,
	[Address] [nvarchar](max) NULL,
	[Birthday] [datetime2](7) NULL,
	[ProfilePicture] [nvarchar](max) NULL,
	[UserName] [nvarchar](256) NULL,
	[NormalizedUserName] [nvarchar](256) NULL,
	[Email] [nvarchar](256) NULL,
	[NormalizedEmail] [nvarchar](256) NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[PasswordHash] [nvarchar](max) NULL,
	[SecurityStamp] [nvarchar](max) NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](max) NULL,
	[PhoneNumberConfirmed] [bit] NOT NULL,
	[TwoFactorEnabled] [bit] NOT NULL,
	[LockoutEnd] [datetimeoffset](7) NULL,
	[LockoutEnabled] [bit] NOT NULL,
	[AccessFailedCount] [int] NOT NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserTokens]    Script Date: 4/28/2021 4:28:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserTokens](
	[UserId] [bigint] NOT NULL,
	[LoginProvider] [nvarchar](450) NOT NULL,
	[Name] [nvarchar](450) NOT NULL,
	[Value] [nvarchar](max) NULL,
 CONSTRAINT [PK_UserTokens] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[LoginProvider] ASC,
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20210419044927_Init', N'5.0.5')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20210419080853_Init', N'5.0.4')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20210420031143_AddProduct', N'5.0.4')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20210420034718_Update', N'5.0.4')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20210427021436_AddOrder', N'5.0.5')
SET IDENTITY_INSERT [dbo].[Category] ON 

INSERT [dbo].[Category] ([Id], [Name], [Description]) VALUES (1, N'Weapon', N'Vũ khí')
INSERT [dbo].[Category] ([Id], [Name], [Description]) VALUES (2, N'Progrise Key', N'Progrise Key')
INSERT [dbo].[Category] ([Id], [Name], [Description]) VALUES (3, N'Zetsumerise Key', N'Zetsumerise Key')
INSERT [dbo].[Category] ([Id], [Name], [Description]) VALUES (4, N'Driver', N'Transformation Gear')
SET IDENTITY_INSERT [dbo].[Category] OFF
SET IDENTITY_INSERT [dbo].[OrderDetails] ON 

INSERT [dbo].[OrderDetails] ([Id], [OrderId], [ProductId], [Quantity], [Price]) VALUES (2, 1, 10, 1, CAST(500000.00 AS Decimal(18, 2)))
INSERT [dbo].[OrderDetails] ([Id], [OrderId], [ProductId], [Quantity], [Price]) VALUES (3, 2, 8, 3, CAST(500000.00 AS Decimal(18, 2)))
INSERT [dbo].[OrderDetails] ([Id], [OrderId], [ProductId], [Quantity], [Price]) VALUES (4, 2, 10, 2, CAST(500000.00 AS Decimal(18, 2)))
INSERT [dbo].[OrderDetails] ([Id], [OrderId], [ProductId], [Quantity], [Price]) VALUES (5, 2, 9, 1, CAST(500000.00 AS Decimal(18, 2)))
INSERT [dbo].[OrderDetails] ([Id], [OrderId], [ProductId], [Quantity], [Price]) VALUES (6, 2, 7, 1, CAST(500000.00 AS Decimal(18, 2)))
INSERT [dbo].[OrderDetails] ([Id], [OrderId], [ProductId], [Quantity], [Price]) VALUES (7, 2, 6, 3, CAST(500000.00 AS Decimal(18, 2)))
INSERT [dbo].[OrderDetails] ([Id], [OrderId], [ProductId], [Quantity], [Price]) VALUES (8, 3, 70, 1, CAST(250000.00 AS Decimal(18, 2)))
INSERT [dbo].[OrderDetails] ([Id], [OrderId], [ProductId], [Quantity], [Price]) VALUES (9, 3, 76, 1, CAST(400000.00 AS Decimal(18, 2)))
SET IDENTITY_INSERT [dbo].[OrderDetails] OFF
SET IDENTITY_INSERT [dbo].[Orders] ON 

INSERT [dbo].[Orders] ([Id], [OrderTotal], [OrderPlaced], [UserId]) VALUES (1, CAST(500000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 10:53:51.5555317' AS DateTime2), 7)
INSERT [dbo].[Orders] ([Id], [OrderTotal], [OrderPlaced], [UserId]) VALUES (2, CAST(5000000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 10:54:31.8691712' AS DateTime2), 7)
INSERT [dbo].[Orders] ([Id], [OrderTotal], [OrderPlaced], [UserId]) VALUES (3, CAST(650000.00 AS Decimal(18, 2)), CAST(N'2021-04-28 15:49:00.2504209' AS DateTime2), 5)
SET IDENTITY_INSERT [dbo].[Orders] OFF
SET IDENTITY_INSERT [dbo].[Product] ON 

INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (6, N'Rising Hopper ', NULL, CAST(250000.00 AS Decimal(18, 2)), CAST(N'2021-04-20 10:18:04.2715482' AS DateTime2), CAST(N'2021-04-20 10:48:59.9615779' AS DateTime2), N'KR01-Rising_Hopper_Progrisekey.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (7, N'Shooting Wolf ', NULL, CAST(250000.00 AS Decimal(18, 2)), CAST(N'2021-04-20 10:43:56.9822054' AS DateTime2), CAST(N'2021-04-20 10:48:46.5935725' AS DateTime2), N'KR01-Shooting_Wolf_Progrisekey.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (8, N'Rushing Cheetah ', NULL, CAST(250000.00 AS Decimal(18, 2)), CAST(N'2021-04-20 10:50:12.4031635' AS DateTime2), CAST(N'2021-04-20 10:52:03.0346981' AS DateTime2), N'KR01-Rushing_Cheetah_Progrisekey.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (9, N'Flying Falcon ', NULL, CAST(250000.00 AS Decimal(18, 2)), CAST(N'2021-04-20 10:52:42.2836392' AS DateTime2), CAST(N'2021-04-20 10:52:42.2836408' AS DateTime2), N'KR01-Flying_Falcon_Progrisekey.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (10, N'Sting Scorpion ', NULL, CAST(250000.00 AS Decimal(18, 2)), CAST(N'2021-04-20 10:53:22.4312809' AS DateTime2), CAST(N'2021-04-27 13:14:05.6909636' AS DateTime2), N'KR01-Sting_Scorpion_Progrisekey.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (11, N'Rising Hopper (Realize Ver.)', NULL, CAST(300000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 13:13:18.3330058' AS DateTime2), CAST(N'2021-04-27 13:13:18.3336877' AS DateTime2), N'KR01-Rising_Hopper_Progrisekey_Realize_Ver.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (12, N'Amazing Caucasus', NULL, CAST(250000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 13:14:58.1928761' AS DateTime2), CAST(N'2021-04-27 13:14:58.1928816' AS DateTime2), N'KR01-Amazing_Caucasus_Progrisekey.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (13, N'Biting Shark', NULL, CAST(250000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 13:15:47.2260995' AS DateTime2), CAST(N'2021-04-27 13:18:08.9225750' AS DateTime2), N'KR01-Biting_Shark_Progrisekey.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (14, N'Flaming Tiger', NULL, CAST(250000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 13:18:53.2800611' AS DateTime2), CAST(N'2021-04-27 13:18:53.2800660' AS DateTime2), N'KR01-Flaming_Tiger_Progrisekey.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (15, N'Freezing Bear', NULL, CAST(250000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 13:19:39.1880848' AS DateTime2), CAST(N'2021-04-27 13:19:39.1880900' AS DateTime2), N'KR01-Freezing_Bear_Progrisekey.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (16, N'Breaking Mammoth', NULL, CAST(250000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 13:20:17.9744776' AS DateTime2), CAST(N'2021-04-27 13:20:17.9744805' AS DateTime2), N'KR01-Breaking_Mammoth_Progrisekey.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (17, N'Hopping Kangaroo', NULL, CAST(250000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 13:20:49.1414507' AS DateTime2), CAST(N'2021-04-27 13:20:49.1414541' AS DateTime2), N'KR01-Hopping_Kangaroo_Progrisekey.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (18, N'Punching Kong', NULL, CAST(250000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 13:21:29.6323637' AS DateTime2), CAST(N'2021-04-27 13:21:29.6323670' AS DateTime2), N'KR01-Punching_Kong_Progrisekey.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (19, N'Gatling Hedgehog', NULL, CAST(250000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 13:22:02.9820759' AS DateTime2), CAST(N'2021-04-27 13:22:02.9820804' AS DateTime2), N'KR01-Gatling_Hedgehog_Progrisekey.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (20, N'Trapping Spider ', NULL, CAST(250000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 13:22:39.1210117' AS DateTime2), CAST(N'2021-04-27 13:22:39.1210186' AS DateTime2), N'KR01-Trapping_Spider_Progrisekey.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (21, N'Lightning Hornet', NULL, CAST(250000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 13:23:07.4238432' AS DateTime2), CAST(N'2021-04-27 13:23:07.4238479' AS DateTime2), N'KR01-Lightning_Hornet_Progrisekey.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (22, N'Amazing Hercules', NULL, CAST(250000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 13:23:35.4753310' AS DateTime2), CAST(N'2021-04-27 13:23:35.4753401' AS DateTime2), N'KR01-Amazing_Hercules_Progrisekey.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (23, N'Crushing Buffalo', NULL, CAST(250000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 13:24:10.5440388' AS DateTime2), CAST(N'2021-04-27 13:24:10.5440439' AS DateTime2), N'KR01-Crushing_Buffalo_Progrisekey.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (24, N'Splashing Whale', NULL, CAST(250000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 13:24:51.3333937' AS DateTime2), CAST(N'2021-04-27 13:24:51.3334066' AS DateTime2), N'KR01-Splashing_Whale_Progrisekey.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (25, N'Dynamaiting Lion', NULL, CAST(250000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 13:25:14.6278585' AS DateTime2), CAST(N'2021-04-27 13:25:14.6278633' AS DateTime2), N'KR01-Dynamaiting_Lion_Progrisekey.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (26, N'Storming Penguin', NULL, CAST(250000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 13:25:44.3809306' AS DateTime2), CAST(N'2021-04-27 13:25:44.3809455' AS DateTime2), N'KR01-Storming_Penguin_Progrisekey.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (27, N'Scouting Panda', NULL, CAST(250000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 13:26:16.8634025' AS DateTime2), CAST(N'2021-04-27 13:26:16.8634036' AS DateTime2), N'KR01-Scouting_Panda_Progrisekey.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (28, N'Fighting Jackal', NULL, CAST(250000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 13:26:52.0919597' AS DateTime2), CAST(N'2021-04-27 13:26:52.0919675' AS DateTime2), N'KR01-Fighting_Jackal_Progrisekey.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (29, N'Sparking Giraffe', NULL, CAST(250000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 13:27:32.3930498' AS DateTime2), CAST(N'2021-04-27 13:27:32.3930548' AS DateTime2), N'KR01-Sparking_Giraffe_Progrisekey.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (30, N'Exciting Stag', NULL, CAST(250000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 13:28:13.3653591' AS DateTime2), CAST(N'2021-04-27 13:28:13.3653609' AS DateTime2), N'KR01-Exciting_Stag_Progrisekey.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (31, N'Shining Hopper', NULL, CAST(300000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 13:28:51.2160037' AS DateTime2), CAST(N'2021-04-27 13:28:51.2160089' AS DateTime2), N'KR01-Shining_Hopper_Progrisekey.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (32, N'Shining Assault Hopper', NULL, CAST(350000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 13:29:19.0704167' AS DateTime2), CAST(N'2021-04-27 13:29:19.0704180' AS DateTime2), N'KR01-Shining_Assault_Hopper_Progrisekey.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (33, N'Assault Wolf', NULL, CAST(350000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 13:29:51.5754432' AS DateTime2), CAST(N'2021-04-27 13:29:51.5754447' AS DateTime2), N'KR01-Assault_Wolf_Progrisekey.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (34, N'MetalCluster Hopper', NULL, CAST(400000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 13:30:35.4049486' AS DateTime2), CAST(N'2021-04-27 13:30:35.4049539' AS DateTime2), N'KR01-MetalCluster_Hopper_Progrisekey.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (35, N'Burning Falcon', NULL, CAST(300000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 13:31:15.4452463' AS DateTime2), CAST(N'2021-04-27 13:31:15.4452482' AS DateTime2), N'KR01-Burning_Falcon_Progrisekey.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (36, N'Rampage Gatling', NULL, CAST(400000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 13:31:50.2536520' AS DateTime2), CAST(N'2021-04-27 13:31:50.2536536' AS DateTime2), N'KR01-Rampage_Gatling_Progrisekey.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (37, N'Zero-Two', NULL, CAST(400000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 13:32:31.8454848' AS DateTime2), CAST(N'2021-04-27 13:32:31.8454860' AS DateTime2), N'KR01-Zero-Two_Progrisekey.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (38, N'Hellrise', NULL, CAST(400000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 13:33:11.8504642' AS DateTime2), CAST(N'2021-04-27 13:33:11.8504669' AS DateTime2), N'KR01-Hellrise_Progrisekey.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (39, N'Ark-One', NULL, CAST(400000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 13:33:48.2476460' AS DateTime2), CAST(N'2021-04-27 13:33:48.2476528' AS DateTime2), N'KR01-Ark-One_Progrisekey.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (40, N'Ark Scorpion', NULL, CAST(400000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 13:34:22.0373513' AS DateTime2), CAST(N'2021-04-27 13:34:22.0373521' AS DateTime2), N'KR01-Ark_Scorpion_Progrisekey.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (41, N'Ark-Zero-One', NULL, CAST(400000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 13:35:00.5843377' AS DateTime2), CAST(N'2021-04-27 13:35:00.5843395' AS DateTime2), N'KR01-Ark-Zero-One_Progrisekey.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (42, N'Berotha', NULL, CAST(200000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 13:36:19.0586668' AS DateTime2), CAST(N'2021-04-27 13:36:19.0586682' AS DateTime2), N'KR01-Berotha_Zetsumerisekey.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (43, N'Kuehne', NULL, CAST(200000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 13:36:56.0748470' AS DateTime2), CAST(N'2021-04-27 13:36:56.0748491' AS DateTime2), N'KR01-Kuehne_Zetsumerisekey.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (44, N'Ekal', NULL, CAST(200000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 13:37:38.2928927' AS DateTime2), CAST(N'2021-04-27 13:37:38.2928940' AS DateTime2), N'KR01-Ekal_Zetsumerisekey.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (45, N'Neohi', NULL, CAST(20000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 13:38:20.0899971' AS DateTime2), CAST(N'2021-04-27 13:38:20.0899985' AS DateTime2), N'KR01-Neohi_Zetsumerisekey.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (46, N'Onycho', NULL, CAST(200000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 13:39:01.3271335' AS DateTime2), CAST(N'2021-04-27 13:39:01.3271365' AS DateTime2), N'KR01-Onycho_Zetsumerisekey.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (47, N'Vicarya', NULL, CAST(200000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 13:39:30.5675542' AS DateTime2), CAST(N'2021-04-27 13:39:30.5675578' AS DateTime2), N'KR01-Vicarya_Zetsumerisekey.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (48, N'Gaeru', NULL, CAST(200000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 13:40:02.5566367' AS DateTime2), CAST(N'2021-04-27 13:40:02.5566379' AS DateTime2), N'KR01-Gaeru_Zetsumerisekey.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (49, N'Mammoth', NULL, CAST(200000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 13:40:45.4852918' AS DateTime2), CAST(N'2021-04-27 13:40:45.4853030' AS DateTime2), N'KR01-Mammoth_Zetsumerisekey.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (50, N'Rocking Hopper', NULL, CAST(250000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 13:41:22.7436772' AS DateTime2), CAST(N'2021-04-27 13:41:22.7436786' AS DateTime2), N'KR01-Rocking_Hopper_Zetsumerisekey.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (51, N'Dodo', NULL, CAST(250000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 13:41:48.2027007' AS DateTime2), CAST(N'2021-04-27 13:41:48.2027032' AS DateTime2), N'KR01-Dodo_Zetsumerisekey.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (52, N'Awaking Arsino', NULL, CAST(250000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 13:42:39.8604732' AS DateTime2), CAST(N'2021-04-27 13:42:39.8604762' AS DateTime2), N'KR01-Awaking_Arsino_Zetsumerisekey.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (53, N'Japanese Wolf', NULL, CAST(250000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 13:43:11.0586235' AS DateTime2), CAST(N'2021-04-27 13:43:11.0586426' AS DateTime2), N'KR01-Japanese_Wolf_Zetsumerisekey.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (54, N'Eden', NULL, CAST(400000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 13:43:55.1082902' AS DateTime2), CAST(N'2021-04-27 13:43:55.1082916' AS DateTime2), N'KR01-Eden_Zetsumerisekey.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (55, N'Massbrain', NULL, CAST(400000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 13:44:41.5234728' AS DateTime2), CAST(N'2021-04-27 13:44:41.5234758' AS DateTime2), N'KR01-Massbrain_Zetsumerisekey.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (56, N'Dire Wolf', NULL, CAST(350000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 13:45:20.5324173' AS DateTime2), CAST(N'2021-04-27 13:45:20.5324199' AS DateTime2), N'KR01-Dire_Wolf_Zetsumerisekey.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (57, N'Serval Tiger', NULL, CAST(300000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 13:46:15.2659359' AS DateTime2), CAST(N'2021-04-27 13:46:15.2659373' AS DateTime2), N'KR01-Serval_Tiger_Zetsumerisekey.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (58, N'Hiden Zero-One', NULL, CAST(300000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 13:50:05.5284729' AS DateTime2), CAST(N'2021-04-27 13:50:05.5284759' AS DateTime2), N'KR01-Hiden_Zero-One_Driver.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (59, N'Zetsumetsu', NULL, CAST(300000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 13:51:03.9174125' AS DateTime2), CAST(N'2021-04-27 13:51:03.9174186' AS DateTime2), N'KR01-Zetsumetsu_Driver.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (60, N'Eden', NULL, CAST(300000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 13:51:35.3118299' AS DateTime2), CAST(N'2021-04-27 13:51:35.3118307' AS DateTime2), N'KR01-Eden_Driver.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (61, N'MetsubouJinrai', NULL, CAST(300000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 13:52:10.4213894' AS DateTime2), CAST(N'2021-04-27 13:52:10.4213917' AS DateTime2), N'KR01-MetsubouJinrai_Driver.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (62, N'ShotRiser', NULL, CAST(300000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 13:54:18.4015096' AS DateTime2), CAST(N'2021-04-27 13:54:18.4015114' AS DateTime2), N'KR01-A.I.M.S._ShotRise.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (63, N'MetsuboJinrai ForceRiser', NULL, CAST(300000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 13:55:09.4057752' AS DateTime2), CAST(N'2021-04-27 13:55:09.4057763' AS DateTime2), N'KR01-MetsuboJinrai_ForceRiser.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (64, N'CycloneRiser', NULL, CAST(300000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 13:55:38.8105433' AS DateTime2), CAST(N'2021-04-27 13:55:38.8105445' AS DateTime2), N'KR01-CycloneRiser.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (65, N'ZAIA Thousandriver', NULL, CAST(300000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 13:56:36.9009806' AS DateTime2), CAST(N'2021-04-27 13:56:36.9009832' AS DateTime2), N'KR01-ZAIA_Thousandriver_(Thouser).png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (66, N'ZAIA Slashriser', NULL, CAST(30000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 13:57:58.6750711' AS DateTime2), CAST(N'2021-04-27 14:14:29.8865108' AS DateTime2), N'KR01-ZAIA_Slashriser.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (67, N'Ark', NULL, CAST(30000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 13:58:55.9808574' AS DateTime2), CAST(N'2021-04-27 13:58:55.9808596' AS DateTime2), N'KR01-Ark_Driver-One.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (68, N'Hiden Zero-Two', NULL, CAST(350000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 13:59:53.8644507' AS DateTime2), CAST(N'2021-04-27 13:59:53.8644565' AS DateTime2), N'KR01-Hiden_Zero-Two_Driver.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (69, N'Zetsumeriser', NULL, CAST(250000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 14:00:50.4124022' AS DateTime2), CAST(N'2021-04-27 14:00:50.4124038' AS DateTime2), N'KR01-ZetsumeRiser.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (70, N'RaidRiser', NULL, CAST(250000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 14:01:25.8245724' AS DateTime2), CAST(N'2021-04-27 14:01:25.8245738' AS DateTime2), N'KR01-RaidRiser.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (71, N'Thousand Jacker', NULL, CAST(500000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 14:05:03.9312908' AS DateTime2), CAST(N'2021-04-27 14:05:03.9312922' AS DateTime2), N'KR01-Thousand_Jacker.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (72, N'Progrise Hopper Blade', NULL, CAST(500000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 14:06:34.7360221' AS DateTime2), CAST(N'2021-04-27 14:06:34.7360252' AS DateTime2), N'KR01-Progrise_Hopper_Blade_Blade_Mode.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (73, N'Attache Calibur', NULL, CAST(350000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 14:07:14.8144241' AS DateTime2), CAST(N'2021-04-27 14:07:14.8144319' AS DateTime2), N'KR01-Attache_Calibur.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (74, N'Attache Arrow', NULL, CAST(35000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 14:08:51.8348893' AS DateTime2), CAST(N'2021-04-27 14:08:51.8348904' AS DateTime2), N'KR01-Attache_Arrow.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (75, N'Attache Shotgun', NULL, CAST(350000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 14:10:03.5582255' AS DateTime2), CAST(N'2021-04-27 14:10:03.5582264' AS DateTime2), N'KR01-Attache_Shotgun.png')
INSERT [dbo].[Product] ([ProductId], [Name], [Description], [Price], [DateCreated], [DateUpdated], [ProductPicture]) VALUES (76, N'Authorise Buster', NULL, CAST(400000.00 AS Decimal(18, 2)), CAST(N'2021-04-27 14:10:58.4219843' AS DateTime2), CAST(N'2021-04-27 14:10:58.4219868' AS DateTime2), N'KR01-Authorise_Buster.png')
SET IDENTITY_INSERT [dbo].[Product] OFF
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (71, 1)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (72, 1)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (73, 1)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (74, 1)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (75, 1)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (76, 1)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (6, 2)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (7, 2)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (8, 2)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (9, 2)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (10, 2)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (11, 2)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (12, 2)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (13, 2)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (14, 2)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (15, 2)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (16, 2)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (17, 2)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (18, 2)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (19, 2)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (20, 2)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (21, 2)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (22, 2)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (23, 2)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (24, 2)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (25, 2)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (26, 2)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (27, 2)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (28, 2)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (29, 2)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (30, 2)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (31, 2)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (32, 2)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (33, 2)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (34, 2)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (35, 2)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (36, 2)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (37, 2)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (38, 2)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (39, 2)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (40, 2)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (41, 2)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (42, 3)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (43, 3)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (44, 3)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (45, 3)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (46, 3)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (47, 3)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (48, 3)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (49, 3)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (50, 3)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (51, 3)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (52, 3)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (53, 3)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (54, 3)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (55, 3)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (56, 3)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (57, 3)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (58, 4)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (59, 4)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (60, 4)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (61, 4)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (62, 4)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (63, 4)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (64, 4)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (65, 4)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (66, 4)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (67, 4)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (68, 4)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (69, 4)
INSERT [dbo].[ProductCategories] ([ProductID], [CategoryID]) VALUES (70, 4)
SET IDENTITY_INSERT [dbo].[RoleClaims] ON 

INSERT [dbo].[RoleClaims] ([Id], [RoleId], [ClaimType], [ClaimValue]) VALUES (1, 4, N'permission', N'user.create')
INSERT [dbo].[RoleClaims] ([Id], [RoleId], [ClaimType], [ClaimValue]) VALUES (2, 4, N'permission', N'user.delete')
INSERT [dbo].[RoleClaims] ([Id], [RoleId], [ClaimType], [ClaimValue]) VALUES (3, 4, N'permission', N'product.view')
INSERT [dbo].[RoleClaims] ([Id], [RoleId], [ClaimType], [ClaimValue]) VALUES (4, 4, N'permission', N'product.create')
INSERT [dbo].[RoleClaims] ([Id], [RoleId], [ClaimType], [ClaimValue]) VALUES (5, 4, N'permission', N'product.update')
INSERT [dbo].[RoleClaims] ([Id], [RoleId], [ClaimType], [ClaimValue]) VALUES (6, 4, N'permission', N'product.delete')
SET IDENTITY_INSERT [dbo].[RoleClaims] OFF
SET IDENTITY_INSERT [dbo].[Roles] ON 

INSERT [dbo].[Roles] ([Id], [Description], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (4, N'Boss', N'Admin', N'ADMIN', N'75486993-8619-401d-bc04-5d89de258d17')
INSERT [dbo].[Roles] ([Id], [Description], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (5, N'Người dùng', N'User', N'USER', N'7a03f3ca-fbbc-4863-8f12-29c600926f4d')
SET IDENTITY_INSERT [dbo].[Roles] OFF
INSERT [dbo].[UserLogins] ([LoginProvider], [ProviderKey], [ProviderDisplayName], [UserId]) VALUES (N'Google', N'104234607192197838825', N'Google', 16)
INSERT [dbo].[UserRoles] ([UserId], [RoleId]) VALUES (5, 4)
SET IDENTITY_INSERT [dbo].[Users] ON 

INSERT [dbo].[Users] ([id], [FullName], [Address], [Birthday], [ProfilePicture], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (5, NULL, NULL, NULL, NULL, N'admin', N'ADMIN', N'admin@gmail.com', N'ADMIN@GMAIL.COM', 1, N'AQAAAAEAACcQAAAAEMn+5LpanvV9bsll6dORnfEmX53H6BsTKkqSL4UdiHG7MUrx8DQgv/oT253+nZsIcQ==', N'PEWVDZESGZVV6S353ZYZF6RBRAUO3PFC', N'8d192ce8-d28a-40bb-86e1-55b229ec2483', NULL, 0, 0, NULL, 1, 0)
INSERT [dbo].[Users] ([id], [FullName], [Address], [Birthday], [ProfilePicture], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (7, NULL, NULL, NULL, NULL, N'minh', N'MINH', N'nhatrang12005@gmail.com', N'NHATRANG12005@GMAIL.COM', 1, N'AQAAAAEAACcQAAAAEPyOpSXIPqY6nRp3DLZZBGXrTw9G3MeY1nS1IIGr3h2GpeNiYy6tZjWNoz9KR24eEA==', N'4VHP4IF6DV7TYAGPETNBPQWMU2F53VR6', N'127d4cff-506e-4990-a38b-bb8c4e740bd1', N'+840358684141', 1, 1, NULL, 1, 0)
INSERT [dbo].[Users] ([id], [FullName], [Address], [Birthday], [ProfilePicture], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (16, NULL, NULL, NULL, NULL, N'ducanh1998s@gmail.com', N'DUCANH1998S@GMAIL.COM', N'ducanh1998s@gmail.com', N'DUCANH1998S@GMAIL.COM', 1, N'AQAAAAEAACcQAAAAEEeyPZR2wVv8nlt24QE4fvjn4coar+6hJ4J0N/peTjZgyRyQ/zLtkEQBDgTamRSvBw==', N'5DZBDIS4BMCCSJDRMVV6YANJG7VGQBGP', N'849e929b-e339-467a-b329-a77653617dca', NULL, 0, 0, NULL, 1, 0)
INSERT [dbo].[Users] ([id], [FullName], [Address], [Birthday], [ProfilePicture], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (17, NULL, NULL, NULL, NULL, N'goldenmagnamon@yahoo.com', N'GOLDENMAGNAMON@YAHOO.COM', N'goldenmagnamon@yahoo.com', N'GOLDENMAGNAMON@YAHOO.COM', 1, N'AQAAAAEAACcQAAAAEI1HrZlFSalgKbqs5Nv6E8fjbXj/DSofSqsNpn6SwB2DICpXlJGT9CaVaRx7zxGytw==', N'6UK3P5YQ6KWOMWSRSC6XTP2LWD2K25K7', N'2d0e878c-c18f-4f38-82c3-29747d917941', NULL, 0, 0, NULL, 1, 0)
SET IDENTITY_INSERT [dbo].[Users] OFF
INSERT [dbo].[UserTokens] ([UserId], [LoginProvider], [Name], [Value]) VALUES (5, N'[AspNetUserStore]', N'AuthenticatorKey', N'DP4KYAMITYZADCD6OL4J3OB6WKAFDUBE')
INSERT [dbo].[UserTokens] ([UserId], [LoginProvider], [Name], [Value]) VALUES (5, N'[AspNetUserStore]', N'RecoveryCodes', N'52438552;29f1f52e;fe9463ef;40bfde50;caa4e9dd;a3ca6228;ed0d1330;56947d7f;e584eab9')
INSERT [dbo].[UserTokens] ([UserId], [LoginProvider], [Name], [Value]) VALUES (7, N'[AspNetUserStore]', N'AuthenticatorKey', N'SDWDKNEALASQAYNT2GQI7AAXE4RXOV6W')
INSERT [dbo].[UserTokens] ([UserId], [LoginProvider], [Name], [Value]) VALUES (7, N'[AspNetUserStore]', N'RecoveryCodes', N'd4826325;38019640;e4787ee1;bf656174;8e6fa4e2;0ff32f0e;e8faa157;b2f467d2;0b6b807a;4032e8a7')
ALTER TABLE [dbo].[OrderDetails]  WITH CHECK ADD  CONSTRAINT [FK_OrderDetails_Orders_OrderId] FOREIGN KEY([OrderId])
REFERENCES [dbo].[Orders] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[OrderDetails] CHECK CONSTRAINT [FK_OrderDetails_Orders_OrderId]
GO
ALTER TABLE [dbo].[OrderDetails]  WITH CHECK ADD  CONSTRAINT [FK_OrderDetails_Product_ProductId] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([ProductId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[OrderDetails] CHECK CONSTRAINT [FK_OrderDetails_Product_ProductId]
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK_Orders_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK_Orders_Users_UserId]
GO
ALTER TABLE [dbo].[ProductCategories]  WITH CHECK ADD  CONSTRAINT [FK_ProductCategories_Category_CategoryID] FOREIGN KEY([CategoryID])
REFERENCES [dbo].[Category] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ProductCategories] CHECK CONSTRAINT [FK_ProductCategories_Category_CategoryID]
GO
ALTER TABLE [dbo].[ProductCategories]  WITH CHECK ADD  CONSTRAINT [FK_ProductCategories_Product_ProductID] FOREIGN KEY([ProductID])
REFERENCES [dbo].[Product] ([ProductId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ProductCategories] CHECK CONSTRAINT [FK_ProductCategories_Product_ProductID]
GO
ALTER TABLE [dbo].[RoleClaims]  WITH CHECK ADD  CONSTRAINT [FK_RoleClaims_Roles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Roles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[RoleClaims] CHECK CONSTRAINT [FK_RoleClaims_Roles_RoleId]
GO
ALTER TABLE [dbo].[UserClaims]  WITH CHECK ADD  CONSTRAINT [FK_UserClaims_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserClaims] CHECK CONSTRAINT [FK_UserClaims_Users_UserId]
GO
ALTER TABLE [dbo].[UserLogins]  WITH CHECK ADD  CONSTRAINT [FK_UserLogins_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserLogins] CHECK CONSTRAINT [FK_UserLogins_Users_UserId]
GO
ALTER TABLE [dbo].[UserRoles]  WITH CHECK ADD  CONSTRAINT [FK_UserRoles_Roles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Roles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserRoles] CHECK CONSTRAINT [FK_UserRoles_Roles_RoleId]
GO
ALTER TABLE [dbo].[UserRoles]  WITH CHECK ADD  CONSTRAINT [FK_UserRoles_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserRoles] CHECK CONSTRAINT [FK_UserRoles_Users_UserId]
GO
ALTER TABLE [dbo].[UserTokens]  WITH CHECK ADD  CONSTRAINT [FK_UserTokens_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserTokens] CHECK CONSTRAINT [FK_UserTokens_Users_UserId]
GO
