using Newtonsoft.Json;

namespace Bug.Helpers.Extensions
{
	public static class StringExtensions
	{
		public static ObjectType SafeParse<ObjectType>(this string jsonObject)
		{
			var obj = JsonConvert.DeserializeObject<ObjectType>(jsonObject);

			if (obj == null)
			{
				throw new Exception($"Erro ao deserializar {nameof(jsonObject)} para o tipo {nameof(ObjectType)}." +
					$"\n{nameof(jsonObject)}: {jsonObject}");
			}

			return obj;
		}
	}
}
