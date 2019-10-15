using System;
using System.Linq;

namespace fantasy_hoops.Repositories
{
    public interface INewsRepository
    {

        IQueryable<Object> GetNews(int start, int count);

    }
}
