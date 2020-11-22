// ******************************************************************
// Copyright � 2015-2020 nventive inc. All rights reserved.
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

using System.Linq;
using System;
using System.Collections.Generic;
using Uno.Extensions;

namespace Uno.Equality
{
	public class WeakReferenceEqualityComparer<T> : IEqualityComparer<System.WeakReference<T>>
		where T : class
	{
		public bool Equals(System.WeakReference<T> w1, T t2)
		{
			T t1;

			var r1 = w1.TryGetTarget(out t1);

			return (!r1 && t1 != null)
				|| t1.SafeEquals(t2);
		}


		public bool Equals(System.WeakReference<T> w1, System.WeakReference<T> w2)
		{
			T t1;
			T t2;

			var r1 = w1.TryGetTarget(out t1);
			var r2 = w2.TryGetTarget(out t2);

			return ((r1 && r2) || (!r1 && !r2))
				&& t1.SafeEquals(t2);
		}

		public int GetHashCode(System.WeakReference<T> w)
		{
			T t;

			return w.TryGetTarget(out t)
				? t.GetHashCode()
				: -1;
		}
	}
}
