using System;
using System.Linq.Expressions;

namespace Tp.Core.Expressions
{
	internal class ExpressionEventArg<T> : EventArgs where T:Expression
	{
		public T Expression { get; set; }
		public Expression Result { get; set; }


		public bool Processed { get; set; }

		public ExpressionEventArg(T expression, bool processByDefault)
		{
			Expression = expression;
			Result = expression;
			Processed = processByDefault;
		}
	}
}