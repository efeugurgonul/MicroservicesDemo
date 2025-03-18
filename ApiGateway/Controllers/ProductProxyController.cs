using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductProxyController : ControllerBase
    {
        /// <summary>
        /// Tüm ürünleri listeler
        /// </summary>
        /// <param name="organizationId">Organizasyon ID filtresi (isteğe bağlı)</param>
        /// <returns>Ürünler listesi</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<ProductDto>), StatusCodes.Status200OK)]
        public IActionResult GetAllProducts([FromQuery] int? organizationId)
        {
            // Bu method sadece Swagger dokümantasyonu için var
            return Ok();
        }

        /// <summary>
        /// Belirtilen ID'ye sahip ürünü getirir
        /// </summary>
        /// <param name="productId">Ürün ID</param>
        /// <returns>Ürün</returns>
        [HttpGet("{productId}")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetProductById(int productId)
        {
            // Bu method sadece Swagger dokümantasyonu için var
            return Ok();
        }

        /// <summary>
        /// Yeni bir ürün oluşturur
        /// </summary>
        /// <param name="product">Ürün bilgileri</param>
        /// <returns>Oluşturulan ürün</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        public IActionResult CreateProduct([FromBody] ProductDto product)
        {
            // Bu method sadece Swagger dokümantasyonu için var
            return Ok();
        }

        /// <summary>
        /// Belirtilen ürünü günceller
        /// </summary>
        /// <param name="productId">Ürün ID</param>
        /// <param name="product">Güncellenmiş ürün bilgileri</param>
        /// <returns>İşlem sonucu</returns>
        [HttpPut("{productId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateProduct(int productId, [FromBody] ProductDto product)
        {
            // Bu method sadece Swagger dokümantasyonu için var
            return NoContent();
        }

        /// <summary>
        /// Belirtilen ürünü siler
        /// </summary>
        /// <param name="productId">Ürün ID</param>
        /// <returns>İşlem sonucu</returns>
        [HttpDelete("{productId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteProduct(int productId)
        {
            // Bu method sadece Swagger dokümantasyonu için var
            return NoContent();
        }
    }
}