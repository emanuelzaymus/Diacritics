using System.Threading.Tasks;
using Diacritics;
using DiacriticsWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace DiacriticsWeb.Controllers
{
    [Route("api/[controller]")]
    public class ReconstructorController : Controller
    {

        // POST api/<controller>
        [HttpPost]
        public async Task<ActionResult<ApiText>> PostApiText([FromBody]ApiText apiText)
        {
            if (apiText.Text.Length > 10000)
            {
                apiText.Text = apiText.Text.Substring(0, 10000);
            }

            var reconstructor = new Reconstructor(Startup.BinaryFilePath, Startup.PositionTriePath);
            apiText.Text = reconstructor.Reconstruct(apiText.Text);

            return CreatedAtAction(null, apiText);
        }

    }
}
