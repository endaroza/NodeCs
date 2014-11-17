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
	public class TestObject
	{
		
	}

	[TestClass]
	public class CodeLineTest
	{

		[TestMethod]
		[Ignore]
		public void CodeLinesParametrizedOperationActions()
		{

		}
		[TestMethod]
		public void AssignShouldAssign()
		{
			const string expected =
@"public System.String Call(System.String first, System.String second)
{
  first = second;
  return first;
}";

			var newExpression = Function.Create()
					.WithParameter<string>("first")
					.WithParameter<string>("second")
					.WithBody(
							CodeLine.Assign("first", "second")
					)
					.Returns("first");

			AssertString.AreEqual(expected, newExpression.ToString());

			var lambda = newExpression.ToLambda<Func<string, string, string>>();
			Assert.IsNotNull(lambda);

			var result = lambda("test", "another");
			Assert.AreEqual("another", result);
		}

		[TestMethod]
		public void SumAssignShouldWorkForStrings()
		{
			const string expected =
@"public System.String Call(System.String first, System.String second)
{
  first += second;
  return first;
}";

			var newExpression = Function.Create()
					.WithParameter<string>("first")
					.WithParameter<string>("second")
					.WithBody(
							CodeLine.Assign("first", "second", AssignementOperator.SumAssign)
					)
					.Returns("first");

			AssertString.AreEqual(expected, newExpression.ToString());

			var lambda = newExpression.ToLambda<Func<string, string, string>>();
			Assert.IsNotNull(lambda);

			var result = lambda("test", "another");
			Assert.AreEqual("testanother", result);
		}


		[TestMethod]
		public void SumAssignShouldWorkForValueTypes()
		{
			const string expected =
@"public System.Int32 Call(System.Int32 first, System.Int32 second)
{
  first += second;
  return first;
}";

			var newExpression = Function.Create()
					.WithParameter<int>("first")
					.WithParameter<int>("second")
					.WithBody(
							CodeLine.Assign("first", "second", AssignementOperator.SumAssign)
					)
					.Returns("first");

			AssertString.AreEqual(expected, newExpression.ToString());

			var lambda = newExpression.ToLambda<Func<int, int, int>>();
			Assert.IsNotNull(lambda);

			var result = lambda(1, 2);
			Assert.AreEqual(3, result);
		}

		[TestMethod]
		public void AssignShouldAssignObjects()
		{
			const string expected =
@"public ExpressionBuilder.Test.TestObject Call(ExpressionBuilder.Test.TestObject first, ExpressionBuilder.Test.TestObject second)
{
  first = second;
  return first;
}";

			var newExpression = Function.Create()
					.WithParameter<TestObject>("first")
					.WithParameter<TestObject>("second")
					.WithBody(
							CodeLine.Assign("first", "second")
					)
					.Returns("first");

			AssertString.AreEqual(expected, newExpression.ToString());

			var lambda = newExpression.ToLambda<Func<TestObject, TestObject, TestObject>>();
			Assert.IsNotNull(lambda);

			var expectedResult = new TestObject();
			var result = lambda(new TestObject(), expectedResult);
			Assert.AreSame(expectedResult, result);
		}

		[TestMethod]
		public void AssignShouldAcceptRightableAndLeftable()
		{
			const string expected =
	@"public System.String Call(System.String par)
{
  par = ""another"";
  return par;
}";

			var newExpression = Function.Create()
					.WithParameter<string>("par")
					.WithBody(
							CodeLine.Assign(Operation.Variable("par"), Operation.Constant("another"))
					)
					.Returns("par");

			var newSource = newExpression.ToString();
			AssertString.AreEqual(expected, newSource);

			var lambda = newExpression.ToLambda<Func<string, string>>();
			Assert.IsNotNull(lambda);

			var result = lambda("test");
			Assert.AreEqual("another", result);
		}



		[TestMethod]
		public void AssignShouldWorkForInternalVariables()
		{
			const string expected =
	@"public System.String Call(System.String par)
{
  System.String var;
  var = ""another"";
  var += par;
  return var;
}";

			var newExpression = Function.Create()
				.WithParameter<string>("par")
				.WithBody(
							CodeLine.CreateVariable<string>("var"),
							CodeLine.Assign("var", Operation.Constant("another")),
							CodeLine.Assign(Operation.Variable("var"), Operation.Variable("par"), AssignementOperator.SumAssign)
					)
					.Returns("var");

			AssertString.AreEqual(expected, newExpression.ToString());

			var lambda = newExpression.ToLambda<Func<string, string>>();
			Assert.IsNotNull(lambda);

			var result = lambda("test");
			Assert.AreEqual("anothertest", result);
		}

		[TestMethod]
		public void CreateVariablesShouldWorkInsideWhileIfAndThen()
		{
			const string expected =
	@"public void Call(System.String par)
{
  System.Int32 count;
  count = 1;
  while(count == 0)
  {
   if(True == True)
   {
    System.String var;
    var = par;
   }   
   else
   {
    System.String var;
    var = par;
   };
  };
}";

			var newExpression = Function.Create()
				.WithParameter<string>("par")
				.WithBody(
					CodeLine.CreateVariable<int>("count"),
					CodeLine.AssignConstant("count", 1),
					CodeLine.CreateWhile(Condition.CompareConst("count", 0))
						.Do(
							CodeLine.CreateIf(Condition.CompareConst(Operation.Constant(true), true))
								.Then(
									CodeLine.CreateVariable<string>("var"),
									CodeLine.Assign("var", "par")
								)
								.ElseIf(Condition.CompareConst(Operation.Constant(true), true))
								.Then(
									CodeLine.CreateVariable<string>("var"),
									CodeLine.Assign("var", "par")
								)
								.Else(
									CodeLine.CreateVariable<string>("var"),
									CodeLine.Assign("var", "par")
								)
						)
					);

			AssertString.AreEqual(expected, newExpression.ToString());

			var lambda = newExpression.ToLambda<Action<string>>();
			Assert.IsNotNull(lambda);

			lambda("test");
		}

		[TestMethod]
		public void MultiplyAssignShouldWorkForValueTypes()
		{
			const string expected =
@"public System.Int32 Call(System.Int32 first, System.Int32 second)
{
  first *= second;
  return first;
}";

			var newExpression = Function.Create()
					.WithParameter<int>("first")
					.WithParameter<int>("second")
					.WithBody(
							CodeLine.Assign("first", "second", AssignementOperator.MultiplyAssign)
					)
					.Returns("first");

			AssertString.AreEqual(expected, newExpression.ToString());

			var lambda = newExpression.ToLambda<Func<int, int, int>>();
			Assert.IsNotNull(lambda);

			var result = lambda(1, 2);
			Assert.AreEqual(2, result);
		}

		[TestMethod]
		public void SubtractAssignShouldWorkForValueTypes()
		{
			const string expected =
@"public System.Int32 Call(System.Int32 first, System.Int32 second)
{
  first -= second;
  return first;
}";

			var newExpression = Function.Create()
					.WithParameter<int>("first")
					.WithParameter<int>("second")
					.WithBody(
							CodeLine.Assign("first", "second", AssignementOperator.SubtractAssign)
					)
					.Returns("first");

			AssertString.AreEqual(expected, newExpression.ToString());

			var lambda = newExpression.ToLambda<Func<int, int, int>>();
			Assert.IsNotNull(lambda);

			var result = lambda(4, 2);
			Assert.AreEqual(2, result);
		}
	}
}
