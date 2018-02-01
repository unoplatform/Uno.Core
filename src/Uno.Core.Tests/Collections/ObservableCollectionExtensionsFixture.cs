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
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections.Generic;
using Uno.Extensions;

namespace Uno.Core.Tests.Collections
{
	[TestClass]
	public class ObservableCollectionExtensionsFixture_Update
	{
		[TestMethod]
		public void AddSingleOnTail()
		{
			var changes = new List<NotifyCollectionChangedEventArgs>();

			var c = new ObservableCollection<int>() { 1, 2, 3, 4 };
			c.CollectionChanged += (s, e) => changes.Add(e);

			var updated = new[] { 1, 2, 3, 4, 5 };
			c.Update(updated);

			Assert.AreEqual(5, c.Count);
			Assert.IsTrue(updated.SequenceEqual(c));
		}

		[TestMethod]
		public void AddSingleOnTail_WithResults()
		{
			var changes = new List<NotifyCollectionChangedEventArgs>();

			var c = new ObservableCollection<int>() { 1, 2, 3, 4 };
			c.CollectionChanged += (s, e) => changes.Add(e);

			var updated = new[] { 1, 2, 3, 4, 5 };
			var results = c.UpdateWithResults(updated);

			Assert.AreEqual(5, c.Count);
			Assert.IsTrue(updated.SequenceEqual(c));

			Assert.AreEqual(0, results.Removed.Count());
			Assert.AreEqual(0, results.Moved.Count());
			Assert.AreEqual(1, results.Added.Count());
			Assert.AreEqual(5, results.Added.ElementAt(0));
		}

		[TestMethod]
		public void AddSingleOnHead()
		{
			var changes = new List<NotifyCollectionChangedEventArgs>();

			var c = new ObservableCollection<int>() { 1, 2, 3, 4 };
			c.CollectionChanged += (s, e) => changes.Add(e);

			var updated = new[] { 0, 1, 2, 3, 4 };
			c.Update(updated);

			Assert.AreEqual(5, c.Count);
			Assert.IsTrue(updated.SequenceEqual(c));
		}

		[TestMethod]
		public void AddSingleOnHead_WithResults() 
		{
			var changes = new List<NotifyCollectionChangedEventArgs>();

			var c = new ObservableCollection<int>() { 1, 2, 3, 4 };
			c.CollectionChanged += (s, e) => changes.Add(e);

			var updated = new[] { 0, 1, 2, 3, 4 };
			var results = c.UpdateWithResults(updated);

			Assert.AreEqual(5, c.Count);
			Assert.IsTrue(updated.SequenceEqual(c));

			Assert.AreEqual(0, results.Removed.Count());
			Assert.AreEqual(0, results.Moved.Count());
			Assert.AreEqual(1, results.Added.Count());
			Assert.AreEqual(0, results.Added.ElementAt(0));
		}

		[TestMethod]
		public void AddSingleOnHeadAndHead()
		{
			var changes = new List<NotifyCollectionChangedEventArgs>();

			var c = new ObservableCollection<int>() { 1, 2, 3, 4 };
			c.CollectionChanged += (s, e) => changes.Add(e);

			var updated = new[] { 0, 1, 2, 3, 4, 5 };
			c.Update(updated);

			Assert.AreEqual(6, c.Count);
			Assert.IsTrue(updated.SequenceEqual(c));
		}

		[TestMethod]
		public void AddSingleOnHeadAndHead_WithResults()
		{
			var changes = new List<NotifyCollectionChangedEventArgs>();

			var c = new ObservableCollection<int>() { 1, 2, 3, 4 };
			c.CollectionChanged += (s, e) => changes.Add(e);

			var updated = new[] { 0, 1, 2, 3, 4, 5 };
			var results = c.UpdateWithResults(updated);

			Assert.AreEqual(6, c.Count);
			Assert.IsTrue(updated.SequenceEqual(c));

			Assert.AreEqual(0, results.Removed.Count());
			Assert.AreEqual(0, results.Moved.Count());
			Assert.AreEqual(2, results.Added.Count());
			Assert.AreEqual(0, results.Added.ElementAt(0));
			Assert.AreEqual(5, results.Added.ElementAt(1));
		}

