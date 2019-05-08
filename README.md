# SqlClrUtils

## Regex
В SQL Server нет встроенного механизма для поиска по регулярному выражению. Для этого реализована SQL CLR функция regex.Match:
```sql
DECLARE @SourceText NVARCHAR(MAX) = N'<SomeNode value1="123" value2="abc" />';
DECLARE @RegexPattern NVARCHAR(MAX) = N'(?<prop>\w+)="(?<val>\w+)"';
DECLARE @IsCaseSensitive BIT = 0;

SELECT *
FROM regex.Match(@SourceText, @RegexPattern, @IsCaseSensitive);
```

Результат:

| GroupNumber | IndexNumber | Length | MatchNumber | Value  | GroupName |
|-------------|-------------|--------|-------------|--------|-----------|
| 1           | 10          | 6      | 0           | value1 | prop      |
| 2           | 18          | 3      | 0           | 123    | val       |
| 1           | 23          | 6      | 1           | value2 | prop      |
| 2           | 31          | 3      | 1           | abc    | val       |

## Hash
Стандартная функция HashBytes ограничена длиной строки равной 8КБ. В данной реализации такого ограничения нет.
```sql
DECLARE @SourceText NVARCHAR(MAX) = N'Hello World';
DECLARE @Algorithm NVARCHAR(20) = N'SHA1';

SELECT hash.Match(@Algorithm, @SourceText);
```

Результат:
0x0A4D55A8D778E5022FAB701977C5D840BBC486D0
