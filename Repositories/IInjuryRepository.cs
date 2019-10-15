using System;
using System.Linq;

namespace fantasy_hoops.Repositories
{
    public interface IInjuryRepository
    {

        IQueryable<Object> GetInjuries();

    }
}
