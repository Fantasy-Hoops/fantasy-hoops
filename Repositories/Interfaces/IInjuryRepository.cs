using System;
using System.Linq;

namespace fantasy_hoops.Repositories.Interfaces
{
    public interface IInjuryRepository
    {

        IQueryable<Object> GetInjuries();

    }
}
