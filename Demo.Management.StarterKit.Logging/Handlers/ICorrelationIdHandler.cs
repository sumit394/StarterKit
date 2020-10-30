using System;

namespace StarterKit.Logging.Handlers
{
    public interface ICorrelationIdHandler
	{
		void EnsureCorrelationId();
		void SetCorrelationId(Guid correlationId);
		Guid GetCorrelationId();
	}
}