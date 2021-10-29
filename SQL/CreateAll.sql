USE [Example]
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Employee]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Employee](
	[EmployeeId] [bigint] IDENTITY(1,1) NOT NULL,
	[EmployeeName] [nvarchar](50) NOT NULL,
	[EmployeeEmail] [nvarchar](50) NOT NULL,
	[DateHired] [datetime] NOT NULL,
	[Salary] [money] NOT NULL,
	[Description] [nvarchar](max) NULL,
	[ManagedByEmployeeId] [bigint] NULL,
 CONSTRAINT [PK_Employee] PRIMARY KEY CLUSTERED 
(
	[EmployeeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EmployeeData]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[EmployeeData](
	[EmployeeDataId] [bigint] IDENTITY(1,1) NOT NULL,
	[EmployeeId] [bigint] NOT NULL,
	[EmployeeDataTypeId] [bigint] NOT NULL,
	[EmployeeDataValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_EmployeeData] PRIMARY KEY CLUSTERED 
(
	[EmployeeDataId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EmployeeDataType]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[EmployeeDataType](
	[EmployeeDataTypeId] [bigint] NOT NULL,
	[EmployeeDataTypeName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_EmployeeDataType] PRIMARY KEY CLUSTERED 
(
	[EmployeeDataTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO


IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Employee]') AND name = N'IX_Employee')
CREATE UNIQUE NONCLUSTERED INDEX [IX_Employee] ON [dbo].[Employee]
(
	[EmployeeEmail] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[EmployeeData]') AND name = N'IX_EmployeeData')
CREATE UNIQUE NONCLUSTERED INDEX [IX_EmployeeData] ON [dbo].[EmployeeData]
(
	[EmployeeId] ASC,
	[EmployeeDataTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Employee_Employee]') AND parent_object_id = OBJECT_ID(N'[dbo].[Employee]'))
ALTER TABLE [dbo].[Employee]  WITH CHECK ADD  CONSTRAINT [FK_Employee_Employee] FOREIGN KEY([ManagedByEmployeeId])
REFERENCES [dbo].[Employee] ([EmployeeId])
GO


IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Employee_Employee]') AND parent_object_id = OBJECT_ID(N'[dbo].[Employee]'))
ALTER TABLE [dbo].[Employee] CHECK CONSTRAINT [FK_Employee_Employee]
GO


IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_EmployeeData_Employee]') AND parent_object_id = OBJECT_ID(N'[dbo].[EmployeeData]'))
ALTER TABLE [dbo].[EmployeeData]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeData_Employee] FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[Employee] ([EmployeeId])
GO


IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_EmployeeData_Employee]') AND parent_object_id = OBJECT_ID(N'[dbo].[EmployeeData]'))
ALTER TABLE [dbo].[EmployeeData] CHECK CONSTRAINT [FK_EmployeeData_Employee]
GO


IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_EmployeeData_EmployeeDataType]') AND parent_object_id = OBJECT_ID(N'[dbo].[EmployeeData]'))
ALTER TABLE [dbo].[EmployeeData]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeData_EmployeeDataType] FOREIGN KEY([EmployeeDataTypeId])
REFERENCES [dbo].[EmployeeDataType] ([EmployeeDataTypeId])
GO


IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_EmployeeData_EmployeeDataType]') AND parent_object_id = OBJECT_ID(N'[dbo].[EmployeeData]'))
ALTER TABLE [dbo].[EmployeeData] CHECK CONSTRAINT [FK_EmployeeData_EmployeeDataType]
GO


USE [master]
GO


ALTER DATABASE [Example] SET  READ_WRITE 
GO
