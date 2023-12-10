using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using shr.API.Exceptions;
using shr.API.Models;
using shr.API.Models.Share;
using shr.API.Services;

namespace shr.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShareController : ControllerBase
    {
        public const string SECRET_HEADER_NAME = "X-Shr-Secret";

        private readonly ILogger<ShareController> _logger;
        private readonly IShareService _shareService;
        private readonly IMimeMappingService _mimeMappingService;

        public ShareController(ILogger<ShareController> logger, IShareService shareService, IMimeMappingService mimeMappingService)
        {
            _logger = logger;
            _shareService = shareService;
            _mimeMappingService = mimeMappingService;
        }

        [HttpPut("{name}", Name = nameof(PutShare))]
        [ProducesResponseType(typeof(ShareGet), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Prefix)]
        public async Task<ShareGet> PutShare([FromRoute] string name, [FromQuery] bool secure)
        {
            var secret = _shareService.GenSecret();

            var share = new Share
            {
                Id = await _shareService.GenIdAsync(secure),
                Name = name,
                Secret = secret,
                ContentType = _mimeMappingService.Map(name),
                CreationDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow
            };

            var result = await _shareService.SaveAsync(share);
            await _shareService.UploadAsync(share.Id, Request.Body);

            Response.Headers.Add(SECRET_HEADER_NAME, secret);

            _logger.LogInformation(Request.Path, Request.Method, $"Created as '{share.Id}'", DateTime.UtcNow.ToLongTimeString());

            return new ShareGet(result);
        }

        [HttpGet("{id}", Name = nameof(GetShare))]
        [ProducesResponseType(typeof(ShareGet), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Prefix)]
        public async Task<ShareGet> GetShare([FromRoute] string id)
        {
            var result = await GetShareAsync(id);

            _logger.LogInformation(Request.Path, Request.Method, $"{id} was fetched", DateTime.UtcNow.ToLongTimeString());

            return new ShareGet(result);
        }

        [HttpPatch("{id}", Name = nameof(PatchShare))]
        [ProducesResponseType(typeof(ShareGet), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status401Unauthorized)]
        [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Prefix)]
        public async Task<ShareGet> PatchShare([FromRoute] string id, [FromBody] SharePatch patch, [FromHeader(Name = SECRET_HEADER_NAME), Required] string secret)
        {
            if (!ModelState.IsValid)
            {
                throw new Exceptions.ValidationException(ModelState);
            }

            var share = await GetShareAsync(id, secret);

            share.Id = await _shareService.GenIdAsync(patch.Secure, id);
            share.Name = patch.Name;
            share.ContentType = _mimeMappingService.Map(patch.Name);
            share.UpdateDate = DateTime.UtcNow;

            if (id != share.Id)
            {
                await _shareService.MoveAsync(id, share.Id);
                await _shareService.DeleteAsync(id);
            }

            var result = await _shareService.SaveAsync(share);

            _logger.LogInformation(Request.Path, Request.Method, $"{id} was patched to '{share.Id}' as '{share.Name}'", DateTime.UtcNow.ToLongTimeString());

            return new ShareGet(result);
        }

        [HttpDelete("{id}", Name = nameof(DeleteShare))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status401Unauthorized)]
        [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Prefix)]
        public async Task<NoContentResult> DeleteShare([FromRoute] string id, [FromHeader(Name = SECRET_HEADER_NAME), Required] string secret)
        {
            if (!ModelState.IsValid)
            {
                throw new Exceptions.ValidationException(ModelState);
            }

            await GetShareAsync(id, secret);

            await _shareService.DeleteAsync(id);

            _logger.LogInformation(Request.Path, Request.Method, $"'{id}' was deleted", DateTime.UtcNow.ToLongTimeString());

            return NoContent();
        }

        [HttpGet("{id}/content", Name = nameof(GetContent))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Prefix)]
        public async Task<FileStreamResult> GetContent([FromRoute] string id)
        {
            var share = await GetShareAsync(id);

            var result = await _shareService.DownloadAsync(id);

            _logger.LogInformation(Request.Path, Request.Method, $"'{share.Id}' content was fetched", DateTime.UtcNow.ToLongTimeString());

            return File(result, share.ContentType, share.Name);
        }

        [HttpPut("{id}/content", Name = nameof(PutContent))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status401Unauthorized)]
        [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Prefix)]
        public async Task<NoContentResult> PutContent([FromRoute] string id, [FromHeader(Name = SECRET_HEADER_NAME), Required] string secret)
        {
            if (!ModelState.IsValid)
            {
                throw new Exceptions.ValidationException(ModelState);
            }

            await GetShareAsync(id, secret);

            await _shareService.UploadAsync(id, Request.Body);

            _logger.LogInformation(Request.Path, Request.Method, $"{id} content was changed", DateTime.UtcNow.ToLongTimeString());

            return NoContent();
        }

        private async Task<Share> GetShareAsync(string id, string? secret = null)
        {
            var result = await _shareService.FindAsync(id) ?? throw new NotFoundException();

            if (secret is not null && result.Secret != secret)
            {
                throw new UnauthorizedException();
            }

            return result;
        }
    }
}
