namespace Bayada.Joy.ZoomConnector.Contracts
{
    public interface IZoomService
    {
        Task<string> GetAccessToken();
        Task<string> CreateZoomMeeting(string accessToken);
        bool IsAccessTokenExpired(string accessToken);
    }
}