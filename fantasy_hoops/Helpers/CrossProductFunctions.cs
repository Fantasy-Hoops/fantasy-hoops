using System.Collections.Generic;
using System.Linq;

namespace fantasy_hoops.Helpers
{
    public static class CrossProductFunctions
    {
        public static IEnumerable<List<T>> CrossProduct<T>(IDictionary<string, List<T>> possibleValueArrays)
        {
            var maxIndices = possibleValueArrays.Select(keyValuePair => keyValuePair.Value.Count - 1).ToList();

            foreach (var indexList in IndexCrossProduct(maxIndices))
            {
                yield return possibleValueArrays.Select((keyValuePair, i) => keyValuePair.Value[indexList[i]]).ToList();
            }
        }
        
        private static IEnumerable<List<int>> IndexCrossProduct(IReadOnlyList<int> maxIndices)
        {
            if (maxIndices == null || maxIndices.Count == 0)
            {
                yield break;
            }

            var lastIndex = maxIndices.Count - 1;
            List<int> currentIndices = Enumerable.Repeat(0, maxIndices.Count).ToList();
            
            for (;; currentIndices[lastIndex]++)
            {
                for (int incrementIndex = lastIndex; ; incrementIndex--)
                {
                    if (currentIndices[incrementIndex] <= maxIndices[incrementIndex])
                    {
                        yield return currentIndices.Select(i => i).ToList();
                        break;
                    }

                    if (incrementIndex == 0)
                        yield break;

                    currentIndices[incrementIndex] = 0;
                    currentIndices[incrementIndex - 1]++;
                }
            }
        }
    }
}