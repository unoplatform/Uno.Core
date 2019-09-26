using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uno.Extensions;

namespace Uno.Core.Tests.Reflection
{
	[TestClass]
	public class TypeExtensionsFixture
	{
		[TestMethod]
		public void TestGetBaseTypes()
		{
			var baseTypes = typeof(MoreDerivedType).GetBaseTypes().ToArray();
			CollectionAssert.AreEqual(new[] { typeof(DerivedType), typeof(BaseType), typeof(object) }, baseTypes);
		}

		public class BaseType { }

		public class DerivedType : BaseType { }

		public class MoreDerivedType : DerivedType { }
	}
}
