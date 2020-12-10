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

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Extensions;

namespace Uno.Core.Tests.Extensions
{
	[TestClass]
	public class StreamExtensionsFixture
	{
		[TestMethod]
		public void ReadBytes_MemoryStream()
		{
			var (sut, originalBytes) = CreateStream(2000);

			var readBytes1 = sut.ReadBytes();

			readBytes1.Should().Equal(originalBytes);
		}

		[TestMethod]
		public async Task ReadBytesAsync_MemoryStream()
		{
			var (sut, originalBytes) = CreateStream(2000);

			var readBytes1 = await sut.ReadBytesAsync();

			readBytes1.Should().Equal(originalBytes);
		}

		[TestMethod]
		public void ReadBytes_UnseekableSyncStream()
		{
			var (stream, originalBytes) = CreateStream(50000);
			var sut = new UnseekableSyncStream(stream);

			var readBytes1 = sut.ReadBytes();

			readBytes1.Should().Equal(originalBytes);
		}

		[TestMethod]
		public void ReadBytes_UnseekableSyncStream2()
		{
			var (stream, originalBytes) = CreateStream(50000);
			var sut = new UnseekableSyncStream(stream, true);

			var readBytes1 = sut.ReadBytes();

			readBytes1.Should().Equal(originalBytes);
		}

		[TestMethod]
		public async Task ReadBytesAsync_UnseekableSyncStream()
		{
			var (stream, originalBytes) = CreateStream(50000);
			var sut = new UnseekableSyncStream(stream);

			var readBytes1 = await sut.ReadBytesAsync();

			readBytes1.Should().Equal(originalBytes);
		}

		[TestMethod]
		public async Task ReadBytesAsync_UnseekableSyncStream2()
		{
			var (stream, originalBytes) = CreateStream(50000);
			var sut = new UnseekableSyncStream(stream, true);

			var readBytes1 = await sut.ReadBytesAsync();

			readBytes1.Should().Equal(originalBytes);
		}

		[TestMethod]
		public void ReadBytes_NoLengthStream()
		{
			var (sut, _) = CreateStream(0);
			sut.Length.Should().Be(0);

			var read = sut.ReadBytes();
			read.Should().BeEmpty();
		}

		[TestMethod]
		public async Task ReadBytesAsync_NoLengthStream()
		{
			var (sut, _) = CreateStream(0);
			sut.Length.Should().Be(0);

			var read = await sut.ReadBytesAsync();
			read.Should().BeEmpty();
		}

		[TestMethod]
		public void ReadBytes_NoLengthStream_UnseekableSyncStream()
		{
			var (stream, _) = CreateStream(0);
			var sut = new UnseekableSyncStream(stream);

			var read = sut.ReadBytes();
			read.Should().BeEmpty();
		}

		[TestMethod]
		public async Task ReadBytesAsync_NoLengthStream_UnseekableSyncStream()
		{
			var (stream, _) = CreateStream(0);
			var sut = new UnseekableSyncStream(stream);

			var read = await sut.ReadBytesAsync();
			read.Should().BeEmpty();
		}

		[TestMethod]
		public async Task StartWith()
		{
			var (sut, _) = CreateStream(100);

			sut.StartsWith(1, 2, 3, 4, 5).Should().BeTrue();
		}

		[TestMethod]
		public void StartWith_Empty()
		{
			var (sut, _) = CreateStream(0);

			sut.StartsWith(42, 42, 42, 42, 42).Should().BeFalse();
		}

		[TestMethod]
		public void StartWith_Empty2()
		{
			var (sut, _) = CreateStream(10);

			sut.StartsWith().Should().BeTrue();
		}

		[TestMethod]
		public async Task StartWithAsync_Empty()
		{
			var (sut, _) = CreateStream(0);

			(await sut.StartsWithAsync(42, 42, 42, 42, 42)).Should().BeFalse();
		}

		[TestMethod]
		public async Task StartWithasync_Empty2()
		{
			var (sut, _) = CreateStream(10);

			(await sut.StartsWithAsync()).Should().BeTrue();
		}

		[TestMethod]
		public async Task StartWithAsync()
		{
			var (sut, _) = CreateStream(100);

			sut.StartsWith(1, 2, 3, 4, 5).Should().BeTrue();
		}

		[TestMethod]
		public async Task ReadBytesAsync_NoLengthStream_UnseekableSyncStream2()
		{
			var (stream, _) = CreateStream(0);
			var sut = new UnseekableSyncStream(stream);

			var read = await sut.ReadBytesAsync();
			read.Should().BeEmpty();
		}

		[TestMethod]
		public async Task ToSeekable()
		{
			var (stream, expected) = CreateStream(int.MaxValue/50);
			var unseekable = new UnseekableSyncStream(stream);
			unseekable.CanSeek.Should().BeFalse();

			var sut = unseekable.ToSeekable();

			sut.CanSeek.Should().BeTrue();
			(await sut.ReadBytesAsync()).Should().Equal(expected);
		}

		private static (MemoryStream Stream, byte[] bytes) CreateStream(int length = 2000)
		{
			var bytes = Enumerable
				.Range(1, length)
				.Select(i =>
				{
					unchecked
					{
						return (byte) i;
					}
				})
				.ToArray();

			var stream = new MemoryStream(bytes);
			return (stream, bytes);
		}

		private class UnseekableSyncStream : Stream
		{
			private readonly Stream _stream;

			public UnseekableSyncStream(Stream stream, bool canSeek = false)
			{
				_stream = stream;
				CanSeek = canSeek;
			}

			public override void Flush()
			{
			}

			public override long Seek(long offset, SeekOrigin origin)
			{
				throw new NotSupportedException();
			}

			public override void SetLength(long value)
			{
				throw new NotSupportedException();
			}

			public override int Read(byte[] buffer, int offset, int count)
			{
				var newCount = count > 2 ? count / 2 : count;
				return _stream.Read(buffer, offset, newCount);
			}

			public override void Write(byte[] buffer, int offset, int count)
			{
				throw new NotSupportedException();
			}

			public override bool CanRead => true;
			public override bool CanSeek { get; }

			public override bool CanWrite => false;

			public override long Length => throw new NotSupportedException();

			public override long Position
			{
				get => throw new NotSupportedException();
				set => throw new NotSupportedException();
			}
		}
	}
}
