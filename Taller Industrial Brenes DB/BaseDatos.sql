CREATE DATABASE [TallerBrenes]
GO
USE [TallerBrenes]
GO

/****** Object:  Table [dbo].[Roles]    Script Date: 29/05/2025 21:52:54 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Roles](
	[RolID] [bigint] IDENTITY(1,1) NOT NULL,
	[NombreRol] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED 
(
	[RolID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Usuarios]    Script Date: 29/05/2025 21:53:21 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Usuarios](
	[UsuarioID] [bigint] IDENTITY(1,1) NOT NULL,
	[Identificacion] [varchar](15) NOT NULL,
	[Nombre] [varchar](50) NOT NULL,
	[ApellidoPaterno] [varchar](50) NOT NULL,
	[ApellidoMaterno] [varchar](50) NOT NULL,
	[Correo] [varchar](100) NOT NULL,
	[Contrasenna] [varchar](255) NOT NULL,
	[TieneContrasennaTemp] [bit] NULL,
	[FechaVencimientoTemp] [datetime] NULL,
	[Estado] [bit] NOT NULL,
	[RolID] [bigint] NOT NULL,
 CONSTRAINT [PK_Usuarios] PRIMARY KEY CLUSTERED 
(
	[UsuarioID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Usuarios]  WITH NOCHECK ADD  CONSTRAINT [FK_Usuarios_Roles] FOREIGN KEY([RolID])
REFERENCES [dbo].[Roles] ([RolID])
GO

ALTER TABLE [dbo].[Usuarios] CHECK CONSTRAINT [FK_Usuarios_Roles]
GO

/*Insert*/
SET IDENTITY_INSERT [dbo].[Usuarios] ON 

INSERT [dbo].[Usuarios] ([UsuarioID], [Identificacion], [Nombre], [ApellidoPaterno], [ApellidoMaterno], [Correo], [Contrasenna], [TieneContrasennaTemp], [FechaVencimientoTemp], [Estado], [RolID]) VALUES (1, '604970701', 'Paola', 'Segura', 'Bellanero', 'pao@gmail.com', 'paola', NULL, NULL, 1, 1)
INSERT [dbo].[Usuarios] ([UsuarioID], [Identificacion], [Nombre], [ApellidoPaterno], [ApellidoMaterno], [Correo], [Contrasenna], [TieneContrasennaTemp], [FechaVencimientoTemp], [Estado], [RolID]) VALUES (2, '104970701', 'Cliente', 'Segura', 'Bellanero', 'psegura70701@ufide.ac.cr', '123456', NULL, NULL, 1, 2)
SET IDENTITY_INSERT [dbo].[Usuarios] OFF
GO

SET IDENTITY_INSERT [dbo].[Roles] ON 

INSERT [dbo].[Roles] ([RolID], [NombreRol]) VALUES (1, 'Administrador')
INSERT [dbo].[Roles] ([RolID], [NombreRol]) VALUES (2, 'Usuario')
SET IDENTITY_INSERT [dbo].[Roles] OFF
GO









