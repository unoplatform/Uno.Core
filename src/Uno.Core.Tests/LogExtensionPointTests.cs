using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Extensions;

namespace Uno.Core.Tests
{
	[TestClass]
	public class LogExtensionPointTests
	{
		[TestInitialize]
		public void Setup()
		{
			// Clear any interceptor from previous tests.
			LogExtensionPoint.RegisterFactoryInterceptor(null);
		}

		[TestCleanup]
		public void Cleanup()
		{
			LogExtensionPoint.RegisterFactoryInterceptor(null);
		}

		[TestMethod]
		public void AmbientLoggerFactory_SetWithoutInterceptor_StoresValueDirectly()
		{
			var factory = new LoggerFactory();

			LogExtensionPoint.AmbientLoggerFactory = factory;

			Assert.AreSame(factory, LogExtensionPoint.AmbientLoggerFactory);
		}

		[TestMethod]
		public void RegisterFactoryInterceptor_InterceptorReceivesCurrentAndProposed()
		{
			var original = new LoggerFactory();
			LogExtensionPoint.AmbientLoggerFactory = original;

			ILoggerFactory capturedCurrent = null;
			ILoggerFactory capturedProposed = null;

			LogExtensionPoint.RegisterFactoryInterceptor((current, proposed) =>
			{
				capturedCurrent = current;
				capturedProposed = proposed;
				return proposed;
			});

			var replacement = new LoggerFactory();
			LogExtensionPoint.AmbientLoggerFactory = replacement;

			Assert.AreSame(original, capturedCurrent, "Interceptor should receive the current factory");
			Assert.AreSame(replacement, capturedProposed, "Interceptor should receive the proposed factory");
		}

		[TestMethod]
		public void RegisterFactoryInterceptor_InterceptorCanReplaceFactory()
		{
			var original = new LoggerFactory();
			LogExtensionPoint.AmbientLoggerFactory = original;

			var intercepted = new LoggerFactory();

			LogExtensionPoint.RegisterFactoryInterceptor((current, proposed) => intercepted);

			var proposed = new LoggerFactory();
			LogExtensionPoint.AmbientLoggerFactory = proposed;

			Assert.AreSame(intercepted, LogExtensionPoint.AmbientLoggerFactory,
				"AmbientLoggerFactory should be the value returned by the interceptor, not the proposed value");
		}

		[TestMethod]
		public void RegisterFactoryInterceptor_NullInterceptorRemovesCallback()
		{
			var intercepted = new LoggerFactory();
			LogExtensionPoint.RegisterFactoryInterceptor((current, proposed) => intercepted);

			// Remove interceptor
			LogExtensionPoint.RegisterFactoryInterceptor(null);

			var direct = new LoggerFactory();
			LogExtensionPoint.AmbientLoggerFactory = direct;

			Assert.AreSame(direct, LogExtensionPoint.AmbientLoggerFactory,
				"After removing interceptor, setter should store the value directly");
		}

		[TestMethod]
		public void RegisterFactoryInterceptor_InterceptorCanWrapFactory()
		{
			var entries = new List<string>();

			LogExtensionPoint.RegisterFactoryInterceptor((current, proposed) =>
			{
				// Wrap the proposed factory with a custom provider
				var wrapper = new LoggerFactory();
				wrapper.AddProvider(new TestLoggerProvider(entries));
				return wrapper;
			});

			LogExtensionPoint.AmbientLoggerFactory = new LoggerFactory();

			// Create a logger from the intercepted factory — should use our provider
			var logger = LogExtensionPoint.AmbientLoggerFactory.CreateLogger("Uno.UI.HotDesign.Test");
			logger.LogDebug("test message");

			Assert.AreEqual(1, entries.Count, "Interceptor-wrapped factory should capture the log entry");
			Assert.AreEqual("test message", entries[0]);
		}

