using System;
using System.Collections.Generic;
using ReactiveETL.Activators;
using ReactiveETL.Operations;

namespace ReactiveETL
{
    /// <summary>
    /// Extension methods for join operations
    /// </summary>
    public static class JoinExtensions
    {
        /// <summary>
        /// Join an enumeration of elements to the pipeline
        /// </summary>
        /// <typeparam name="T">type of elements to join</typeparam>
        /// <param name="observed">observed operation</param>
        /// <param name="activator">join parameters</param>
        /// <returns>resulting operation</returns>
        public static JoinOperation<T> Join<T>(this IObservableOperation observed, JoinActivator<T> activator)
        {
            JoinOperation<T> op = new JoinOperation<T>(activator);
            observed.Subscribe(op);
            return op;
        }
        
        /// <summary>
        /// Join an enumeration of elements to the pipeline
        /// </summary>
        /// <typeparam name="T">type of elements to join</typeparam>
        /// <param name="observed">observed operation</param>
        /// <param name="list">enumeration of elements to join</param>
        /// <param name="checkMatch">callback method to check if an element is matching the row</param>
        /// <param name="processrow">callback method called to process the row</param>
        /// <returns>resulting operation</returns>
        public static JoinOperation<T> Join<T>(this IObservableOperation observed, IEnumerable<T> list, Func<Row, T, bool> checkMatch, Func<Row, T, Row> processrow)
        {
            JoinActivator<T> activator = new JoinActivator<T>();
            activator.List = list;
            activator.CheckMatch = checkMatch;
            activator.ProcessRow = processrow;

            return observed.Join(activator);
        }

        /// <summary>
        /// Join an enumeration of elements to the pipeline
        /// </summary>
        /// <param name="observed">observed operation</param>
        /// <param name="rightOp">operation to join</param>
        /// <param name="checkMatch">callback method to check if an element is matching the row</param>
        /// <param name="processrow">callback method called to process the row</param>
        /// <returns>resulting operation</returns>
        public static JoinOperation<Row> Join(this IObservableOperation observed, IObservableOperation rightOp, Func<Row, Row, bool> checkMatch, Func<Row, Row, Row> processrow)
        {
            OperationJoinActivator activator = new OperationJoinActivator();
            activator.Operation = rightOp;
            activator.CheckMatch = checkMatch;
            activator.ProcessRow = processrow;

            return observed.Join(activator);
        }

        /// <summary>
        /// Join an enumeration of elements to the pipeline
        /// </summary>
        /// <param name="observed">observed operation</param>
        /// <param name="rightOp">operation to join</param>
        /// <param name="field">Name of the field used for the join</param>
        /// <param name="processrow">callback method called to process the row</param>
        /// <returns>resulting operation</returns>
        public static JoinOperation<Row> FullJoin(this IObservableOperation observed, IObservableOperation rightOp, string field, Func<Row, Row, Row> processrow)
        {
            return observed.Join(
                rightOp,
                new RowJoinHelper(field).FullJoinMatch,
                processrow);
        }

        /// <summary>
        /// Join an enumeration of elements to the pipeline
        /// </summary>
        /// <param name="observed">observed operation</param>
        /// <param name="rightOp">operation to join</param>
        /// <param name="field">Name of the field used for the join</param>
        /// <param name="processrow">callback method called to process the row</param>
        /// <returns>resulting operation</returns>
        public static JoinOperation<Row> FullJoin(this IObservableOperation observed, IEnumerable<Row> rightOp, string field, Func<Row, Row, Row> processrow)
        {
            return observed.Join(
                rightOp,
                new RowJoinHelper(field).FullJoinMatch,
                processrow);
        }

        /// <summary>
        /// Join an enumeration of elements to the pipeline
        /// </summary>
        /// <param name="observed">observed operation</param>
        /// <param name="rightOp">operation to join</param>
        /// <param name="leftField">Name of the field used for the join</param>
        /// <param name="rightField">Name of the field used for the join</param>
        /// <param name="processrow">callback method called to process the row</param>
        /// <returns>resulting operation</returns>
        public static JoinOperation<Row> FullJoin(this IObservableOperation observed, IEnumerable<Row> rightOp, string leftField, string rightField, Func<Row, Row, Row> processrow)
        {
            return observed.Join(
                rightOp,
                new RowJoinHelper(leftField, rightField).FullJoinMatch,
                processrow);
        }

