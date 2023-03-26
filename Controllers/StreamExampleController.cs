using Microsoft.AspNetCore.Mvc;

using redis_example_net.Interface;

namespace redis_example_net.Controllers
{
    [ApiController]
    [Route("/redis")]
    public class StreamExampleController : ControllerBase
    {
        private readonly IRedisService _redisService;

        public StreamExampleController(IRedisService redisService) {
            _redisService = redisService;
        }

        [HttpPost("stream")]
        public async Task<IActionResult> SetStream(string userId, string productId, string price)
        {
            Dictionary<String, String> dic = new Dictionary<String, String>();

            dic.Add("userId", userId);
            dic.Add("productId", productId);
            dic.Add("price", price);

            await _redisService.SetStream("order", dic);

            return Ok();
        }
    }
}