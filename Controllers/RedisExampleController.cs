using Microsoft.AspNetCore.Mvc;

using redis_example_net.Interface;

namespace redis_example_net.Controllers
{
    [ApiController]
    [Route("/redis")]
    public class RedisExampleController : ControllerBase
    {
        private readonly IRedisService _redisService;

        public RedisExampleController(IRedisService redisService) {
            _redisService = redisService;
        }

        [HttpGet("string/{id}")]
        public async Task<IActionResult> GetString(string id)
        {
            var result = await _redisService.GetString(id);

            return Ok(result);
        }

        [HttpPost("string")]
        public async Task<IActionResult> SetString(string id, string value)
        {
            await _redisService.SetString(id, value);

            return Ok();
        }
    }
}