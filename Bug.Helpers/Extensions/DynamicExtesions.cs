using System.Text.Json;

namespace Bug.Helpers.Extensions
{
	public static class DynamicExtesions
	{
		public static string ToJson<ObjectType>(this ObjectType obj)
		{
			return JsonSerializer.Serialize(obj, typeof(ObjectType));
		}
	}
}
