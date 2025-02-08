using Amazon.DynamoDBv2.Model;
using Newtonsoft.Json;

namespace Bug.Helpers.Extensions
{
	public static class DynamoExtensions
	{
		public static string ToJson(this Dictionary<string, AttributeValue> item)
		{
			var normalDict = item.ToDictionary(
				kvp => kvp.Key,
				kvp => kvp.Value.ConvertToObject()
			);

			return JsonConvert.SerializeObject(normalDict, Formatting.Indented);
		}

		public static object? ConvertToObject(this AttributeValue attr)
		{
			if (attr.S != null) return attr.S;
			if (attr.N != null) return Convert.ToDecimal(attr.N);
			if (attr.L != null) return attr.L.ConvertAll(item => item.ConvertToObject());
			if (attr.M != null) return attr.M.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ConvertToObject());
			if (attr.NULL) return null;
			return attr.BOOL;
		}
	}
}
