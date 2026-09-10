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

namespace Uno.Core.Tests.Conversions
{
	/// <summary>
	/// A minimal <see cref="IServiceProvider"/> whose registrations are resolved once
	/// and then cached, so a test can mutate the very instance the code under test uses.
	/// </summary>
	internal class TestServiceProvider : IServiceProvider
	{
		private readonly Dictionary<Type, Lazy<object>> _services = new Dictionary<Type, Lazy<object>>();

		public void Register<TService>(Func<TService> factory)
			where TService : class
			=> _services[typeof(TService)] = new Lazy<object>(() => factory());

		public TService Resolve<TService>()
			where TService : class
			=> (TService)GetService(typeof(TService));

		public object GetService(Type serviceType)
			=> _services.TryGetValue(serviceType, out var service) ? service.Value : null;
	}
}
