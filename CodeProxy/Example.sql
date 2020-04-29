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