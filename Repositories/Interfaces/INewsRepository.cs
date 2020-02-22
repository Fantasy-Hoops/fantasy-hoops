using System;
using System.Collections.Generic;
using fantasy_hoops.Dtos;

namespace fantasy_hoops.Repositories.Interfaces
{
    public interface INewsRepository
    {

        Dictionary<String, List<NewsDto>> GetNews(int start, int count);
        List<NewsDto> GetPreviews(int start, int count);
        List<NewsDto> GetRecaps(int start, int count);

    }
}
