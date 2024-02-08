namespace Bayada.Joy.ZoomConnector.ConfigOptions
{
    public class ZoomSettings
    {
        public EndPoint EndPoints { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string AccountId { get; set; }
    }
    public class EndPoint
    {
        public string Auth { get; set; }
        public string CreateMeeting { get; set; }

    }
}
