using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace BasicTriggerHTTP
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;

        public Function1(ILogger<Function1> logger)
        {
            _logger = logger;
        }

        [Function("Function1")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            // If name is not in the query string, check the body
            if (string.IsNullOrEmpty(name) && req.Body != null)
            {
                using var reader = new StreamReader(req.Body);
                var requestBody = await reader.ReadToEndAsync();

                if (!string.IsNullOrWhiteSpace(requestBody))
                {
                    var data = JsonSerializer.Deserialize<Dictionary<string, string>>(requestBody);

                    if (data != null && data.TryGetValue("name", out string bodyName))
                    {
                        name = bodyName;
                    }
                }
            }

            if (string.IsNullOrEmpty(name))
            {
                return new BadRequestObjectResult("Please provide a name in the query string or request body.");
            }

            return new OkObjectResult($"Hello {name}, welcome to Azure Functions!");
        }

    }
    }
