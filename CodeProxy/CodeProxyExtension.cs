using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Text;
using Microsoft.SqlServer.Server;

namespace Coding4fun.MsSql.CodeProxy
{
	public class CodeProxyExtension
	{
		[SqlProcedure]
		public static void Execute(SqlString query, out SqlString xmlOfDataTable)
		{
			xmlOfDataTable = null;
			if (query.IsNull || string.IsNullOrWhiteSpace(query.Value)) return;

			using (var connection = CreateContextConnection())
			using (var command = new SqlCommand(query.Value, connection))
			using (var reader = command.ExecuteReader())
			using (var dataTable = new DataTable("ProxyDataTable"))
			using (var xmlMemoryStream = new MemoryStream())	
			{
				dataTable.Load(reader);
				dataTable.WriteXml(xmlMemoryStream);
				xmlMemoryStream.Position = 0;
				xmlOfDataTable = new SqlString(Encoding.UTF8.GetString(xmlMemoryStream.ToArray()));
			}
		}

		private static SqlConnection CreateContextConnection()
		{
			var connection = new SqlConnection("Context Connection=true");
			connection.Open();
			return connection;
		}
	}
}