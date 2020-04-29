ALTER DATABASE [Utils] SET TRUSTWORTHY ON;
GO

USE Utils;
GO

CREATE ASSEMBLY CodeProxy
    FROM 'D:\CodeProxy.dll'
    WITH PERMISSION_SET = UNSAFE;
GO

CREATE SCHEMA [codeProxy];
GO

CREATE PROCEDURE codeProxy.ExecuteInternal
    @Query NVARCHAR(MAX),
    @XmlOfDataTable NVARCHAR(MAX) OUT
AS EXTERNAL NAME CodeProxy.[Coding4fun.MsSql.CodeProxy.CodeProxyExtension].Execute;
GO

CREATE PROCEDURE codeProxy.Execute
    @Query NVARCHAR(MAX),
    @XmlOfDataTable NVARCHAR(MAX) OUT
AS
BEGIN
    EXEC codeProxy.ExecuteInternal @Query, @XmlOfDataTable;
END