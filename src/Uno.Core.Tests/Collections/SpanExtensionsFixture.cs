using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uno.Collections;

namespace Uno.Core.Tests.Collections
{
    [TestClass]
    public class SpanExtensionsFixture
    {
        [TestMethod]
        public void When_SelectToSpan()
        {
            Span<int> s = stackalloc int[] { 1, 2, 3 };

            Span<int> s2 = stackalloc int[3];
            s.SelectToSpan(s2, v => v + 1);

            Assert.IsTrue(s.ToArray().Select(v => v + 1).SequenceEqual(s2.ToArray()));
        }

        [TestMethod]
        public void When_List_SelectToSpan()
        {
            var s = new List<int> { 1, 2, 3 };

            Span<int> s2 = stackalloc int[3];
            s.SelectToSpan(s2, v => v + 1);

            Assert.IsTrue(s.ToArray().Select(v => v + 1).SequenceEqual(s2.ToArray()));
        }

        [TestMethod]
        public void When_Array_SelectToSpan()
        {
            var s = new int[] { 1, 2, 3 };

            Span<int> s2 = stackalloc int[3];
            s.SelectToSpan(s2, v => v + 1);

            Assert.IsTrue(s.ToArray().Select(v => v + 1).SequenceEqual(s2.ToArray()));
        }

        [TestMethod]
        public void When_SelectToSpan_Indexed()
        {
            Span<int> s = stackalloc int[] { 1, 2, 3 };

            Span<int> s2 = stackalloc int[3];
            s.SelectToSpan(s2, (v, i) => v + i);

            Assert.IsTrue(s.ToArray().Select((v, i) => v + i).SequenceEqual(s2.ToArray()));
        }

        [TestMethod]
        public void When_WhereToSpan()
        {
            Span<int> s = stackalloc int[] { 1, 2, 3 };

            Span<int> s2 = stackalloc int[3];
            var s3 = s.WhereToSpan(s2, v => v > 1);

            Assert.IsTrue(s.ToArray().Where(v => v > 1).SequenceEqual(s3.ToArray()));
        }

        [TestMethod]
        public void When_SelectToMemory()
        {
            Span<int> s = stackalloc int[] { 1, 2, 3 };

            var s2 = s.SelectToMemory(v => v + 1);

            Assert.IsTrue(s.ToArray().Select(v => v + 1).SequenceEqual(s2.ToArray()));
        }

        [TestMethod]
        public void When_List_SelectToMemory()
        {
            var s = new List<int> { 1, 2, 3 };

            var s2 = s.SelectToMemory(v => v + 1);

            Assert.IsTrue(s.ToArray().Select(v => v + 1).SequenceEqual(s2.ToArray()));
        }

        [TestMethod]
        public void When_SelectToMemory_And_Empty()
        {
            Span<int> s = stackalloc int[0];

            var s2 = s.SelectToMemory(v => v + 1);

            Assert.IsTrue(s.ToArray().Select(v => v + 1).SequenceEqual(s2.ToArray()));
        }

        [TestMethod]
        public void When_SelectToMemory_Indexed()
        {
            Span<int> s = stackalloc int[] { 1, 2, 3 };

            var s2 = s.SelectToMemory((v, i) => v + i);

            Assert.IsTrue(s.ToArray().Select((v, i) => v + i).SequenceEqual(s2.ToArray()));
        }

        [TestMethod]
        public void When_WhereToMemory()
        {
            Span<int> s = stackalloc int[] { 1, 2, 3 };

            var s3 = s.WhereToMemory(v => v > 1);

            Assert.IsTrue(s.ToArray().Where(v => v > 1).SequenceEqual(s3.ToArray()));
        }


        [TestMethod]
        public void When_WhereToMemory_Indexed()
        {
            Span<int> s = stackalloc int[] { 1, 2, 3 };

            var s3 = s.WhereToMemory((v, i) => v > i);

            Assert.IsTrue(s.ToArray().Where((v, i) => v > i).SequenceEqual(s3.ToArray()));
        }


        [TestMethod]
        public void When_WhereToMemory_Indexed_Projected()
        {
            Span<int> s = stackalloc int[] { 1, 2, 3 };

            var s3 = s.WhereToMemory(v => v > 1, v => v + 1);

            Assert.IsTrue(s.ToArray().Where(v => v > 1).Select(v => v + 1).SequenceEqual(s3.ToArray()));
        }


        [TestMethod]
        public void When_Span_Count()
        {
            Span<int> s = stackalloc int[] { 1, 2, 3 };

            var s3 = s.Count(v => v > 1);

            Assert.AreEqual(s3, s.ToArray().Count(v => v > 1));
        }

        [TestMethod]
        public void When_Span_Count_Empty()
        {
            Span<int> s = stackalloc int[0];

            var s3 = s.Count(v => v > 1);

            Assert.AreEqual(s3, s.ToArray().Count(v => v > 1));
        }

        [TestMethod]
        public void When_Contains()
        {
            Span<int> s = stackalloc int[] { 1, 2, 3 };

            Assert.IsTrue(s.Contains(v => v == 2));
        }

        [TestMethod]
        public void When_Not_Contains()
        {
            Span<int> s = stackalloc int[] { 1, 2, 3 };

            Assert.IsFalse(s.Contains(v => v == 4));
        }

        [TestMethod]
        public void When_ToDictionary()
        {
            Span<int> s = stackalloc int[] { 1, 2, 3 };

            var dict = s.ToDictionary(k => k, v => v + 1);

            Assert.AreEqual(3, dict.Count);
            Assert.AreEqual(2, dict[1]);
            Assert.AreEqual(3, dict[2]);
            Assert.AreEqual(4, dict[3]);
        }

        [TestMethod]
        public void When_Double_Sum()
        {
            Span<double> s = stackalloc double[] { 1, 2, 3 };

            Assert.AreEqual(6, s.Sum());
        }

        [TestMethod]
        public void When_Double_Projected_Sum()
        {
            Span<double> s = stackalloc double[] { 1, 2, 3 };

            Assert.AreEqual(9, s.Sum(v => v + 1));
        }

        [TestMethod]
        public void When_ClampedSlice()
        {
            Span<double> s = stackalloc double[] { 1, 2, 3 };

            var s2 = s.SliceClamped(0, 1);
            Assert.AreEqual(1, s2.Length);
            Assert.AreEqual(1, s2[0]);
        }

        [TestMethod]
        public void When_ClampedSlice_Too_Long()
        {
            Span<double> s = stackalloc double[] { 1, 2, 3 };

            var s2 = s.SliceClamped(0, 5);
            Assert.AreEqual(3, s2.Length);
            Assert.AreEqual(1, s2[0]);
            Assert.AreEqual(2, s2[1]);
            Assert.AreEqual(3, s2[2]);
        }

        [TestMethod]
        public void When_ClampedSlice_Zero()
        {
            Span<double> s = stackalloc double[] { 1, 2, 3 };

            var s2 = s.SliceClamped(0, 0);
            Assert.AreEqual(0, s2.Length);
        }

        [TestMethod]
        public void When_ClampedSlice_Too_Far()
        {
            Span<double> s = stackalloc double[] { 1, 2, 3 };

            var s2 = s.SliceClamped(4, 1);
            Assert.AreEqual(0, s2.Length);
        }
    }
}
