namespace Bug.Domain.Entities.DrawNumber
{
	public class DrawNumberPayload
	{
		public int MinLimit { get; set; }
		public int MaxLimit { get; set; }
		public int DrawedNumber { get; set; }
		public int Guess { get; set; }
		public GuessFeedback GuessFeedback { get; set; }
		public int NumberOfAttempts { get; set; }
	}
}
