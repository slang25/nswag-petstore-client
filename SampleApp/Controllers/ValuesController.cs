using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.PetStore.Client;

namespace SampleApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        readonly IPetStoreClient petStoreClient;

        public ValuesController(IPetStoreClient petStoreClient)
        {
            this.petStoreClient = petStoreClient;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> Get(CancellationToken ct)
        {
            var stu = await petStoreClient.GetUserByNameAsync("Stu", ct);
            return new[] { stu.Email };
        }
    }
}