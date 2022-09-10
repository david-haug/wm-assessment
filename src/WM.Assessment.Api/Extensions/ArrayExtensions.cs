using System.Collections.Generic;
using System.Linq;
using WM.Assessment.Application;

namespace WM.Assessment.Api.Extensions
{
    public static class ArrayExtensions
    {
        public static IEnumerable<SortItem> ToSortItems(this string[] sort, string defaultSortByName,
            bool isDescending = false)
        {
            var sortItems = new List<SortItem>();
            if (sort == null || !sort.Any())
                sortItems.Add(new SortItem {Name = defaultSortByName, IsDescending = isDescending});
            else
                foreach (var s in sort)
                {
                    var name = s;
                    var descending = false;
                    if (s.StartsWith("-"))
                    {
                        name = s.Substring(1, s.Length - 1);
                        @descending = true;
                    }

                    sortItems.Add(new SortItem {Name = name, IsDescending = @descending});
                }

            return sortItems;
        }
    }
}