        /// <summary>
        /// Join an enumeration of elements to the pipeline
        /// </summary>
        /// <param name="observed">observed operation</param>
        /// <param name="rightOp">operation to join</param>
        /// <param name="leftField">Name of the field used for the join</param>
        /// <param name="rightField">Name of the field used for the join</param>
        /// <param name="processrow">callback method called to process the row</param>
        /// <returns>resulting operation</returns>
        public static JoinOperation<Row> FullJoin(this IObservableOperation observed, IObservableOperation rightOp, string leftField, string rightField, Func<Row, Row, Row> processrow)
        {
            return observed.Join(
                rightOp,
                new RowJoinHelper(leftField, rightField).FullJoinMatch,
                processrow);
        }

        /// <summary>
        /// Join an enumeration of elements to the pipeline
        /// </summary>
        /// <param name="observed">observed operation</param>
        /// <param name="rightOp">operation to join</param>
        /// <param name="field">Name of the field used for the join</param>
        /// <returns>resulting operation</returns>
        public static JoinOperation<Row> FullJoin(this IObservableOperation observed, IObservableOperation rightOp, string field)
        {
            return observed.FullJoin(rightOp, field, RowJoinHelper.MergeRows);
        }

        /// <summary>
        /// Join an enumeration of elements to the pipeline
        /// </summary>
        /// <param name="observed">observed operation</param>
        /// <param name="rightOp">operation to join</param>
        /// <param name="field">Name of the field used for the join</param>
        /// <returns>resulting operation</returns>
        public static JoinOperation<Row> FullJoin(this IObservableOperation observed, IEnumerable<Row> rightOp, string field)
        {
            return observed.FullJoin(rightOp, field, RowJoinHelper.MergeRows);
        }

        /// <summary>
        /// Join an enumeration of elements to the pipeline
        /// </summary>
        /// <param name="observed">observed operation</param>
        /// <param name="rightOp">operation to join</param>
        /// <param name="leftField">Name of the field used for the join</param>
        /// <param name="rightField">Name of the field used for the join</param>
        /// <returns>resulting operation</returns>
        public static JoinOperation<Row> FullJoin(this IObservableOperation observed, IObservableOperation rightOp, string leftField, string rightField)
        {
            return observed.FullJoin(rightOp, leftField, rightField, RowJoinHelper.MergeRows);
        }

        /// <summary>
        /// Join an enumeration of elements to the pipeline
        /// </summary>
        /// <param name="observed">observed operation</param>
        /// <param name="rightOp">operation to join</param>
        /// <param name="leftField">Name of the field used for the join</param>
        /// <param name="rightField">Name of the field used for the join</param>
        /// <returns>resulting operation</returns>
        public static JoinOperation<Row> FullJoin(this IObservableOperation observed, IEnumerable<Row> rightOp, string leftField, string rightField)
        {
            return observed.FullJoin(rightOp, leftField, rightField, RowJoinHelper.MergeRows);
        }

        /// <summary>
        /// Join an enumeration of elements to the pipeline
        /// </summary>
        /// <param name="observed">observed operation</param>
        /// <param name="rightOp">operation to join</param>
        /// <param name="field">Name of the field used for the join</param>
        /// <param name="processrow">callback method called to process the row</param>
        /// <returns>resulting operation</returns>
        public static JoinOperation<Row> InnerJoin(this IObservableOperation observed, IObservableOperation rightOp, string field, Func<Row, Row, Row> processrow)
        {
            return observed.Join(
                rightOp,
                new RowJoinHelper(field).InnerJoinMatch,
                processrow);
        }

        /// <summary>
        /// Join an enumeration of elements to the pipeline
        /// </summary>
        /// <param name="observed">observed operation</param>
        /// <param name="rightOp">operation to join</param>
        /// <param name="field">Name of the field used for the join</param>
        /// <param name="processrow">callback method called to process the row</param>
        /// <returns>resulting operation</returns>
        public static JoinOperation<Row> InnerJoin(this IObservableOperation observed, IEnumerable<Row> rightOp, string field, Func<Row, Row, Row> processrow)
        {
            return observed.Join(
                rightOp,
                new RowJoinHelper(field).InnerJoinMatch,
                processrow);
        }

