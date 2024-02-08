using Bayada.Joy.WorkDayConnector.CustomException;
using Bayada.Joy.ZoomConnector.Contracts;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Bayada.Joy.ZoomConnector.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ZoomController : ControllerBase
    {
        private readonly ILogger<ZoomController> _logger;
        private readonly IZoomService _zoomService;
        public ZoomController(ILogger<ZoomController> logger, IZoomService zoomService)
        {
            _logger = logger;
            _zoomService = zoomService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateZoomMeeting()
        {
            try
            {
                var accessToken = HttpContext.Session.GetString("AccessToken");
                if (string.IsNullOrEmpty(accessToken) || _zoomService.IsAccessTokenExpired(accessToken))
                {
                    accessToken = await _zoomService.GetAccessToken();
                    HttpContext.Session.SetString("AccessToken", accessToken);
                }
                var data = await _zoomService.CreateZoomMeeting(accessToken);
                if (!string.IsNullOrEmpty(data))
                {
                    return Ok(data);
                }
                else
                {
                    return NoContent();
                }
            }
            catch (Exception ex) 
            {
                throw new ZoomConnectorException(ex.Message, ex.InnerException);
            }
        }
            
    }
}