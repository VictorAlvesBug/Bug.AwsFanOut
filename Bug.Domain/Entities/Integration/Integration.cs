namespace Bug.Domain.Entities.Integration
{
	public class Integration
	{
		public string PK { get; set; }
		public string SK { get; set; }
		public IntegrationStatus Status { get; set; }
		public IntegrationType Type { get; set; }
		public string Body { get; set; }
		public string FifoMessageGroupId { get; set; }
		public DateTime CreatedAt { get; set; }
		public string ResponseMessage { get; set; }

		public Integration()
		{

		}

		public Integration(IntegrationToPut integrationToPut)
		{
			PK = Guid.NewGuid().ToString();
			SK = DateTime.Now.ToString("yyyy-MM-dd");
			Status = IntegrationStatus.Pending;
			Type = integrationToPut.Type;
			Body = integrationToPut.Body;
			FifoMessageGroupId = integrationToPut.FifoMessageGroupId;
			CreatedAt = DateTime.Now;
		}

	}
}