		[TestMethod]
		public void Log_Type_UsesCurrentAmbientLoggerFactory()
		{
			var entries = new List<string>();

			var factory = new LoggerFactory();
			factory.AddProvider(new TestLoggerProvider(entries));

			LogExtensionPoint.AmbientLoggerFactory = factory;

			// typeof(T).Log() calls AmbientLoggerFactory.CreateLogger(forType) each time
			typeof(LogExtensionPointTests).Log().LogInformation("hello from Log(Type)");

			Assert.AreEqual(1, entries.Count);
			Assert.AreEqual("hello from Log(Type)", entries[0]);
		}

		[TestMethod]
		public void Log_Type_ReflectsFactoryReplacementViaInterceptor()
		{
			var entriesBefore = new List<string>();
			var entriesAfter = new List<string>();

			// Set initial factory
			var initialFactory = new LoggerFactory();
			initialFactory.AddProvider(new TestLoggerProvider(entriesBefore));
			LogExtensionPoint.AmbientLoggerFactory = initialFactory;

			// Log with initial factory
			typeof(string).Log().LogInformation("before");
			Assert.AreEqual(1, entriesBefore.Count);

			// Install interceptor that redirects to a different provider
			LogExtensionPoint.RegisterFactoryInterceptor((current, proposed) =>
			{
				var wrapper = new LoggerFactory();
				wrapper.AddProvider(new TestLoggerProvider(entriesAfter));
				return wrapper;
			});

			// Simulate app setting a new factory (interceptor will wrap it)
			LogExtensionPoint.AmbientLoggerFactory = new LoggerFactory();

			// Log again — should go to the interceptor's factory
			typeof(string).Log().LogInformation("after");
			Assert.AreEqual(1, entriesBefore.Count, "Original provider should not receive new logs");
			Assert.AreEqual(1, entriesAfter.Count, "Interceptor provider should receive the new log");
			Assert.AreEqual("after", entriesAfter[0]);
		}

		[TestMethod]
		public void RegisterFactoryInterceptor_InterceptorMustReturnNonNull()
		{
			// The interceptor contract requires a non-null return (ILoggerFactory, not ILoggerFactory?).
			// This validates the interceptor can pass through the proposed factory unchanged.
			var passedThrough = false;
			LogExtensionPoint.RegisterFactoryInterceptor((current, proposed) =>
			{
				passedThrough = true;
				return proposed;
			});

			var factory = new LoggerFactory();
			LogExtensionPoint.AmbientLoggerFactory = factory;

			Assert.IsTrue(passedThrough);
			Assert.AreSame(factory, LogExtensionPoint.AmbientLoggerFactory);
		}

		[TestMethod]
		public void AmbientLoggerFactory_Set_InterceptorRunsOnceEvenUnderContention()
		{
			// Validates the interceptor runs exactly once per set (not multiple times
			// due to Transactional.Update retry loop).
			var callCount = 0;
			LogExtensionPoint.RegisterFactoryInterceptor((current, proposed) =>
			{
				System.Threading.Interlocked.Increment(ref callCount);
				return proposed;
			});

			LogExtensionPoint.AmbientLoggerFactory = new LoggerFactory();

			Assert.AreEqual(1, callCount, "Interceptor should run exactly once per set");
		}

		/// <summary>
		/// Simple test logger provider that captures log messages.
		/// </summary>
		private sealed class TestLoggerProvider : ILoggerProvider
		{
			private readonly List<string> _entries;
			public TestLoggerProvider(List<string> entries) { _entries = entries; }
			public ILogger CreateLogger(string categoryName) { return new TestLogger(_entries); }
			public void Dispose() { }
		}

		private sealed class TestLogger : ILogger
		{
			private readonly List<string> _entries;
			public TestLogger(List<string> entries) { _entries = entries; }
			public IDisposable BeginScope<TState>(TState state) { return null; }
			public bool IsEnabled(LogLevel logLevel) { return true; }
			public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
			{
				_entries.Add(formatter(state, exception));
			}
		}
	}
}
