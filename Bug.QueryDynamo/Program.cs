using Bug.Domain.Entities.DrawNumber;
using Bug.Domain.Entities.Integration;
using Bug.Helpers.Extensions;
using Bug.Infrastructure.Services;

var _integrationService = new IntegrationService();

var options = new Dictionary<string, Func<Task>>
{
	{
		"Salvar Nova Integração de DrawNumber",
		async () =>
		{
			const string messageGroupId = "groupId-0";

			var drawNumberPayload = new DrawNumberPayload
			{
				 MinLimit = 0,
				 MaxLimit = 100
			};

			await _integrationService.PutAsync(new IntegrationToPut
			{
				Type = IntegrationType.DrawNumber,
				Body = drawNumberPayload.ToJson(),
				FifoMessageGroupId = messageGroupId
			});
		}
	},
	{
		"Listar Integrações",
		async () =>
		{
			var list = await _integrationService.GetAllAsync();

			list.PrintTable();
		}
	}
};

/*
IMPLEMENTAR
UPDATE DE INTEGRAÇÕES, PARA ALTERAR STATUS PARA PROCESSANDO, PROCESSADO E FALHA
ADICIONAR ATRIBUTO NAS INTEGRAÇÕES PARA ARMAZENAR RETORNO STRING
IMPLEMENTAR RETORNO EM CADA UMA DAS INTEGRAÇÕES
VERIFICAR O MOTIVO DE ESTAR DANDO ERRO NAS CREDENCIAIS
LIMPAR CONSOLE DO QD LOGO ANTES DE EXECUTAR A AÇÃO SOLICITADA, PARA MANTER SEMPRE LIMPO

*/


bool ConfirmBeforeExecute(string message)
{
	return true;

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