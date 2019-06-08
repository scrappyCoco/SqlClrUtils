ALTER DATABASE [Utils] SET TRUSTWORTHY ON;
GO

USE Utils;
GO

CREATE ASSEMBLY Regex   
FROM 'D:\Regex.dll'
WITH PERMISSION_SET = UNSAFE;
GO

CREATE SCHEMA regex;
GO


CREATE FUNCTION regex.MatchInternal (
  @inputText NVARCHAR(MAX),
  @pattern nvarchar(MAX),
  @isCaseSensitive BIT
)
RETURNS TABLE
(
  GroupNumber INT,
  IndexNumber INT,
  Length INT,
  MatchNumber INT,
  Value NVARCHAR(MAX),
  GroupName NVARCHAR(500)
)
EXTERNAL NAME Regex.[Coding4fun.MsSql.Regex.RegexExtension].Match
GO

CREATE FUNCTION regex.Match (
  @inputText NVARCHAR(MAX),
  @pattern NVARCHAR(MAX),
  @isCaseSensitive BIT
)
RETURNS TABLE
AS
RETURN
  SELECT *
  FROM regex.MatchInternal(@inputText, @pattern, @isCaseSensitive);
GO

CREATE FUNCTION regex.ReplaceInternal (
  @inputText NVARCHAR(MAX),
  @pattern NVARCHAR(MAX),
  @replacement NVARCHAR(MAX),
  @isCaseSensitive BIT
)
RETURNS NVARCHAR(MAX)
EXTERNAL NAME Regex.[Coding4fun.MsSql.Regex.RegexExtension].Replace;
GO

CREATE FUNCTION regex.Replace (
  @inputText NVARCHAR(MAX),
  @pattern NVARCHAR(MAX),
  @replacement NVARCHAR(MAX),
  @isCaseSensitive BIT
)
RETURNS NVARCHAR(MAX)
AS
BEGIN
  RETURN regex.ReplaceInternal(@inputText, @pattern, @replacement, @isCaseSensitive);
END