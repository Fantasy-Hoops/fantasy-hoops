using System.Collections.Generic;
using System.Linq;

namespace fantasy_hoops.Helpers
{
    public static class CrossProductFunctions
    {
        public static IEnumerable<IEnumerable<T>> CrossProduct<T>(IDictionary<string, List<T>> source) => 
            source.Aggregate(
                (IEnumerable<IEnumerable<T>>) new[] { Enumerable.Empty<T>() },
                (acc, src) => src.Value.SelectMany(x => acc.Select(a => a.Concat(new[] {x}))));
    }
}