ALTER DATABASE [Utils] SET TRUSTWORTHY ON;
GO

USE Utils;
GO

CREATE ASSEMBLY Hash   
FROM 'D:\Hash.dll'
WITH PERMISSION_SET = UNSAFE;
GO

CREATE SCHEMA [hash];
GO


CREATE FUNCTION hash.MatchInternal (
  @algorithm NVARCHAR(20),
  @data nvarchar(max)
 )
RETURNS VARBINARY(64)
EXTERNAL NAME Hash.[Coding4fun.MsSql.Hash.HashExtension].Match;
GO

CREATE FUNCTION hash.Match (
  @algorithm NVARCHAR(20),
  @data nvarchar(max)
 )
RETURNS VARBINARY(64)
AS
BEGIN
  RETURN hash.MatchInternal(@algorithm, @data);
END