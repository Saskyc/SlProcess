using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeryEpicEventPlugin.Utilities
{
    /// <summary>
    /// Result of a method that can cause exceptions without throwing.
    /// </summary>
    public class MethodResult<T>
    {
        /// <summary>
        /// The result of method.
        /// </summary>
        public T Result { get; } 

        /// <summary>
        /// Exception caused because of the method
        /// </summary>
        public Exception? Exception { get; }

        /// <summary>
        /// Constructor creating <see cref="MethodResult{T}"/>
        /// </summary>
        /// <param name="result">The result of the method</param>
        /// <param name="exception">The excpetion caused (if any), defaultly nothing.</param>
        public MethodResult(T result, Exception? exception = null)
        {
            Result = result;
            Exception = exception;
        }

        /// <summary>
        /// Implicit conversion constructing <see cref="MethodResult{T}"/> from result.
        /// </summary>
        /// <param name="result"></param>
        public static implicit operator MethodResult<T>(T result)
        {
            return new MethodResult<T>(T);
        }

        /// <summary>
        /// Implicit conversion of <see cref="MethodResult{T}"/> to result.
        /// </summary>
        /// <param name="methodResult"></param>
        public static implicit operator T(MethodResult<T> methodResult)
        {
            return methodResult.Result;
        }

        /// <summary>
        /// Implicit conversion of <see cref="MethodResult{T}"/> to Exception (if any).
        /// </summary>
        /// <param name="methodResult"></param>
        public static implicit operator Exception?(MethodResult methodResult)
        {
            return methodResult.Exception;
        }
    }
}
