using Amazon.DynamoDBv2.Model;
using Bug.Helpers.Extensions;
using System.Text;

namespace Bug.Helpers.Utils
{
	public static class DynamoUtils
	{
		public static string BuildUpdateExpression(this List<string> attributesToUpdate)
		{
			var sb = new StringBuilder();

			sb.Append("SET ");

			for (var index=0; index<attributesToUpdate.Count; index++){
				var attributeName = attributesToUpdate.ElementAt(index);
				sb.Append($"#{attributeName} = :{attributeName}");

				if(index < attributesToUpdate.Count - 1)
					sb.Append(", ");
			}

			return sb.ToString();
		}

		public static Dictionary<string, string> BuildExpressionAttributeNames(this List<string> attributesToUpdate)
		{
			return attributesToUpdate.ToDictionary(
				attributeName => $"#{attributeName}",
				attributeName => attributeName
			);
		}

		public static Dictionary<string, AttributeValue> BuildExpressionAttributeValues<ObjectType>(
			this List<string> attributesToUpdate, 
			ObjectType obj)
		{
			return attributesToUpdate.ToDictionary(
				attributeName => $":{attributeName}",
				attributeName => obj?.GetType().GetProperty(attributeName)?
					.GetValue(obj, null).ConvertToAttributeValue()
					?? throw new Exception($"Atributo '{attributeName}' não encontrado no objeto '{obj.ToJson()}'")
			);
		}
	}
}
