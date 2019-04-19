using System.Threading.Tasks;
using Diacritics;
using DiacriticsWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace DiacriticsWeb.Controllers
{
    [Route("api/[controller]")]
    public class ReconstructorController : Controller
    {
        private readonly Reconstructor reconstructor;

        public ReconstructorController(Reconstructor reconstructor)
        {
            this.reconstructor = reconstructor;
        }

        // GET api/<controller>/5
        //[HttpGet("{text}")]
        //public async Task<ActionResult<ApiText>> GetApiText(string text)
        //{
        //    string reconstructedText = reconstructor.Reconstruct(text);

        //    return new ApiText() { Text = reconstructedText };
        //}

        // POST api/<controller>
        [HttpPost]
        public async Task<ActionResult<ApiText>> PostApiText([FromBody]ApiText apiText)
        {
            apiText.Text = reconstructor.Reconstruct(apiText.Text);

            return CreatedAtAction(null, apiText);
        }

    }
}
