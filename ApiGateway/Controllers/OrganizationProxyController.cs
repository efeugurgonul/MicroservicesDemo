using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers
{
    [ApiController]
    [Route("api/organizations")]
    public class OrganizationProxyController : ControllerBase
    {
        /// <summary>
        /// Tüm organizasyonları listeler
        /// </summary>
        /// <param name="activeStatus">Aktiflik durumu filtresi (isteğe bağlı)</param>
        /// <returns>Organizasyonlar listesi</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<OrganizationDto>), StatusCodes.Status200OK)]
        public IActionResult GetAllOrganizations([FromQuery] int? activeStatus)
        {
            // Bu method sadece Swagger dokümantasyonu için var
            return Ok();
        }

        /// <summary>
        /// Belirtilen ID'ye sahip organizasyonu getirir
        /// </summary>
        /// <param name="organizationId">Organizasyon ID</param>
        /// <returns>Organizasyon</returns>
        [HttpGet("{organizationId}")]
        [ProducesResponseType(typeof(OrganizationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetOrganizationById(int organizationId)
        {
            // Bu method sadece Swagger dokümantasyonu için var
            return Ok();
        }

        /// <summary>
        /// Yeni bir organizasyon oluşturur
        /// </summary>
        /// <param name="organization">Organizasyon bilgileri</param>
        /// <returns>Oluşturulan organizasyonun ID'si</returns>
        [HttpPost]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        public IActionResult CreateOrganization([FromBody] OrganizationDto organization)
        {
            // Bu method sadece Swagger dokümantasyonu için var
            return CreatedAtAction(nameof(GetOrganizationById), new { organizationId = 1 }, 1);
        }

        /// <summary>
        /// Belirtilen organizasyonu günceller
        /// </summary>
        /// <param name="organizationId">Organizasyon ID</param>
        /// <param name="organization">Güncellenmiş organizasyon bilgileri</param>
        /// <returns>İşlem sonucu</returns>
        [HttpPut("{organizationId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateOrganization(int organizationId, [FromBody] OrganizationDto organization)
        {
            // Bu method sadece Swagger dokümantasyonu için var
            return NoContent();
        }

        /// <summary>
        /// Belirtilen organizasyonu siler
        /// </summary>
        /// <param name="organizationId">Organizasyon ID</param>
        /// <returns>İşlem sonucu</returns>
        [HttpDelete("{organizationId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteOrganization(int organizationId)
        {
            // Bu method sadece Swagger dokümantasyonu için var
            return NoContent();
        }
    }
}
