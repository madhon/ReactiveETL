using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ReactiveETL.Exceptions;
using ReactiveETL.Operations;

namespace ReactiveETL
{
    /// <summary>
    /// Extension methods for common operations
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Apply an action on the rows
        /// </summary>
        /// <param name="observed">observed operation</param>
        /// <returns>resulting operation</returns>
        public static EtlResult Start(this IObservableOperation observed)
        {
            var op = new StartOperation();
            observed.Subscribe(op);
            op.Trigger();
            return op.Result;
        }

        /// <summary>
        /// Start the operation in a thread. Start method calls are bubbled up through the pipeline
        /// </summary>
        public static EtlResult StartInThread(this IObservableOperation observed)
        {
            var op = new StartOperation();
            observed.Subscribe(op);            
            
            op.Result.Thread = new Thread(new ThreadStart(op.Trigger));
            op.Result.Thread.Start();
            
            return op.Result;
        }

        /// <summary>
        /// Execute the pipeline
        /// </summary>
        /// <param name="observed">observed operation</param>
        /// <returns>List of row resulting from the pipeline</returns>
        public static EtlFullResult Execute(this IObservableOperation observed)
        {
            return Execute(observed, true);
        }

        /// <summary>
        /// Execute the pipeline
        /// </summary>
        /// <param name="observed">observed operation</param>
        /// <param name="throwOnError">Throw exception on error</param>
        /// <returns>List of row resulting from the pipeline</returns>
        public static EtlFullResult Execute(this IObservableOperation observed, bool throwOnError)
        {
            RecordOperation record = new RecordOperation();
            observed.Subscribe(record);
            record.Start();
            
            if (record.Result.CountExceptions > 0 && throwOnError)
            {
                throw new EtlResultException(record.Result, "Pipeline error", record.Result.Exceptions.FirstOrDefault());
            }
            return record.Result;
        }

        /// <summary>
        /// Execute the pipeline
        /// </summary>
        /// <param name="observed">observed operation</param>
        /// <returns>List of row resulting from the pipeline</returns>
        public static EtlFullResult ExecuteInThread(this IObservableOperation observed)
        {
            RecordOperation record = new RecordOperation();
            observed.Subscribe(record);
            record.Result.Thread = new Thread(new ThreadStart(record.Trigger));
            record.Result.Thread.Start();

            return record.Result;
        }
        
        /// <summary>
        /// Set the name of an operation
        /// </summary>
        /// <typeparam name="T">type of the operation</typeparam>
        /// <param name="operation">operation to name</param>
        /// <param name="name">name to apply</param>
        /// <returns>named operation</returns>
        public static T Named<T>(this T operation, string name) where T : IObservableOperation
        {
            operation.Name = name;
            return operation;
        }

        /// <summary>
        /// Operation that record values in a list. 
        /// Beware that this operation keep a list of all data in memory
        /// </summary>
        /// <param name="observed">observed operation</param>
        /// <returns>resulting operation</returns>
        public static RecordOperation Record(this IObservableOperation observed)
        {
            RecordOperation observer = new RecordOperation();
            observed.Subscribe(observer);
            return observer;
        }
        
        /// <summary>
        /// Add an operation in the pipeline.
        /// </summary>
        /// <typeparam name="T">type of the operation</typeparam>
        /// <param name="observed">observed operation</param>
        /// <param name="args">arguments for the constructor</param>
        /// <returns>resulting operation</returns>
        public static T Operation<T>(this IObservableOperation observed, params object[] args) where T : IOperation
        {
            T op = (T)Activator.CreateInstance(typeof(T), args);
            observed.Subscribe(op);
            return op;
        }

        /// <summary>
        /// Transform the pipelined row
        /// </summary>
        /// <param name="observed">observed operation</param>
        /// <param name="transform">callback method for transforming the row</param>
        /// <returns>resulting operation</returns>
        public static TransformOperation Transform(this IObservableOperation observed, Func<Row, Row> transform)
        {
            TransformOperation op = new TransformOperation(transform);
            observed.Subscribe(op);
            return op;
        }

        /// <summary>
        /// Transform the pipelined row
        /// </summary>
        /// <param name="observed">observed operation</param>
        /// <param name="transform">callback method for transforming the row</param>
        /// <returns>resulting operation</returns>
        public static TransformOperation Many(this IObservableOperation observed, Func<Row, IEnumerable<Row>> transform)
        {
            TransformOperation op = new TransformOperation(transform);
            observed.Subscribe(op);
            return op;
        }

        /// <summary>
        /// Filter the rows
        /// </summary>
        /// <param name="observed">observed operation</param>
        /// <param name="filterexpr">callback method for filtering</param>
        /// <returns>resulting operation</returns>
        public static FilterOperation Filter(this IObservableOperation observed, Predicate<Row> filterexpr)
        {
            FilterOperation op = new FilterOperation(filterexpr);
            observed.Subscribe(op);
            return op;
        } 
       
        /// <summary>
        /// Apply an action on the rows
        /// </summary>
        /// <param name="observed">observed operation</param>
        /// <param name="rowact">callback method for the action</param>
        /// <returns>resulting operation</returns>
        public static ApplyOperation Apply(this IObservableOperation observed, Action<Row> rowact)
        {
            ApplyOperation op = new ApplyOperation(rowact);
            observed.Subscribe(op);
            return op;
        }

        /// <summary>
        /// Write text followed by currently processed row number on console
        /// </summary>
        /// <param name="observed">observed operation</param>
        /// <param name="text">callback method for the action</param>
        /// <returns>resulting operation</returns>
        public static ConsoleCountOperation ConsoleCount(this IObservableOperation observed, string text)
        {
            var op = new ConsoleCountOperation(text);
            observed.Subscribe(op);
            return op;
        }

        /// <summary>
        /// Apply an action on the rows
        /// </summary>
        /// <param name="observed">observed operation</param>
        /// <param name="columns">callback method for the action</param>
        /// <returns>resulting operation</returns>
        public static GroupByOperation GroupBy(this IObservableOperation observed, params string[] columns)
        {
            var op = new GroupByOperation(columns);
            observed.Subscribe(op);
            return op;
        }

        public static GroupByOperation GroupBy(this IObservableOperation observed, string[] columns, Action<Row, Row> aggregate = null)
        {
            var op = new GroupByOperation(columns, aggregate);
            observed.Subscribe(op);
            return op;
        }

        /// <summary>
        /// Dispatch the grouped elements (trigger for every grouped rows)
        /// </summary>
        /// <param name="observed">observed operation</param>
        /// <returns>resulting operation</returns>
        public static DispatchGroupOperation DispatchGroup(this IObservableOperation observed)
        {
            var op = new DispatchGroupOperation();
            observed.Subscribe(op);
            return op;
        }

        /// <summary>
        /// Dispatch the grouped elements (trigger for every grouped rows)
        /// </summary>
        /// <param name="groupedRow">observed operation</param>
        /// <returns>resulting operation</returns>
        public static List<Row> GroupedRows(this Row groupedRow)
        {
            return (List<Row>)groupedRow[Constants.GroupListName];
        }

        /// <summary>
        /// Dispatch the grouped elements (trigger for every grouped rows)
        /// </summary>
        /// <param name="groupedRow">observed operation</param>
        /// <returns>resulting operation</returns>
        public static Row ParentRow(this Row groupedRow)
        {
            return (Row)groupedRow[Constants.GroupParentName];
        }
    }
}
