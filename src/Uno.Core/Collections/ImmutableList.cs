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

namespace Uno.Collections
{
	/// <summary>
	/// An immutable list implementation, designed for safe concurrent access.
	/// </summary>
	public class ImmutableList<T>
	{
		private T[] data;
		private static ImmutableList<T> _empty = new ImmutableList<T>();

		/// <summary>
		/// Creates an empty list
		/// </summary>
		public ImmutableList()
		{
			data = new T[0];
		}

		/// <summary>
		/// Provides an empty list
		/// </summary>
		public static ImmutableList<T> Empty {
			get { return _empty; }
		}

		/// <summary>
		/// Initializes the list with the provided array.
		/// </summary>
		/// <param name="data"></param>
		public ImmutableList(T[] data)
		{
			this.data = data;
		}

		/// <summary>
		/// Returns a new list with the specifed value appended at the end.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public ImmutableList<T> Add(T value)
		{
			var newData = new T[data.Length + 1];
			Array.Copy(data, newData, data.Length);
			newData[data.Length] = value;
			return new ImmutableList<T>(newData);
		}

		/// <summary>
		/// Returns a new list with specified value removed.
		/// </summary>
		/// <param name="value">The value to remove</param>
		/// <returns>A new list</returns>
		public ImmutableList<T> Remove(T value)
		{
			var i = IndexOf(value);
			if (i < 0)
				return this;
			return RemoveAt(i);
		}


		/// <summary>
		/// Determines whether the list contains a specified element
		/// </summary>
		/// <param name="value">The value to locate.</param>
		/// <returns></returns>
		public bool Contains(T value)
		{
			return data.Contains(value);
		}

		/// <summary>
		/// Removes the item at the specified index.
		/// </summary>
		/// <param name="i">The index to remove</param>
		/// <returns>A new list with the item removed</returns>
		public ImmutableList<T> RemoveAt(int i)
		{
			if(data.Length == 0)
			{
				throw new InvalidOperationException("The list is empty");
			}

			var newData = new T[data.Length - 1];
			Array.Copy(data, 0, newData, 0, i);
			Array.Copy(data, i + 1, newData, i, data.Length - i - 1);
			return new ImmutableList<T>(newData);
		}

		/// <summary>
		/// Returns the index of the specified value
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public int IndexOf(T value)
		{
			for (var i = 0; i < data.Length; ++i)
				if (data[i].Equals(value))
					return i;
			return -1;
		}

		/// <summary>
		/// The underlying data available for thread-safe access
		/// </summary>
		public T[] Data
		{
			get { return data; }
		}
	}
}
