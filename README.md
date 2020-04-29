# SqlClrUtils

## Regex
SQL Server doesn't have built-in regex function. For this reason was implemented SQL CLR function **regex.Match**:
```sql
DECLARE @SourceText NVARCHAR(MAX) = N'<SomeNode value1="123" value2="abc" />';
DECLARE @RegexPattern NVARCHAR(MAX) = N'(?<prop>\w+)="(?<val>\w+)"';
DECLARE @IsCaseSensitive BIT = 0;

SELECT *
FROM regex.Match(@SourceText, @RegexPattern, @IsCaseSensitive);
```

Result:

| GroupNumber | IndexNumber | Length | MatchNumber | Value  | GroupName |
|-------------|-------------|--------|-------------|--------|-----------|
| 1           | 10          | 6      | 0           | value1 | prop      |
| 2           | 18          | 3      | 0           | 123    | val       |
| 1           | 23          | 6      | 1           | value2 | prop      |
| 2           | 31          | 3      | 1           | abc    | val       |

Also was added function **regex.Replace** for replace regex's matches for specific value. 
```sql
DECLARE @SourceText NVARCHAR(MAX) = N'My first line <br /> My second line';
DECLARE @RegexPattern NVARCHAR(MAX) = N'([<]br\s*/[>])';
DECLARE @Replacement NVARCHAR(MAX) = N''
DECLARE @IsCaseSensitive BIT = 0;

SELECT regex.Replace(@SourceText, @RegexPattern, @Replacement, @IsCaseSensitive);
```

Result:
My first line  My second line

## Hash
Standard HashBytes is limited by length of a string 8KB. Implimintation of this project doesn't have this limitation.
```sql
DECLARE @SourceText NVARCHAR(MAX) = N'Hello World';
DECLARE @Algorithm NVARCHAR(20) = N'SHA1';

SELECT hash.Match(@Algorithm, @SourceText);
```

Result:
0x0A4D55A8D778E5022FAB701977C5D840BBC486D0

## codeProxy
SQL Server doesn't allow to write nested INSERT INTO ... EXEC. It' possible only once. Else exception will be thrown **An INSERT EXEC statement cannot be nested**. In some situations it's really fetters. We get some procedure from our сolleagues, that already uses INSERT EXEC, and this procedure returns table. We must to transform this result, but INSERT INTO already used.
```sql
CREATE PROCEDURE MainProc
AS
BEGIN
  SELECT A = 1, B = 2;
END
GO

CREATE PROCEDURE SubProc
AS
BEGIN
  DECLARE @T TABLE (A INT, B INT);
  INSERT INTO @T EXEC MainProc;
  SELECT * FROM @T;
END
GO

-- ERROR: An INSERT EXEC statement cannot be nested.
DECLARE @T TABLE (A INT, B INT);
INSERT INTO @T (A, B)
EXEC SubProc
```

I suggest to use codeProxy.Execute:
```sql
-- Another solution is to use [codeProxy].
DECLARE @Query NVARCHAR(MAX);
DECLARE @Xml NVARCHAR(MAX);

SET @query = N'EXEC SubProc';

EXEC codeProxy.[Execute] @Query, @Xml OUT;

SELECT
 DasTable.X.value('(A[1])', 'INT'),
 DasTable.X.value('(B[1])', 'INT')
FROM (VALUES(CAST(@Xml AS XML))) AS XmlData(X)
CROSS APPLY XmlData.X.nodes('*/*') AS DasTable(X);
```
This solution also allow to build dynamic query while OPENQUERY require static text.
