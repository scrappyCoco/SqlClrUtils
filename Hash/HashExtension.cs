using System.Data.SqlTypes;
using System.Security.Cryptography;
using System.Text;
using JetBrains.Annotations;
using Microsoft.SqlServer.Server;

namespace Coding4fun.MsSql.Hash
{
	/// <summary>
	///     Вычисление hash строки. Стандартная функция HashBytes ограничена длиной строки равной 8КБ.
	///     В данной реализации такого ограничения нет.
	/// </summary>
	[PublicAPI]
	public class HashExtension
	{
		/// <summary>
		///     Получение hash для строки.
		/// </summary>
		/// <param name="algorithm">SHA1 | MD5 | SHA256 | SHA384 | SHA512.</param>
		/// <param name="data">Строка, для которой необходимо получить hash.</param>
		[SqlFunction]
		public static SqlBinary Match(SqlString algorithm, SqlString data)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(data.Value);

			using (HashAlgorithm hashAlgorithm = HashAlgorithm.Create(algorithm.Value))
			{
				return new SqlBinary(hashAlgorithm.ComputeHash(bytes));
			}
		}
	}
}