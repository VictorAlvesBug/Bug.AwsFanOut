using Bug.Domain.Entities;
using Bug.Infrastructure.Services;
using Newtonsoft.Json;
using System.Collections.Generic;

var _dynamoDbService = new DynamoDbService();

var options = new Dictionary<string, Func<Task>>
{
	{
		"Salvar Nova Integração",
		async () =>
		{
			var messageGroupId = "groupId-0";

			var itemToPut = new Integration
			{
				PK = Guid.NewGuid().ToString(),
				SK = (decimal)DateTime.Now.ToOADate(),
				Status = IntegrationStatus.Pending,
				Body = JsonConvert.SerializeObject(new { MessageGroupId = messageGroupId, AnotherField = DateTime.Now }),
				FifoMessageGroupId = messageGroupId
			};

			await _dynamoDbService.PutItemAsync(itemToPut);

			var item = await _dynamoDbService.GetItemAsync(itemToPut.PK, itemToPut.SK);
		}
	},
	{
		"Listar Integrações",
		async () =>
		{
			var list = await _dynamoDbService.GetAllAsync();

			PrintTable(list);
			//Console.WriteLine(JsonConvert.SerializeObject(list, Formatting.Indented));
		}
	}
};

void PrintTable<T>(List<T> list)
{
	if (list is null)
		throw new ArgumentNullException(nameof(list));

	if (list.Count == 0)
		return;

	Dictionary<string, int> columnsLength = 
		list.FirstOrDefault()
		.GetType().GetProperties()
		.Select(prop => new KeyValuePair<string, int>(prop.Name, prop.Name.Length ))
		.ToDictionary();

	foreach (var item in list)
	{
		item.GetType().GetProperties()
			.ToList().ForEach((prop) => {
				columnsLength[prop.Name] =
					Math.Max(columnsLength[prop.Name], Convert.ToInt32(prop.GetValue(item).ToString().Length));
			});
	}
	
	columnsLength
		.ToList().ForEach((kvp) =>
		{
			var (name, length) = kvp;
			Console.Write($"{kvp.Key.PadRight(length, ' ')} | ");
		});

	Console.WriteLine();

	foreach (var item in list)
	{

		item.GetType().GetProperties()
			.ToList().ForEach((prop) => {
				var length = columnsLength[prop.Name];
				Console.Write($"{prop.GetValue(item).ToString().PadRight(length, ' ')} | ");
			});

		Console.WriteLine();
	}
}

bool ConfirmBeforeExecute(string message)
{
	Console.WriteLine(message);
	Console.WriteLine("Opções: sim, não");
	var selectedOption = Console.ReadLine().ToLower();
	return new string[] { "s", "sim" }.Contains(selectedOption);
}

async Task OpenMenuAsync()
{
	KeyValuePair<string, Func<Task>> optionSelected = default;

	while (true)
	{
		Console.WriteLine("Qual opção deseja executar?");

		for (var index = 0; index < options.Count; index++)
		{
			var (name, func) = options.ElementAt(index);
			Console.WriteLine($"{index + 1:00} - {name};");
		}

		var indexSelected = Console.ReadLine();

		var isValid = int.TryParse(indexSelected, out var intIndexSelected)
			&& intIndexSelected >= 1
			&& intIndexSelected <= options.Count;

		if (isValid)
		{
			optionSelected = options.ElementAt(intIndexSelected - 1);
			break;
		}

		Console.WriteLine($"Informe uma opção entre 1 e {options.Count}\n");
	}

	if (ConfirmBeforeExecute($"\nDeseja executar a opção '{optionSelected.Key}'?"))
	{
		Console.WriteLine();
		await optionSelected.Value.Invoke();
	}
	
	if (ConfirmBeforeExecute("\nDeseja retornar ao menu?"))
	{
		Console.WriteLine();
		await OpenMenuAsync();
	}
}

await OpenMenuAsync();