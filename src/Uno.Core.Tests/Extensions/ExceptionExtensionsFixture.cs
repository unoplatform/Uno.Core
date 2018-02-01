// ******************************************************************
// Copyright � 2015-2018 nventive inc. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// ******************************************************************
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Extensions;

namespace Uno.Core.Tests.Extensions
{
	[TestClass]
	public class ExceptionExtensionsFixture
	{
		[TestMethod]
		public void TestDirectType()
		{
			var exception = new TestException();

			Assert.IsTrue(exception.IsOrContainsExceptionType(typeof(TestException)));
			Assert.IsTrue(exception.IsOrContainsExceptionType<TestException>());

			Assert.AreEqual(exception.GetPossibleInnerException<TestException>(), exception);
		}

		[TestMethod]
		public void TestNotDirectType()
		{
			var exception = new TestException();

			Assert.IsFalse(exception.IsOrContainsExceptionType(typeof(InvalidOperationException)));
			Assert.IsFalse(exception.IsOrContainsExceptionType<InvalidOperationException>());

			Assert.IsNull(exception.GetPossibleInnerException<InvalidOperationException>());
		}

		[TestMethod]
		public void TestFirstLevelChildExceptionType()
		{
			var innerException = (new TestException());
			var aggregateException = new AggregateException(innerException);

			Assert.IsTrue(aggregateException.IsOrContainsExceptionType(typeof(TestException)));
			Assert.IsTrue(aggregateException.IsOrContainsExceptionType<TestException>());

			Assert.AreEqual(aggregateException.GetPossibleInnerException<TestException>(), innerException);
		}

		[TestMethod]
		public void TestNotFirstLevelExceptionType()
		{
			var innerException = (new TestException());
			var aggregateException = new AggregateException(innerException);

			Assert.IsFalse(aggregateException.IsOrContainsExceptionType(typeof(InvalidOperationException)));
			Assert.IsFalse(aggregateException.IsOrContainsExceptionType<InvalidOperationException>());

			Assert.IsNull(aggregateException.GetPossibleInnerException<InvalidOperationException>());
		}

		[TestMethod]
		public void TestSecondLevelChildExceptionType()
		{
			var innerException = (new TestException());
			var aggregateException = new AggregateException(new AggregateException(innerException));

			Assert.IsTrue(aggregateException.IsOrContainsExceptionType(typeof(TestException)));
			Assert.IsTrue(aggregateException.IsOrContainsExceptionType<TestException>());

			Assert.AreEqual(aggregateException.GetPossibleInnerException<TestException>(), innerException);
		}

		[TestMethod]
		public void TestNotSecondLevelExceptionType()
		{
			var innerException = (new TestException());
			var aggregateException = new AggregateException(new AggregateException(innerException));

			Assert.IsFalse(aggregateException.IsOrContainsExceptionType(typeof(InvalidOperationException)));
			Assert.IsFalse(aggregateException.IsOrContainsExceptionType<InvalidOperationException>());

			Assert.IsNull(aggregateException.GetPossibleInnerException<InvalidOperationException>());
		}
	}

	public class TestException : Exception
	{

	}
}
