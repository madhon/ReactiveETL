using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReactiveETL.Operations
{
    /// <summary>
    /// Operation to group values
    /// </summary>
    public class GroupByOperation : AbstractOperation
    {
        private string[] _columns;

        private List<Row> groups = new List<Row>();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="columns"></param>
        public GroupByOperation(params string[] columns)
        {
            _columns = columns;
        }

        /// <summary>
        /// Method called by OnNext to dispatch the new value to the observers of the operation
        /// </summary>
        /// <param name="value">value to dispatch</param>
        protected override void Dispatch(Row value)
        {
            var group = groups.Find(r => this.Match(value, r));
            if (group == null)
            {
                group = GetNewGroup(value);                
            }

            var lst = (List<Row>)group[Constants.GroupListName];
            lst.Add(value);
        }

        private Row GetNewGroup(Row currentRow)
        {
            var res = new Row();
            res[Constants.GroupListName] = new List<Row>();
            foreach (var column in _columns)
            {
                res[column] = currentRow[column];                
            }
            groups.Add(res);
            return res;
        }

        private bool Match(Row currentRow, Row groupRow)
        {
            foreach (var column in _columns)
            {
                if (!groupRow[column].Equals(currentRow[column])) return false;
            }

            return true;
        }

        /// <summary>
        /// Notifies the observers of the end of the sequence.
        /// </summary>
        public override void OnCompleted()
        {
            foreach (var eltgroup in groups)
            {
                base.Dispatch(eltgroup);                
            }
            base.OnCompleted();
        }
    }
}