        /// <summary>
        /// Join an enumeration of elements to the pipeline
        /// </summary>
        /// <param name="observed">observed operation</param>
        /// <param name="rightOp">operation to join</param>
        /// <param name="leftField">Name of the field used for the join</param>
        /// <param name="rightField">Name of the field used for the join</param>
        /// <param name="processrow">callback method called to process the row</param>
        /// <returns>resulting operation</returns>
        public static JoinOperation<Row> InnerJoin(this IObservableOperation observed, IObservableOperation rightOp, string leftField, string rightField, Func<Row, Row, Row> processrow)
        {
            return observed.Join(
                rightOp,
                new RowJoinHelper(leftField, rightField).InnerJoinMatch,
                processrow);
        }

        /// <summary>
        /// Join an enumeration of elements to the pipeline
        /// </summary>
        /// <param name="observed">observed operation</param>
        /// <param name="rightOp">operation to join</param>
        /// <param name="leftField">Name of the field used for the join</param>
        /// <param name="rightField">Name of the field used for the join</param>
        /// <param name="processrow">callback method called to process the row</param>
        /// <returns>resulting operation</returns>
        public static JoinOperation<Row> InnerJoin(this IObservableOperation observed, IEnumerable<Row> rightOp, string leftField, string rightField, Func<Row, Row, Row> processrow)
        {
            return observed.Join(
                rightOp,
                new RowJoinHelper(leftField, rightField).InnerJoinMatch,
                processrow);
        }

        /// <summary>
        /// Join an enumeration of elements to the pipeline
        /// </summary>
        /// <param name="observed">observed operation</param>
        /// <param name="rightOp">operation to join</param>
        /// <param name="field">Name of the field used for the join</param>
        /// <returns>resulting operation</returns>
        public static JoinOperation<Row> InnerJoin(this IObservableOperation observed, IObservableOperation rightOp, string field)
        {
            return observed.InnerJoin(rightOp, field, RowJoinHelper.MergeRows);
        }

        /// <summary>
        /// Join an enumeration of elements to the pipeline
        /// </summary>
        /// <param name="observed">observed operation</param>
        /// <param name="rightOp">operation to join</param>
        /// <param name="field">Name of the field used for the join</param>
        /// <returns>resulting operation</returns>
        public static JoinOperation<Row> InnerJoin(this IObservableOperation observed, IEnumerable<Row> rightOp, string field)
        {
            return observed.InnerJoin(rightOp, field, RowJoinHelper.MergeRows);
        }

        /// <summary>
        /// Join an enumeration of elements to the pipeline
        /// </summary>
        /// <param name="observed">observed operation</param>
        /// <param name="rightOp">operation to join</param>
        /// <param name="leftField">Name of the field used for the join</param>
        /// <param name="rightField">Name of the field used for the join</param>
        /// <returns>resulting operation</returns>
        public static JoinOperation<Row> InnerJoin(this IObservableOperation observed, IObservableOperation rightOp, string leftField, string rightField)
        {
            return observed.InnerJoin(rightOp, leftField, rightField, RowJoinHelper.MergeRows);
        }

        /// <summary>
        /// Join an enumeration of elements to the pipeline
        /// </summary>
        /// <param name="observed">observed operation</param>
        /// <param name="rightOp">operation to join</param>
        /// <param name="leftField">Name of the field used for the join</param>
        /// <param name="rightField">Name of the field used for the join</param>
        /// <returns>resulting operation</returns>
        public static JoinOperation<Row> InnerJoin(this IObservableOperation observed, IEnumerable<Row> rightOp, string leftField, string rightField)
        {
            return observed.InnerJoin(rightOp, leftField, rightField, RowJoinHelper.MergeRows);
        }

        /// <summary>
        /// Join an enumeration of elements to the pipeline
        /// </summary>
        /// <param name="observed">observed operation</param>
        /// <param name="rightOp">operation to join</param>
        /// <param name="field">Name of the field used for the join</param>
        /// <param name="processrow">callback method called to process the row</param>
        /// <returns>resulting operation</returns>
        public static JoinOperation<Row> LeftJoin(this IObservableOperation observed, IObservableOperation rightOp, string field, Func<Row, Row, Row> processrow)
        {
            return observed.Join(
                rightOp,
                new RowJoinHelper(field).LeftJoinMatch,
                processrow);
        }

        /// <summary>
        /// Join an enumeration of elements to the pipeline
        /// </summary>
        /// <param name="observed">observed operation</param>
        /// <param name="rightOp">operation to join</param>
        /// <param name="field">Name of the field used for the join</param>
        /// <param name="processrow">callback method called to process the row</param>
        /// <returns>resulting operation</returns>
        public static JoinOperation<Row> LeftJoin(this IObservableOperation observed, IEnumerable<Row> rightOp, string field, Func<Row, Row, Row> processrow)
        {
            return observed.Join(
                rightOp,
                new RowJoinHelper(field).LeftJoinMatch,
                processrow);
        }

