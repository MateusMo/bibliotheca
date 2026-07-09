using Bibliotheca.Application.Dtos.Book;
using Bibliotheca.Application.Dtos.Common;
using Bibliotheca.Application.Dtos.Feed;

namespace Bibliotheca.Application.Services;

public interface IFeedService
{
    Task<ResponseDto<PagedResultDto<FeedItemDto>>> SearchAsync(BookFilterDto filter, int pageNumber, int pageSize);
}