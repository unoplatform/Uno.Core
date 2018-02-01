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
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Uno.Core.Tests
{
	partial class TransactionalFixture
	{
		[TestMethod]
		public void When_Dictionary_GetOrAdd_Then_FactoryIsInvokedOnlyOnce()
		{
			var dictionary = ImmutableDictionary<object, object>.Empty;

			var invocation = 0;
			Transactional.GetOrAdd(
				ref dictionary, 
				new object(), 
				o =>
				{
					// Cause concurrency issue
					if (invocation++ == 0)
					{
						dictionary = ImmutableDictionary<object, object>.Empty.Add(new object(), new object());
					}

					return new object();
				});

			Assert.AreEqual(2, dictionary.Count);
			Assert.AreEqual(1, invocation);
		}

		[TestMethod]
		public void When_Dictionary_GetOrAddWithCOntext_Then_FactoryIsInvokedOnlyOnce()
		{
			var dictionary = ImmutableDictionary<object, object>.Empty;

			var invocation = 0;
			Transactional.GetOrAdd(
				ref dictionary,
				new object(),
				new object(),
				(o, c) =>
				{
					// Cause concurrency issue
					if (invocation++ == 0)
					{
						dictionary = ImmutableDictionary<object, object>.Empty.Add(new object(), new object());
					}

					return new object();
				});

			Assert.AreEqual(2, dictionary.Count);
			Assert.AreEqual(1, invocation);
		}

		[TestMethod]
		public void When_Dictionary_TryAdd_Then_FactoryIsInvokedOnlyOnce()
		{
			var dictionary = ImmutableDictionary<object, object>.Empty;

			var invocation = 0;
			object _;
			Transactional.TryAdd(
				ref dictionary, 
				new object(), 
				o =>
				{
					// Cause concurrency issue
					if (invocation++ == 0)
					{
						dictionary = ImmutableDictionary<object, object>.Empty.Add(new object(), new object());
					}

					return new object();
				},
				out _);

			Assert.AreEqual(2, dictionary.Count);
			Assert.AreEqual(1, invocation);
		}

		[TestMethod]
		public void When_Dictionary_SetItem_Then_FactoryIsInvokedOnlyOnce()
		{
			var key = new object();
			var dictionary = ImmutableDictionary<object, object>.Empty.Add(key, new object());

			var invocation = 0;
			Transactional.SetItem(
				ref dictionary,
				new object(),
				o =>
				{
					// Cause concurrency issue
					if (invocation++ == 0)
					{
						dictionary = ImmutableDictionary<object, object>.Empty.Add(new object(), new object());
					}

					return new object();
				});

			Assert.AreEqual(1, invocation);
		}

		[TestMethod]
		public void When_Dictionary_UpdateItem_RetunsSameValue_Then_DictionaryNotUpdated()
		{
			var key = new object();

			// **I**ImmutableDictionary
			IImmutableDictionary<object, object> dictionary_interface = ImmutableDictionary<object, object>.Empty.Add(key, new object());
			var dictionary_interface_capture = dictionary_interface;

			Transactional.UpdateItem(ref dictionary_interface, key, (k, v) => v);

			Assert.AreSame(dictionary_interface_capture, dictionary_interface);

			// ImmutableDictionary
			ImmutableDictionary<object, object> dictionary_implementation = ImmutableDictionary<object, object>.Empty.Add(key, new object());
			var dictionary_implementation_capture = dictionary_implementation;

			Transactional.TryUpdateItem(ref dictionary_implementation, key, (k, v) => v);

			Assert.AreSame(dictionary_implementation_capture, dictionary_implementation);

			// ImmutableSortedDictionary
			ImmutableSortedDictionary<object, object> dictionary_sorted = ImmutableSortedDictionary<object, object>.Empty.Add(key, new object());
			var dictionary_sorted_capture = dictionary_sorted;

			Transactional.TryUpdateItem(ref dictionary_sorted, key, (k, v) => v);

			Assert.AreSame(dictionary_sorted_capture, dictionary_sorted);
		}

	}
}