		[TestMethod]
		public void MoveSingleItem()
		{
			var changes = new List<NotifyCollectionChangedEventArgs>();

			var c = new ObservableCollection<int>() { 1, 2, 3, 4 };
			c.CollectionChanged += (s, e) => changes.Add(e);

			var updated = new[] { 1, 2, 4, 3 };
			c.Update(updated);

			Assert.AreEqual(4, c.Count);
			Assert.IsTrue(updated.SequenceEqual(c));
		}
		  
		[TestMethod]
		public void MoveSingleItem_WithResults()
		{
			var changes = new List<NotifyCollectionChangedEventArgs>();

			var c = new ObservableCollection<int>() { 1, 2, 3, 4 };
			c.CollectionChanged += (s, e) => changes.Add(e);

			var updated = new[] { 1, 2, 4, 3 };
			var results = c.UpdateWithResults(updated);

			Assert.AreEqual(4, c.Count);
			Assert.IsTrue(updated.SequenceEqual(c));

			Assert.AreEqual(0, results.Removed.Count());
			Assert.AreEqual(0, results.Added.Count());
			Assert.AreEqual(1, results.Moved.Count());
			Assert.AreEqual(4, results.Moved.ElementAt(0));
		}

		[TestMethod]
		public void MoveRemoveSingleItem()
		{
			var changes = new List<NotifyCollectionChangedEventArgs>();

			var c = new ObservableCollection<int>() { 1, 2, 3, 4 };
			c.CollectionChanged += (s, e) => changes.Add(e);

			var updated = new[] { 0, 1, 4, 3 };
			c.Update(updated);

			Assert.AreEqual(4, c.Count);
			Assert.IsTrue(updated.SequenceEqual(c));
		}
		  
		[TestMethod]
		public void MoveRemoveSingleItem_WithResults()
		{
			var changes = new List<NotifyCollectionChangedEventArgs>();

			var c = new ObservableCollection<int>() { 1, 2, 3, 4 };
			c.CollectionChanged += (s, e) => changes.Add(e);

			var updated = new[] { 0, 1, 4, 3 };
			var results = c.UpdateWithResults(updated);

			Assert.AreEqual(4, c.Count);
			Assert.IsTrue(updated.SequenceEqual(c));

			Assert.AreEqual(1, results.Added.Count());
			Assert.AreEqual(0, results.Added.ElementAt(0));
			Assert.AreEqual(1, results.Moved.Count());
			Assert.AreEqual(4, results.Moved.ElementAt(0));
			Assert.AreEqual(1, results.Removed.Count());
			Assert.AreEqual(2, results.Removed.ElementAt(0));
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void UpdateWithDuplicates()
		{
			var changes = new List<NotifyCollectionChangedEventArgs>();

			var c = new ObservableCollection<int>() { 8, 7 };
			c.CollectionChanged += (s, e) => changes.Add(e);

			var updated = new[] { 5, 13, 15, 17, 16, 3, 3, 2, 14 };
			c.Update(updated);
		}

		[TestMethod]
		public void Reverse()
		{
			var changes = new List<NotifyCollectionChangedEventArgs>();

			var c = new ObservableCollection<int>() { 1, 2, 3, 4 };
			c.CollectionChanged += (s, e) => changes.Add(e);

			var updated = c.Reverse().ToArray();
			c.Update(updated);

			Assert.AreEqual(4, c.Count);
			Assert.IsTrue(updated.SequenceEqual(c));
		}

		[TestMethod]
		public void RemoveSingle()
		{
			var changes = new List<NotifyCollectionChangedEventArgs>();

			var c = new ObservableCollection<int>() { 1, 2, 3, 4 };
			c.CollectionChanged += (s, e) => changes.Add(e);

			var updated = new[] { 1, 2, 3 };
			c.Update(updated);

			Assert.AreEqual(3, c.Count);
			Assert.IsTrue(updated.SequenceEqual(c));
		}

		[TestMethod]
		public void RemoveSingle_WithResults()
		{
			var changes = new List<NotifyCollectionChangedEventArgs>();

			var c = new ObservableCollection<int>() { 1, 2, 3, 4 };
			c.CollectionChanged += (s, e) => changes.Add(e);

			var updated = new[] { 1, 2, 3 };
			var results = c.UpdateWithResults(updated);

			Assert.AreEqual(3, c.Count);
			Assert.IsTrue(updated.SequenceEqual(c));

			Assert.AreEqual(1, results.Removed.Count());
			Assert.AreEqual(4, results.Removed.ElementAt(0));
			Assert.AreEqual(0, results.Added.Count());
			Assert.AreEqual(0, results.Moved.Count());
		}
	}
}
