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
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Uno.Extensions
{
	public static class StreamExtensions
	{
		private const int BUFFER_SIZE = 4096;

		public static Task<byte[]> ReadBytesAsync(this Stream stream) => stream.ReadBytesAsync(default);

		/// <summary>
		/// Read all bytes of a stream.
		/// </summary>
		/// <remarks>
		/// The stream won't be closed by this method.
		/// </remarks>
		public static async Task<byte[]> ReadBytesAsync(this Stream stream, CancellationToken ct)
		{
			long? length;
			long? startingPosition;

			try
			{
				length = stream.CanSeek ? stream.Length : (long?)null;
				startingPosition = stream.CanSeek ? stream.Position : (long?)null;
			}
			catch (NotSupportedException)
			{
				length = null;
				startingPosition = null;
			}

			if (length == 0 || (startingPosition.HasValue && startingPosition == length)) // empty stream
			{
				return new byte[] { };
			}

			if (stream is MemoryStream {Position: 0} memStream)
			{
				// MemoryStream.ToArray() is already optimized, so use it when possible
				return memStream.ToArray();
			}

			var readBuffer = new byte[length ?? BUFFER_SIZE];

			var totalBytesRead = 0;
			int bytesRead;

			while ((bytesRead = await stream.ReadAsync(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead, ct)) > 0)
			{
				totalBytesRead += bytesRead;

				if (totalBytesRead == readBuffer.Length)
				{
					var nextBytes = new byte[1];
					var read = await stream.ReadAsync(nextBytes, 0, 1, ct);

					if (read != 1)
					{
						continue;
					}

					var temp = new byte[readBuffer.Length * 2];
					Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
					Buffer.SetByte(temp, totalBytesRead, (byte)nextBytes[0]);
					readBuffer = temp;
					totalBytesRead++;
				}
			}

			var buffer = readBuffer;
			if (readBuffer.Length != totalBytesRead)
			{
				buffer = new byte[totalBytesRead];
				Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
			}
			return buffer;
		}

		/// <summary>
		/// Read all bytes of a stream.
		/// </summary>
		/// <remarks>
		/// The stream won't be closed by this method.
		/// </remarks>
		public static byte[] ReadBytes(this Stream stream)
		{
			long? length;
			long? startingPosition;

			try
			{
				length = stream.CanSeek ? stream.Length : (long?)null;
				startingPosition = stream.CanSeek ? stream.Position : (long?)null;
			}
			catch (NotSupportedException)
			{
				length = null;
				startingPosition = null;
			}

			if (length == 0 || (startingPosition.HasValue && startingPosition == length)) // empty stream
			{
				return new byte[] { };
			}

			if (stream is MemoryStream {Position: 0} memStream)
			{
				// MemoryStream.ToArray() is already optimized, so use it when possible
				return memStream.ToArray();
			}

			var readBuffer = new byte[length ?? BUFFER_SIZE];

			var totalBytesRead = 0;
			int bytesRead;

			while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
			{
				totalBytesRead += bytesRead;

				if (totalBytesRead == readBuffer.Length)
				{
					var nextByte = stream.ReadByte();
					if (nextByte == -1)
					{
						continue;
					}

					var temp = new byte[readBuffer.Length * 2];
					Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
					Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
					readBuffer = temp;
					totalBytesRead++;
				}
			}

			var buffer = readBuffer;
			if (readBuffer.Length != totalBytesRead)
			{
				buffer = new byte[totalBytesRead];
				Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
			}
			return buffer;
		}

		/// <summary>
		/// Reads the text container into the specified stream.
		/// </summary>
		/// <returns>The read string using the default encoding.</returns>
		/// <remarks>The stream will be disposed when calling this method.</remarks>
		public static string ReadToEnd(this Stream stream)
		{
			using var reader = new StreamReader(stream);
			return reader.ReadToEnd();
		}


		/// <summary>
		/// Reads the text container into the specified stream.
		/// </summary>
		/// <returns>The read string using the specified encoding.</returns>
		/// <remarks>The stream will be disposed when calling this method.</remarks>
		public static string ReadToEnd(this Stream stream, Encoding encoding)
		{
			using var reader = new StreamReader(stream, encoding);
			return reader.ReadToEnd();
		}

		/// <summary>
		/// Warning, if stream cannot be seek, will read from current position!
		/// Warning, stream position will not been restored!
		/// </summary>
		public static bool StartsWith(this Stream stream, params byte[] start)
		{
			if (start.Length == 0)
			{
				return true;
			}

			if (stream.CanSeek)
			{
				stream.Position = 0;
			}

			var buffer = new byte[start.Length];

			stream.Read(buffer, 0, buffer.Length);

			return start.SequenceEqual(buffer);
		}

		/// <summary>
		/// Warning, if stream cannot be seek, will read from current position!
		/// Warning, stream position will not been restored!
		/// </summary>
		public static async Task<bool> StartsWithAsync(this Stream stream, params byte[] start)
		{
			if (stream.CanSeek)
			{
				stream.Position = 0;
			}

			var buffer = new byte[start.Length];

			await stream.ReadAsync(buffer, 0, buffer.Length);

			return start.SequenceEqual(buffer);
		}

		/// <summary>
		/// Create a MemoryStream, copy <see cref="source"/> to it, and set position to 0.
		/// </summary>
		/// <param name="source">Stream to copy</param>
		/// <returns>Newly created memory stream, position set to 0</returns>
		public static MemoryStream ToMemoryStream(this Stream source)
		{
			if (source is MemoryStream memStream)
			{
				return memStream;
			}

			memStream = source.CanSeek
				? new MemoryStream((int)source.Length)
				: new MemoryStream();

			source.CopyTo(memStream);
			memStream.Position = 0;

			return memStream;
		}

		/// <summary>
		/// Check if <see cref="stream"/> is seekable (CanSeek), if not copy it to a MemoryStream. 
		/// WARNING: Some stream (like UnmanagedMemoryStream) return CanSeek = true but are not seekable. Prefer using ToMemoryStream() to be 100% safe.
		/// </summary>
		/// <param name="stream">A stream</param>
		/// <returns>A seekable stream (orginal if seekable, a MemoryStream copy of <see cref="stream"/> else)</returns>
		public static Stream ToSeekable(this Stream stream) => stream.CanSeek ? stream : stream.ToMemoryStream();
	}
}
