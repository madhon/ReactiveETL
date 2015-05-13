using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReactiveETL
{
    /// <summary>
    /// Constants used by reactive etl
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Name of the column containing the list of child rows when using GroupBy Operation
        /// </summary>
        public const string GroupListName = "EtlGroupValues";

        /// <summary>
        /// Name of the column containing the parent group when using DispatchGroup
        /// </summary>
        public const string GroupParentName = "EtlParentGroup";
    }
}
