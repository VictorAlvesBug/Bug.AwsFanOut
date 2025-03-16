using Amazon.DynamoDBv2.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

			if (attr.N != null)
			{
				if (attr.N.Contains('.'))
					return Convert.ToDecimal(attr.N);

				return Convert.ToInt32(attr.N);
			}

			if (attr.L != null) return attr.L.ConvertAll(item => item.ConvertToObject());
			if (attr.M != null) return attr.M.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ConvertToObject());
			if (attr.NULL) return null;
			return attr.BOOL;
		}

		public static AttributeValue ConvertToAttributeValue(this object? obj)
		{
			if (obj == null) return new AttributeValue { NULL = true };

			return obj switch
			{
				string s => new AttributeValue { S = s },
				int i => new AttributeValue { N = i.ToString() },
				long l => new AttributeValue { N = l.ToString() },
				decimal d => new AttributeValue { N = d.ToString(System.Globalization.CultureInfo.InvariantCulture) },
				bool b => new AttributeValue { BOOL = b },
				Enum e => new AttributeValue { N = Convert.ToInt32(e).ToString() },
				IList<object> list => new AttributeValue { L = list.Select(ConvertToAttributeValue).ToList() },
				IDictionary<string, object> dict => new AttributeValue
				{
					M = dict.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ConvertToAttributeValue())
				},
				_ => throw new InvalidOperationException($"Tipo não suportado: {obj.GetType()}")
			};
		}
	}
}
