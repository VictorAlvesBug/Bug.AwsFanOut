namespace Bug.Domain.Entities
{
	public class DrawNumberPayload
	{
		public int DrawedNumber { get; set; }
		public int MinLimit { get; set; }
		public int MaxLimit { get; set; }
	}
}
