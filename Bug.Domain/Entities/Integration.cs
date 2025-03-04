using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bug.Domain.Entities
{
	public class Integration
	{
		public string PK { get; set; }
		public decimal SK { get; set; }
		public IntegrationStatus Status { get; set; }
		public IntegrationType Type { get; set; }
		public string Body { get; set; }
		public string FifoMessageGroupId { get; set; }
		public DateTime CreatedAt { get; set; }

	}
}
