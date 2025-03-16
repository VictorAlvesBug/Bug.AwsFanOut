using System;
using System.Text.Json;
using System.Xml.Linq;

namespace Bug.Helpers.Extensions
{
	public static class DynamicExtensions
	{
		public static string ToJson<ObjectType>(this ObjectType obj)
		{
			return JsonSerializer.Serialize(obj, typeof(ObjectType));
		}

		public static void PrintTable<ObjectType>(this IEnumerable<ObjectType> list)
		{
			if (list is null)
				throw new ArgumentNullException(nameof(list));

			if (!list.Any())
			{
				Console.WriteLine($"Nenhum item na lista de '{typeof(ObjectType).Name}'");
				return;
			}

			var propList = typeof(ObjectType).GetProperties().ToList();

			Dictionary<string, int> columnsLength =
				propList
				.Select(prop => new KeyValuePair<string, int>(prop.Name, prop.Name.Length))
				.ToDictionary();

			// Iterando cada item e recuperando o tamanho do maior valor de cada coluna
			foreach (var item in list)
			{
				for (var index = 0; index < propList.Count; index++)
				{
					var prop = propList[index];
					var currentLength = columnsLength[prop.Name];

					var strValue = prop.GetValue(item)?.ToString() ?? string.Empty;
					var possibleNewLength = Convert.ToInt32(strValue.Length);

					columnsLength[prop.Name] = Math.Max(currentLength, possibleNewLength);
				}
			}

			var headerHorizontalSeparator = string.Empty;

			// Exibindo cabeçalho, com o nome de cada uma das colunas
			for (var index = 0; index < columnsLength.Count; index++)
			{
				var (name, length) = columnsLength.ElementAt(index);

				var columnToPrint = name.PadRight(length, ' ');

				var isLastItem = columnsLength.Count == index + 1;
				var separator = isLastItem ? string.Empty : " | ";

				Console.Write(columnToPrint + separator);

				var hiphens = string.Join(string.Empty, Enumerable.Repeat('-', length));
				var headerVerticalSeparator = isLastItem ? string.Empty : "-|-";
				headerHorizontalSeparator += hiphens + headerVerticalSeparator;
			}

			// Exibindo linha horizontal que separa o cabeçalho do conteúdo
			Console.Write($"\n{headerHorizontalSeparator}\n");

			// Exibindo itens
			foreach (var item in list)
			{
				for (var index=0; index < propList.Count; index++)
				{
					var prop = propList[index];

					var length = columnsLength[prop.Name];
					var strValue = prop.GetValue(item)?.ToString() ?? string.Empty;

					var columnToPrint = strValue.PadRight(length, ' ');

					var isLastItem = columnsLength.Count == index + 1;
					var separator = isLastItem ? string.Empty : " | ";

					Console.Write(columnToPrint + separator);
				}

				Console.Write("\n");
			}
		}
	}
}
