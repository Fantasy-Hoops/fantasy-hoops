using System.Collections.Generic;
using fantasy_hoops.Dtos;

namespace fantasy_hoops.Repositories
{
    public interface INewsRepository
    {

        List<NewsDto> GetNews(int start, int count);

    }
}
