using Microsoft.AspNetCore.Mvc;

using redis_example_net.Interface;

namespace redis_example_net.Controllers
{
    [ApiController]
    [Route("/cache")]
    public class CacheExampleController : ControllerBase
    {
        private readonly ICacheService _cacheService;

        public CacheExampleController(ICacheService cacheService) {
            _cacheService = cacheService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCache(string id)
        {
            var result = await _cacheService.GetCache(id);

            return Ok(result);
        }
    }
}