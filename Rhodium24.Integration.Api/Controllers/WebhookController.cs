using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Rhodium24.Integration.Api.Rhodium24;

namespace Rhodium24.Integration.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebhookController : ControllerBase
    {
        private readonly RhodiumHelper _rhodiumHelper;
        private readonly ILogger<WebhookController> _logger;

        public WebhookController(RhodiumHelper rhodiumHelper, ILogger<WebhookController> logger)
        {
            _rhodiumHelper = rhodiumHelper;
            _logger = logger;
        }

        [HttpOptions]
        public IActionResult Options()
        {
            var webhookRequestOrigin = HttpContext.Request.Headers["WebHook-Request-Origin"].FirstOrDefault();
            HttpContext.Response.Headers.Add("WebHook-Allowed-Rate", "*");
            HttpContext.Response.Headers.Add("WebHook-Allowed-Origin", webhookRequestOrigin);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            try
            {
                using StreamReader reader = new StreamReader(Request.Body);
                var json = await reader.ReadToEndAsync();
                _logger.LogInformation("Webhook data received: {Json}", json);

                var webhookMessage = JsonConvert.DeserializeObject<WebhookMessage>(json);

                if (webhookMessage == null)
                {
                    _logger.LogError("Webhook data could not be deserialized.");
                }
                else if (webhookMessage.Type == WebhookMessageType.DocumentCreated)
                {
                    var documentCreatedMessage = JsonConvert.DeserializeObject<WebhookMessage<DocumentCreatedData>>(json);

                    // download file content (base64)
                    var fileContent = await _rhodiumHelper.GetDocument(documentCreatedMessage.Data.PartyId, documentCreatedMessage.Data.ProjectId, documentCreatedMessage.Data.FileName);

                    // save file in temp dir
                    await System.IO.File.WriteAllBytesAsync(Path.Combine(Path.GetTempPath(), documentCreatedMessage.Data.FileName), fileContent);
                }
                else if (webhookMessage.Type == WebhookMessageType.ProjectStatusChanged)
                {
                    var projectStatusChangedMessage = JsonConvert.DeserializeObject<WebhookMessage<ProjectStatusChangedData>>(json);

                    // download project json
                    var project = await _rhodiumHelper.GetProject(projectStatusChangedMessage.Data.PartyId, projectStatusChangedMessage.Data.ProjectId);

                    // log
                    _logger.LogInformation("Retrieved project info from API status is {Status}", project.Status);
                }
                else
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing webhook data");
            }

            return Ok();
        }
    }
}
