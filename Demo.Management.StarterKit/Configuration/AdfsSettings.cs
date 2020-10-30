namespace StarterKit.Configuration
{
	public class AdfsSettings
	{
		public const string Key = "Adfs";

		public string PublicCertificatePath { get; set; }
		public string Issuer { get; set; }
		public string ClientId { get; set; }
		public string Url { get; set; }
	}
}