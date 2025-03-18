using Newtonsoft.Json;
using Ocelot.Middleware;
using Ocelot.Multiplexer;
using System.Net;
using System.Text;

namespace ApiGateway.Aggregators
{
    public class OrganizationWithProductsAggregator : IDefinedAggregator
    {
        public async Task<DownstreamResponse> Aggregate(List<HttpContext> responses)
        {
            // Gelen yanıtları kontrol etmek için loglama yapın
            Console.WriteLine($"Aggregate method called with {responses.Count} responses");

            var organizationResponse = responses.FirstOrDefault(r => r.Items.DownstreamRoute().Key == "organization-data");
            var productsResponse = responses.FirstOrDefault(r => r.Items.DownstreamRoute().Key == "products-data");

            // Her yanıtın durumunu loglayın
            if (organizationResponse != null)
            {
                Console.WriteLine($"Organization response status: {organizationResponse.Items.DownstreamResponse().StatusCode}");
            }
            else
            {
                Console.WriteLine("Organization response is null");
            }

            if (productsResponse != null)
            {
                Console.WriteLine($"Products response status: {productsResponse.Items.DownstreamResponse().StatusCode}");
            }
            else
            {
                Console.WriteLine("Products response is null");
            }

            // Her iki yanıt da başarılı değilse bir hata yanıtı döndürün
            if (organizationResponse?.Items.DownstreamResponse().StatusCode != HttpStatusCode.OK ||
                productsResponse?.Items.DownstreamResponse().StatusCode != HttpStatusCode.OK)
            {
                var errorMessage = "Error occurred while retrieving data from one or more services";

                var errorContent = new StringContent(
                    JsonConvert.SerializeObject(new { error = errorMessage }),
                    Encoding.UTF8,
                    "application/json");

                var errorHeaders = new List<KeyValuePair<string, IEnumerable<string>>>
                {
                    new KeyValuePair<string, IEnumerable<string>>("Content-Type", new[] { "application/json" })
                };

                return new DownstreamResponse(
                    content: errorContent,
                    statusCode: HttpStatusCode.BadRequest,
                    headers: errorHeaders,
                    reasonPhrase: errorMessage);
            }

            try
            {
                // Organization verilerini al
                var orgContent = await organizationResponse.Items.DownstreamResponse().Content.ReadAsStringAsync();
                var organization = JsonConvert.DeserializeObject<dynamic>(orgContent);

                // Products verilerini al
                var productsContent = await productsResponse.Items.DownstreamResponse().Content.ReadAsStringAsync();
                var products = JsonConvert.DeserializeObject<dynamic>(productsContent);

                // Verileri birleştir
                var aggregatedResult = new
                {
                    Organization = organization,
                    Products = products
                };

                var jsonResult = JsonConvert.SerializeObject(aggregatedResult);
                Console.WriteLine($"Aggregated result: {jsonResult}");

                var stringContent = new StringContent(jsonResult, Encoding.UTF8, "application/json");
                var headers = new List<KeyValuePair<string, IEnumerable<string>>>
                {
                    new KeyValuePair<string, IEnumerable<string>>("Content-Type", new[] { "application/json" })
                };

                return new DownstreamResponse(
                    content: stringContent,
                    statusCode: HttpStatusCode.OK,
                    headers: headers,
                    reasonPhrase: "OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in aggregator: {ex.Message}");

                var errorContent = new StringContent(
                    JsonConvert.SerializeObject(new { error = ex.Message }),
                    Encoding.UTF8,
                    "application/json");

                var errorHeaders = new List<KeyValuePair<string, IEnumerable<string>>>
                {
                    new KeyValuePair<string, IEnumerable<string>>("Content-Type", new[] { "application/json" })
                };

                return new DownstreamResponse(
                    content: errorContent,
                    statusCode: HttpStatusCode.InternalServerError,
                    headers: errorHeaders,
                    reasonPhrase: "Internal Server Error");
            }
        }
    }
}