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
using Funq;
using CommonServiceLocator;

namespace Uno.Core.Tests.Conversions
{
	internal class FunqAdapter : IServiceLocator
	{
		private readonly Container _container;

		public FunqAdapter(Container container)
		{
			_container = container;
		}

		public IEnumerable<object> GetAllInstances(Type serviceType) => throw new NotSupportedException();
		public IEnumerable<TService> GetAllInstances<TService>() => new[] { _container.Resolve<TService>() };
		public object GetInstance(Type serviceType) => throw new NotSupportedException();
		public object GetInstance(Type serviceType, string key) => throw new NotSupportedException();
		public TService GetInstance<TService>() => _container.Resolve<TService>();
		public TService GetInstance<TService>(string key) => _container.ResolveNamed<TService>(key);
		public object GetService(Type serviceType) => throw new NotSupportedException();
	}
}
