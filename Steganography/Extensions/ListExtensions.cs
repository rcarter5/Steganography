using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Steganography.Extensions
{ /// <summary>
  /// Preforms tasks on list
  /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// To the populate list with value.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="numToAdd">The number to add.</param>
        /// <returns></returns>
        public static ObservableCollection<string> ToPopulateListWithValue(this ObservableCollection<string> collection, int numToAdd)
        {
            for (int i = 1; i < numToAdd + 1; i++)
            {
                collection.Add(i.ToString());
            }

            return collection;
        }
    }
}