        /// <summary>
        /// Join an enumeration of elements to the pipeline
        /// </summary>
        /// <param name="observed">observed operation</param>
        /// <param name="rightOp">operation to join</param>
        /// <param name="leftField">Name of the field used for the join</param>
        /// <param name="rightField">Name of the field used for the join</param>
        /// <param name="processrow">callback method called to process the row</param>
        /// <returns>resulting operation</returns>
        public static JoinOperation<Row> LeftJoin(this IObservableOperation observed, IEnumerable<Row> rightOp, string leftField, string rightField, Func<Row, Row, Row> processrow)
        {
            return observed.Join(
                rightOp,
                new RowJoinHelper(leftField, rightField).LeftJoinMatch,
                processrow);
        }

        /// <summary>
        /// Join an enumeration of elements to the pipeline
        /// </summary>
        /// <param name="observed">observed operation</param>
        /// <param name="rightOp">operation to join</param>
        /// <param name="leftField">Name of the field used for the join</param>
        /// <param name="rightField">Name of the field used for the join</param>
        /// <param name="processrow">callback method called to process the row</param>
        /// <returns>resulting operation</returns>
        public static JoinOperation<Row> LeftJoin(this IObservableOperation observed, IObservableOperation rightOp, string leftField, string rightField, Func<Row, Row, Row> processrow)
        {
            return observed.Join(
                rightOp,
                new RowJoinHelper(leftField, rightField).LeftJoinMatch,
                processrow);
        }

        /// <summary>
        /// Join an enumeration of elements to the pipeline
        /// </summary>
        /// <param name="observed">observed operation</param>
        /// <param name="rightOp">operation to join</param>
        /// <param name="field">Name of the field used for the join</param>
        /// <returns>resulting operation</returns>
        public static JoinOperation<Row> LeftJoin(this IObservableOperation observed, IObservableOperation rightOp, string field)
        {
            return observed.LeftJoin(rightOp, field, RowJoinHelper.MergeRows);
        }

        /// <summary>
        /// Join an enumeration of elements to the pipeline
        /// </summary>
        /// <param name="observed">observed operation</param>
        /// <param name="rightOp">operation to join</param>
        /// <param name="field">Name of the field used for the join</param>
        /// <returns>resulting operation</returns>
        public static JoinOperation<Row> LeftJoin(this IObservableOperation observed, IEnumerable<Row> rightOp, string field)
        {
            return observed.LeftJoin(rightOp, field, RowJoinHelper.MergeRows);
        }

        /// <summary>
        /// Join an enumeration of elements to the pipeline
        /// </summary>
        /// <param name="observed">observed operation</param>
        /// <param name="rightOp">operation to join</param>
        /// <param name="leftField">Name of the field used for the join</param>
        /// <param name="rightField">Name of the field used for the join</param>
        /// <returns>resulting operation</returns>
        public static JoinOperation<Row> LeftJoin(this IObservableOperation observed, IObservableOperation rightOp, string leftField, string rightField)
        {
            return observed.LeftJoin(rightOp, leftField, rightField, RowJoinHelper.MergeRows);
        }

        /// <summary>
        /// Join an enumeration of elements to the pipeline
        /// </summary>
        /// <param name="observed">observed operation</param>
        /// <param name="rightOp">operation to join</param>
        /// <param name="leftField">Name of the field used for the join</param>
        /// <param name="rightField">Name of the field used for the join</param>
        /// <returns>resulting operation</returns>
        public static JoinOperation<Row> LeftJoin(this IObservableOperation observed, IEnumerable<Row> rightOp, string leftField, string rightField)
        {
            return observed.LeftJoin(rightOp, leftField, rightField, RowJoinHelper.MergeRows);
        }

        /// <summary>
        /// Join an enumeration of elements to the pipeline
        /// </summary>
        /// <param name="observed">observed operation</param>
        /// <param name="rightOp">operation to join</param>
        /// <param name="field">Name of the field used for the join</param>
        /// <param name="processrow">callback method called to process the row</param>
        /// <returns>resulting operation</returns>
        public static JoinOperation<Row> RightJoin(this IObservableOperation observed, IObservableOperation rightOp, string field, Func<Row, Row, Row> processrow)
        {
            return observed.Join(
                rightOp,
                new RowJoinHelper(field).RightJoinMatch,
                processrow);
        }

