namespace Bug.Domain.Entities.Integration
{
	public class IntegrationToPut
	{
		public IntegrationType Type { get; set; }
		public string Body { get; set; }
		public string FifoMessageGroupId { get; set; }

	}
}
