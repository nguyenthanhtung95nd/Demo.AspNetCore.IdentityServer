using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Demo.API.Controllers
{
    [Route("api/companies")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "KMS", 
            "NashTech", 
            "FPT", 
            "GCS", 
            "MTI", 
            "TMA"
        };

        private readonly ILogger<CompaniesController> _logger;

        public CompaniesController(ILogger<CompaniesController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetCompanies")]
        [Authorize]
        public IEnumerable<CompanyDto> GetCompanies()
        {
            return Enumerable.Range(1, 5).Select(index => new CompanyDto
            {
                Id = Random.Shared.Next(1, 55),
                Name = Summaries[Random.Shared.Next(Summaries.Length)],
                FullAddress = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}