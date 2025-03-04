using Bug.Domain.Entities;
using Bug.Helpers.Extensions;
using Bug.Infrastructure.Services;

var _dynamoDbService = new DynamoDbService();

var options = new Dictionary<string, Func<Task>>
{
	{
		"Salvar Nova Integração de DrawNumber",
		async () =>
		{
			const string messageGroupId = "groupId-0";

			var drawNumberPayload = new DrawNumberPayload{
				 MinLimit = 0,
				 MaxLimit = 100
			};

			var itemToPut = new Integration
			{
				PK = Guid.NewGuid().ToString(),
				SK = (decimal)DateTime.Now.ToOADate(),
				Status = IntegrationStatus.Pending,
				Type = IntegrationType.DrawNumber,
				Body = drawNumberPayload.ToJson(),
				//Body = JsonConvert.SerializeObject(new { MessageGroupId = messageGroupId, AnotherField = DateTime.Now }),
				FifoMessageGroupId = messageGroupId,
				CreatedAt = DateTime.Now
			};

			await _dynamoDbService.PutItemAsync(itemToPut);
		}
	},
	{
		"Listar Integrações",
		async () =>
		{
			var list = await _dynamoDbService.GetAllAsync();

			list.PrintTable();
		}
	}
};



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