using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers
{
    [ApiController]
    [Route("api")]
    public class AggregateController : ControllerBase
    {
        /// <summary>
        /// Bir organizasyonu ve bu organizasyona ait ürünleri birleştirilmiş olarak getirir
        /// </summary>
        /// <param name="organizationId">Organizasyon ID</param>
        /// <returns>Organizasyon ve ürünleri birleştirilmiş veri</returns>
        [HttpGet("organization-with-products/{organizationId}")]
        [ProducesResponseType(typeof(OrganizationWithProductsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetOrganizationWithProducts(int organizationId)
        {
            // Bu method sadece Swagger dokümantasyonu için var
            // Gerçek çağrılar Ocelot tarafından yönlendirilir
            return Ok();
        }
    }
    // Swagger dokümantasyonu için model sınıfları
    public class OrganizationWithProductsResponse
    {
        public OrganizationDto Organization { get; set; }
        public List<ProductDto> Products { get; set; }
    }

    public class OrganizationDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ActiveStatus { get; set; }
    }

    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int OrganizationId { get; set; }
    }
}