        /// <summary>
        /// Join an enumeration of elements to the pipeline
        /// </summary>
        /// <param name="observed">observed operation</param>
        /// <param name="rightOp">operation to join</param>
        /// <param name="field">Name of the field used for the join</param>
        /// <param name="processrow">callback method called to process the row</param>
        /// <returns>resulting operation</returns>
        public static JoinOperation<Row> RightJoin(this IObservableOperation observed, IEnumerable<Row> rightOp, string field, Func<Row, Row, Row> processrow)
        {
            return observed.Join(
                rightOp,
                new RowJoinHelper(field).RightJoinMatch,
                processrow);
        }

        /// <summary>
        /// Join an enumeration of elements to the pipeline
        /// </summary>
        /// <param name="observed">observed operation</param>
        /// <param name="rightOp">operation to join</param>
        /// <param name="leftField">Name of the field used for the join</param>
        /// <param name="rightField">Name of the field used for the join</param>
        /// <param name="processrow">callback method called to process the row</param>
        /// <returns>resulting operation</returns>
        public static JoinOperation<Row> RightJoin(this IObservableOperation observed, IObservableOperation rightOp, string leftField, string rightField, Func<Row, Row, Row> processrow)
        {
            return observed.Join(
                rightOp,
                new RowJoinHelper(leftField, rightField).RightJoinMatch,
                processrow);
        }

        /// <summary>
        /// Join an enumeration of elements to the pipeline
        /// </summary>
        /// <param name="observed">observed operation</param>
        /// <param name="rightOp">operation to join</param>
        /// <param name="leftField">Name of the field used for the join</param>
        /// <param name="rightField">Name of the field used for the join</param>
        /// <param name="processrow">callback method called to process the row</param>
        /// <returns>resulting operation</returns>
        public static JoinOperation<Row> RightJoin(this IObservableOperation observed, IEnumerable<Row> rightOp, string leftField, string rightField, Func<Row, Row, Row> processrow)
        {
            return observed.Join(
                rightOp,
                new RowJoinHelper(leftField, rightField).RightJoinMatch,
                processrow);
        }

        /// <summary>
        /// Join an enumeration of elements to the pipeline
        /// </summary>
        /// <param name="observed">observed operation</param>
        /// <param name="rightOp">operation to join</param>
        /// <param name="field">Name of the field used for the join</param>
        /// <returns>resulting operation</returns>
        public static JoinOperation<Row> RightJoin(this IObservableOperation observed, IObservableOperation rightOp, string field)
        {
            return observed.RightJoin(rightOp, field, RowJoinHelper.MergeRows);
        }

        /// <summary>
        /// Join an enumeration of elements to the pipeline
        /// </summary>
        /// <param name="observed">observed operation</param>
        /// <param name="rightOp">operation to join</param>
        /// <param name="field">Name of the field used for the join</param>
        /// <returns>resulting operation</returns>
        public static JoinOperation<Row> RightJoin(this IObservableOperation observed, IEnumerable<Row> rightOp, string field)
        {
            return observed.RightJoin(rightOp, field, RowJoinHelper.MergeRows);
        }

        /// <summary>
        /// Join an enumeration of elements to the pipeline
        /// </summary>
        /// <param name="observed">observed operation</param>
        /// <param name="rightOp">operation to join</param>
        /// <param name="leftField">Name of the field used for the join</param>
        /// <param name="rightField">Name of the field used for the join</param>
        /// <returns>resulting operation</returns>
        public static JoinOperation<Row> RightJoin(this IObservableOperation observed, IObservableOperation rightOp, string leftField, string rightField)
        {
            return observed.RightJoin(rightOp, leftField, rightField, RowJoinHelper.MergeRows);
        }

        /// <summary>
        /// Join an enumeration of elements to the pipeline
        /// </summary>
        /// <param name="observed">observed operation</param>
        /// <param name="rightOp">operation to join</param>
        /// <param name="leftField">Name of the field used for the join</param>
        /// <param name="rightField">Name of the field used for the join</param>
        /// <returns>resulting operation</returns>
        public static JoinOperation<Row> RightJoin(this IObservableOperation observed, IEnumerable<Row> rightOp, string leftField, string rightField)
        {
            return observed.RightJoin(rightOp, leftField, rightField, RowJoinHelper.MergeRows);
        }
    }
}
