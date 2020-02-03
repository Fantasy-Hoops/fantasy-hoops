using System.Collections.Generic;
using fantasy_hoops.Dtos;

namespace fantasy_hoops.Repositories.Interfaces
{
    public interface INewsRepository
    {

        List<NewsDto> GetNews(int start, int count);

    }
}
