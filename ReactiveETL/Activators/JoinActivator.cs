using System;
using System.Collections.Generic;

namespace ReactiveETL.Activators
{
    /// <summary>
    /// Activator for join operations
    /// </summary>
    /// <typeparam name="T">Type of data in the join</typeparam>
    public class JoinActivator<T> : IEnumerable<T>
    {
        /// <summary>
        /// List of data to join
        /// </summary>
        public IEnumerable<T> List { get; set; }

        /// <summary>
        /// Callback method for checking if the element is matching
        /// </summary>
        public Func<Row, T, bool> CheckMatch { get; set; }

        /// <summary>
        /// Callback method for processing the row with found element
        /// </summary>
        public Func<Row, T, Row> ProcessRow { get; set; }

        /// <summary>
        /// Get the list of elements that matches the condition
        /// </summary>
        /// <param name="rowVal">the row for the left part of the join</param>
        /// <returns>an enumeration of right join elements matching condition</returns>
        public IEnumerable<T> GetMatches(Row rowVal)
        {
            foreach (var elt in this)
            {
                if (CheckMatch(rowVal, elt))
                    yield return elt;
            }
        }

        #region IEnumerable<T> Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public virtual IEnumerator<T> GetEnumerator()
        {
            return List.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return List.GetEnumerator();
        }

        #endregion
    }
}