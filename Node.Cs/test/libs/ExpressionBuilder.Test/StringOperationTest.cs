// ===========================================================
// Copyright (C) 2014-2015 Kendar.org
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, 
// modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software 
// is furnished to do so, subject to the following conditions:
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS 
// BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF 
// OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ===========================================================


using System;
using ExpressionBuilder.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressionBuilder.Test
{

	[TestClass]
	public class StringOperationTest
	{
		[TestMethod]
		public void ItShouldPossibleToInvokeToString()
		{
			const string expected =
@"public System.String Call(System.Int32 par)
{
  System.String result;
  result = par.ToString();
  return result;
}";

			var newExpression = Function.Create()
				.WithParameter<int>("par")
				.WithBody(
					CodeLine.CreateVariable<string>("result"),
					CodeLine.Assign(Operation.Variable("result"), StringOperation.ToString("par"))
				)
				.Returns("result");

			AssertString.AreEqual(expected, newExpression.ToString());

			var lambda = newExpression.ToLambda<Func<int, string>>();
			Assert.IsNotNull(lambda);
			var result = lambda(1);
			Assert.AreEqual("1", result);
		}

		[TestMethod]
		public void ItShouldPossibleToInvokeStringCompare()
		{
			const string expected =
@"public System.Int32 Call(System.String par1, System.String par2)
{
  System.Int32 result;
  result = System.String.Compare(par1, par2, CurrentCulture);
  return result;
}";

			var newExpression = Function.Create()
				.WithParameter<string>("par1")
				.WithParameter<string>("par2")
				.WithBody(
					CodeLine.CreateVariable<int>("result"),
					CodeLine.Assign(Operation.Variable("result"), StringOperation.Compare("par1", "par2"))
				)
				.Returns("result");

			AssertString.AreEqual(expected, newExpression.ToString());

			var lambda = newExpression.ToLambda<Func<string, string, int>>();
			Assert.IsNotNull(lambda);
			var result = lambda("a", "a");
			Assert.AreEqual(0, result);
			result = lambda("a", "b");
			Assert.AreEqual(-1, result);
		}

		[TestMethod]
		public void ItShouldPossibleToInvokeStringFormat()
		{
			const string expected =
@"public System.String Call(System.String par1, System.String par2)
{
  System.String result;
  result = System.String.Format(""{0}-{1}"", par1, par2);
  return result;
}";

			var newExpression = Function.Create()
				.WithParameter<string>("par1")
				.WithParameter<string>("par2")
				.WithBody(
					CodeLine.CreateVariable<string>("result"),
					CodeLine.Assign(Operation.Variable("result"),
						StringOperation.Format(
							"{0}-{1}",
							Operation.Variable("par1"),
							Operation.Variable("par2")))
				)
				.Returns("result");

			AssertString.AreEqual(expected, newExpression.ToString());

			var lambda = newExpression.ToLambda<Func<string, string, string>>();
			Assert.IsNotNull(lambda);
			var result = lambda("a", "b");
			Assert.AreEqual("a-b", result);
		}
	}
}
