using System;

namespace StarterKit.Logging.Handlers
{
    public class CorrelationIdHandler : ICorrelationIdHandler
	{
		public void EnsureCorrelationId()
		{
			var id = CallContext<Guid>.GetData(CorrelationIdConstants.CorrelationId).ToString();

			if (string.IsNullOrEmpty(id))
				SetCorrelationId(Guid.NewGuid());
		}

		public void SetCorrelationId(Guid correlationId)
		{
			CallContext<Guid>.SetData(CorrelationIdConstants.CorrelationId, correlationId);
		}
		public Guid GetCorrelationId()
		{
			return Guid.Parse(CallContext<Guid>.GetData(CorrelationIdConstants.CorrelationId).ToString());
		}
	}
}