using System.Collections;
using System.Data.SqlTypes;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Microsoft.SqlServer.Server;
using TextRegex = System.Text.RegularExpressions.Regex;

namespace Coding4fun.MsSql.Regex
{
	/// <summary>
	///     Регулярные выражения для Sql Server.
	/// </summary>
	[PublicAPI]
	public static class RegexExtension
	{
		/// <summary>
		///     Замена части текста, найденного регулярным выражением.
		/// </summary>
		/// <param name="inputText">Текст, в котором происходит поиск.</param>
		/// <param name="pattern">Шаблон регулярного выражения.</param>
		/// <param name="replacement">Чем заменять.</param>
		/// <param name="isCaseSensitive">Зависит от регистра?</param>
		/// <returns></returns>
		[SqlFunction(IsDeterministic = true, DataAccess = DataAccessKind.Read)]
		public static SqlString Replace(SqlString inputText, SqlString pattern, SqlString replacement,
			bool isCaseSensitive)
		{
			if (inputText.IsNull
			    || string.IsNullOrEmpty(inputText.Value)
			    || pattern.IsNull
			    || string.IsNullOrEmpty(pattern.Value)
			)
			{
				return SqlString.Null;
			}

			TextRegex regex = isCaseSensitive
				? new TextRegex(pattern.Value)
				: new TextRegex(pattern.Value, RegexOptions.IgnoreCase);

			string result = regex.Replace(inputText.Value, replacement.IsNull ? "" : replacement.Value);

			return new SqlString(result);
		}

		/// <summary>
		///     Поиск совпадений по регулярному выражению.
		/// </summary>
		/// <param name="inputText">Текст, в котором происходит поиск.</param>
		/// <param name="pattern">Шаблон регулярного выражения.</param>
		/// <param name="isCaseSensitive">Зависит от регистра?</param>
		/// <returns>Список найденных групп.</returns>
		[SqlFunction(FillRowMethodName = nameof(GetRegexRow), IsDeterministic = true, DataAccess = DataAccessKind.Read)]
		public static ArrayList Match(SqlString inputText, SqlString pattern, bool isCaseSensitive)
		{
			ArrayList result = new ArrayList();

			try
			{
				if (!inputText.IsNull
				    && !string.IsNullOrEmpty(inputText.Value)
				    && !pattern.IsNull
				    && !string.IsNullOrEmpty(pattern.Value)
				)
				{
					TextRegex regex = isCaseSensitive
						? new TextRegex(pattern.Value)
						: new TextRegex(pattern.Value, RegexOptions.IgnoreCase);


					MatchCollection matches = regex.Matches(inputText.Value);
					for (int matchNumber = 0; matchNumber < matches.Count; ++matchNumber)
					{
						Match match = matches[matchNumber];
						for (int groupNumber = 1; groupNumber < match.Groups.Count; ++groupNumber)
						{
							Group group = match.Groups[groupNumber];
							string groupName = regex.GroupNameFromNumber(groupNumber);
							MatchGroupRow resultItem = new MatchGroupRow
							{
								GroupName = groupName,
								GroupNumber = groupNumber,
								Index = group.Index,
								Length = group.Length,
								MatchNumber = matchNumber,
								Value = group.Value
							};
							result.Add(resultItem);
						}
					}
				}
			}
			catch
			{
				// ignored
			}

			return result;
		}


		private static void GetRegexRow(
			object matchObject,
			out SqlInt32 groupNumber,
			out SqlInt32 index,
			out SqlInt32 length,
			out SqlInt32 matchNumber,
			out SqlString value,
			out SqlString groupName)
		{
			if (matchObject != null)
			{
				MatchGroupRow match = (MatchGroupRow) matchObject;
				groupNumber = match.GroupNumber;
				index = match.Index;
				length = match.Length;
				matchNumber = match.MatchNumber;
				value = match.Value;
				groupName = string.IsNullOrEmpty(match.GroupName) ? SqlString.Null : new SqlString(match.GroupName);
			}
			else
			{
				groupNumber = SqlInt32.Null;
				index = SqlInt32.Null;
				length = SqlInt32.Null;
				matchNumber = SqlInt32.Null;
				value = SqlString.Null;
				groupName = SqlString.Null;
			}
		}
	}
}