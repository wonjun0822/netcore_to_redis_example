using Microsoft.AspNetCore.Mvc;
using redis_example_net.Attributes;

namespace redis_example_net.Controllers
{
    [ApiController]
    [Route("/session")]
    public class SessionExampleController : ControllerBase
    {
        [HttpGet]
        [Authorize(role = "user")]
        public async Task<IActionResult> GetSession()
        {
            // 저장소에서 세션 비동기 로드
            // 먼저 호출되지 않는 경우 동기적으로 로드 됨
            await HttpContext.Session.LoadAsync();

            return Ok(HttpContext.Session.GetString("id"));
        }

        [HttpPost]
        public async Task<IActionResult> SetSession(string id)
        {
            HttpContext.Session.SetString("id", id);
            HttpContext.Session.SetString("role", "user");

            // 저장소에서 세션 비동기 저장
            await HttpContext.Session.CommitAsync();

            return Ok();
        }

        [HttpDelete]
        [Authorize(role = "user")]
        public async Task<IActionResult> DeleteSession()
        {
            await HttpContext.Session.LoadAsync();

            HttpContext.Session.Clear();

            return NoContent();
        }
    }
}