using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReactiveETL.Operations
{
    /// <summary>
    /// Output count to console
    /// </summary>
    public class ConsoleCountOperation : AbstractOperation
    {
        private ulong _count;

        private string _text;
        private string _curCount;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="text"></param>
        public ConsoleCountOperation(string text)
        {
            _count = 0;
            _text = text;
        }

        /// <summary>
        /// Dispatch the value only if filter condition is met
        /// </summary>
        /// <param name="value">value to dispatch</param>
        protected override void Dispatch(Row value)
        {
            if (string.IsNullOrEmpty(_curCount)) 
                Console.Write(_text);
            else
                Console.CursorLeft = Console.CursorLeft - _curCount.Length;

            _count++;
            _curCount = _count.ToString();

            Console.Write(_curCount);

            base.Dispatch(value);
        }

        /// <summary>
        /// Notifies the observers of the end of the sequence.
        /// </summary>
        public override void OnCompleted()
        {
            base.OnCompleted();
            Console.WriteLine(string.Empty);
        }
    }
}
