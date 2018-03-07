using System;
using System.IO;

namespace hySite
{
    public class OnFileChangedRequest
    {
        public string FilePath { get; set; }
    }

    public class OnFileChangedResponse
    {

    }

    public class OnFileChangedHandler : IHandler<OnFileChangedRequest, OnFileChangedResponse>
    {
        private readonly IFileParserService _fileParserService;
        private readonly IBlogPostRepository _blogPostRepository;
        private AppDbContext _dbContext;

        public OnFileChangedHandler(IFileParserService fileParserService, IBlogPostRepository blogPostRepository, AppDbContext dbContext)
        {
            _fileParserService = fileParserService;
            _blogPostRepository = blogPostRepository;
            _dbContext = dbContext;
        }

        public OnFileChangedResponse Handle(OnFileChangedRequest request)
        {
            //@todo: this is called several times.
            var fileName = Path.GetFileNameWithoutExtension(request.FilePath).ToLower();
            BlogPost oldPost = _blogPostRepository.FindPostByFileName(fileName);
            var fileInfo = new FileInfo(request.FilePath);

            using(var reader = fileInfo.OpenText())
            {

                var blogPost = _fileParserService.ParseFile(fileName, reader);

                if(oldPost == null)
                {
                    _blogPostRepository.Add(blogPost);
                }
                else
                {
                    oldPost.Update(blogPost);
                }
                
                _dbContext.SaveChanges();
            }

            return new OnFileChangedResponse();
        }
    }
}
