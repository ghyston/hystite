using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace hySite 
{
    public class GitController : Controller 
    {
        private readonly IGitRepository _gitRepository;
        private readonly IFileParserService _fileParserService;
        private readonly IRssFeedService _rssFeedService;
        private readonly IConfiguration _configuration;

        public GitController(IGitRepository gitRepository, IFileParserService fileParserService, IRssFeedService rssFeedService, IConfiguration configuration)
        {
            _gitRepository = gitRepository;
            _fileParserService = fileParserService;
            _rssFeedService = rssFeedService;
            _configuration = configuration;
        }

        [Route("update")]
        public async Task<IActionResult> UpdatePostsAsync()
        {
            var signature = Request.Headers["X-Hub-Signature"];
            
            using (var reader = new StreamReader(Request.Body))
            {
                var payload = await reader.ReadToEndAsync();
                if (_gitRepository.IsSecretValid(signature, payload))
                {
                    _gitRepository.Pull();
                    _fileParserService.ParseExistingFiles();
                    _rssFeedService.CreateRssFeed();
                    return Ok();
                }
            }

            return Unauthorized();
        }
    }
}

