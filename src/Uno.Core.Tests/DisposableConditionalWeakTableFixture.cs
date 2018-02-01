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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Disposables;

namespace Uno.Core.Tests
{
	[TestClass]
	public class DisposableConditionalWeakTableFixture
	{
		private Environnement _environnement;

		[TestInitialize]
		public void Init() => _environnement = new Environnement();

		[TestMethod]
		public void When_KeyCollected_Then_ValueIsDisposed()
		{
			var key = _environnement.AddWeak();

			TryCollect(key);

			Assert.IsFalse(key.IsAlive);
			Assert.AreEqual(1, _environnement.CreatedItems);
			Assert.AreEqual(1, _environnement.DisposedItems);
		}


		[TestMethod]
		public void While_KeyStronglyReferenced_Then_ValueIsNotDisposed()
		{
			var key = _environnement.AddStrong();

			TryCollect(key);

			Assert.IsTrue(key.IsAlive);
			Assert.AreEqual(1, _environnement.CreatedItems);
			Assert.AreEqual(0, _environnement.DisposedItems);

			_environnement.ReleaseStrong(key);

			TryCollect(key);

			Assert.IsFalse(key.IsAlive);
			Assert.AreEqual(1, _environnement.CreatedItems);
			Assert.AreEqual(1, _environnement.DisposedItems);
		}

		[TestMethod]
		public void When_RemoveKey_Then_ValueIsDisposed()
		{
			var key = _environnement.AddStrong();

			TryCollect(key);

			Assert.IsTrue(key.IsAlive);
			Assert.AreEqual(1, _environnement.CreatedItems);
			Assert.AreEqual(0, _environnement.DisposedItems);

			Assert.IsTrue(_environnement.Remove(key));

			TryCollect(key);

			Assert.IsTrue(key.IsAlive);
			Assert.AreEqual(1, _environnement.CreatedItems);
			Assert.AreEqual(1, _environnement.DisposedItems);
		}

		private static void TryCollect(WeakReference<object> key)
		{
			for (var i = 0; i < 50 && key.IsAlive; i++)
			{
				GC.Collect(3, GCCollectionMode.Forced, true);
				GC.WaitForPendingFinalizers();
			}
		}

		private class Environnement
		{
			private readonly DisposableConditionalWeakTable<object, IDisposable> _sut = new DisposableConditionalWeakTable<object, IDisposable>();
			private readonly List<object> _items = new List<object>();

			public int DisposedItems;
			public int CreatedItems;

			public WeakReference<object> AddWeak()
			{
				var key = new object();
				CreatedItems++;
				_sut.Add(key, Disposable.Create(() => DisposedItems++));

				return new WeakReference<object>(key);
			}


			public WeakReference<object> AddStrong()
			{
				var key = new object();
				CreatedItems++;
				_sut.Add(key, Disposable.Create(() => DisposedItems++));

				_items.Add(key);

				return new WeakReference<object>(key);
			}

			public void ReleaseStrong(WeakReference<object> key)
			{
				if (!_items.Remove(key.GetTarget() ?? throw new NullReferenceException("Key was collected")))
				{
					throw new ArgumentOutOfRangeException(nameof(key));
				}
			}

			public bool Remove(WeakReference<object> key)
				=> _sut.Remove(key.GetTarget() ?? throw new NullReferenceException("Key was collected"));
		}
	}
}
