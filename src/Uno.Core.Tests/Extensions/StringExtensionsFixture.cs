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
using System.Globalization;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Extensions;

namespace Uno.Tests.Extensions
{
	[TestClass]
	public class StringExtensionsFixture
	{
		[TestMethod]
		public void IsNullOrEmpty()
		{
			using var _ = new AssertionScope();

			((string)null).IsNullOrEmpty().Should().BeTrue();
			"".IsNullOrEmpty().Should().BeTrue();
			string.Empty.IsNullOrEmpty().Should().BeTrue();
			"A".IsNullOrEmpty().Should().BeFalse();
		}

		[TestMethod]
		public void HasValue()
		{
			using var _ = new AssertionScope();

			((string)null).HasValue().Should().BeFalse();
			"".HasValue().Should().BeFalse();
			string.Empty.HasValue().Should().BeFalse();
			"A".HasValue().Should().BeTrue();
		}

		[TestMethod]
		public void IsNumber()
		{
			using var _ = new AssertionScope();

			"123".IsNumber().Should().BeTrue();
			"123 ".IsNumber().Should().BeTrue();
			" 123".IsNumber().Should().BeTrue();
			"\t\t123".IsNumber().Should().BeTrue();

			"₀₁₂₃₄₅₆₇₈₉".IsNumber().Should().BeTrue("subscripts");
			"⁰¹²³⁴⁵⁶⁷⁸⁹".IsNumber().Should().BeTrue("superscripts");

			"¼½¾".IsNumber().Should().BeTrue("fractions 1");
			"⅓⅔⅕⅖⅗⅘⅙⅚⅛⅜⅝⅞".IsNumber().Should().BeTrue("fractions 2");
			//Assert.IsTrue("⅐⅑⅒".IsNumber());  THOSE ARE NOT SUPPORTED BY .NET!

			// Roman numerals http://www.unicode.org/charts/PDF/U2150.pdf
			"ⅠⅡⅢⅣⅤⅥⅦⅧⅨⅩⅪⅫⅬⅭⅮⅯ".IsNumber().Should().BeTrue("romans upper"); // watch-out, those are not letters! ;-)
			"ⅰⅱⅲⅳⅴⅵⅶⅷⅸⅹⅺⅻⅼⅽⅾⅿ".IsNumber().Should().BeTrue("romans lower"); // watch-out, those are not letters! ;-)

			// Umbrella are not supporting Aegean numbers, as they are 32bits unicode characters

			"٠١٢٣٤٥٦٧٨٩".IsNumber().Should().BeTrue("Eastern Arabic numerals");
			"۰۱۲۳۴۵۶۷۸۹".IsNumber().Should().BeTrue("Persian numerals");

			"1A2".IsNumber().Should().BeFalse();
			"1A2".IsNumber().Should().BeFalse();
		}

		[TestMethod]
		public void IsDigit()
		{
			using var _ = new AssertionScope();

			"0123456789".IsDigit().Should().BeTrue("Normal digits");
			"٠١٢٣٤٥٦٧٨٩".IsDigit().Should().BeTrue("Eastern Arabic numerals");
			"۰۱۲۳۴۵۶۷۸۹".IsDigit().Should().BeTrue("Persian numerals");

			" 123".IsDigit().Should().BeTrue("space before");
			"123 ".IsDigit().Should().BeTrue("space after");
			"\t\t123".IsDigit().Should().BeTrue("tab before");

			"₀₁₂₃₄₅₆₇₈₉".IsDigit().Should().BeFalse();
			"⁰¹²³⁴⁵⁶⁷⁸⁹".IsDigit().Should().BeFalse();
			"¼½¾".IsDigit().Should().BeFalse();
			"⅓⅔⅕⅖⅗⅘⅙⅚⅛⅜⅝⅞".IsDigit().Should().BeFalse();
			"⅐⅑⅒".IsDigit().Should().BeFalse();
			"ⅠⅡⅢⅣⅤⅥⅦⅧⅨⅩⅪⅫⅬⅭⅮⅯ".IsDigit().Should().BeFalse(); // watch-out, those are not letters! ;-)
			"ⅰⅱⅲⅳⅴⅵⅶⅷⅸⅹⅺⅻⅼⅽⅾⅿ".IsDigit().Should().BeFalse(); // watch-out, those are not letters! ;-)

			// Full Width digits http://www.unicode.org/charts/PDF/UFF00.pdf
			var fullWidthDigits = "\uFF10\uFF11\uFF12\uFF13\uFF14\uFF15\uFF16\uFF17\uFF18\uFF19";
			fullWidthDigits.IsNumber().Should().BeTrue("Full Width Digits");

			"1A2".IsDigit().Should().BeFalse();
		}

		[TestMethod]
		public void CustomStringFormatAcceptsOneAndMinusOne()
		{
			var ones = new[] { 0.95, 1, 1.00001, 1.04 };

			var cultures = new[]
			{
				CultureInfo.InvariantCulture,
				new CultureInfo("fr-CA"),
				new CultureInfo("en-US"),
				new CultureInfo("ru")
			};

			foreach (var culture in cultures)
			{
				foreach (var value in ones)
				{
					Assert.AreEqual(
						StringExtensions.Format(culture, @"{0:#.#;(#.#);ZERO;ONE;MINUS ONE}", value),
						"ONE");
					Assert.AreEqual(
						StringExtensions.Format(culture, @"{0:#.#;(#.#);ZERO;ONE;MINUS ONE}", -value),
						"-MINUS ONE"); // Can't prevent injection of the minus sign for now.
				}
			}
		}


		[TestMethod]
		public void StringFormatsAreCompatible()
		{
			using var _ = new AssertionScope();

			var formats = new[]
			{
				"{0:+#.#;-#.#;ZERO}",
				"{0:C;C;ZERO}",
			};

			var values = new[] { -5.55, -4.44, -1.05, -1.04, -1, -0.95, -0.949, -0.05, -0.049, 0, 0.049, 0.05, 0.949, 0.95, 1, 1.04, 1.05, 3, 42.42 };

			var cultures = new[]
			{
				CultureInfo.InvariantCulture,
				new CultureInfo("fr-CA"),
				new CultureInfo("en-US"),
				new CultureInfo("ja-JP"),
				new CultureInfo("ru")
			};

			foreach (var format in formats)
			{
				foreach (var value in values)
				{
					var noCultureFormat = StringExtensions.Format(format, value);
					noCultureFormat.Should().Be(string.Format(format, value));

					foreach (var culture in cultures)
					{
						var cultureFormat = StringExtensions.Format(culture, format, value);
						cultureFormat.Should().Be(string.Format(culture, format, value));
					}
				}
			}
		}

		[TestMethod]
		public void StringFormatsAreComplementary()
		{
			using var _ = new AssertionScope();

			var formats = new[]
			{
				"{0:+#.##;-#.##;ZERO;ONE;MINUS ONE}",
				"{0:C;C;ZERO;ONE;MINUS ONE}",
			};

			var identicalValues = new[] { -5.55, -4.44, -1.005, /*-1.004, -1, -0.995,*/ -0.9949, -0.005, -0.0049, 0, 0.0049, 0.005, 0.9949, /*0.995, 1, 1.004,*/ 1.005, 3, 42.42 };
			var differentValues = new[] { /*-5.55, -4.44, -1.005,*/ -1.004, -1, -0.995, /*-0.9949, -0.005, -0.0049, 0, 0.0049, 0.005, 0.9949,*/ 0.995, 1, 1.004/*, 1.005, 3, 42.42*/ };

			var cultures = new[]
			{
				CultureInfo.InvariantCulture,
				new CultureInfo("fr-CA"),
				new CultureInfo("en-US"),
				new CultureInfo("ru")
			};

			foreach (var format in formats)
			{
				foreach (var value in identicalValues)
				{
					var identicalValueFormat = StringExtensions.Format(format, value);
					identicalValueFormat.Should().Be(string.Format(format, value), "using current culture");

					foreach (var culture in cultures)
					{
						var identicalValueFormatWithCulture = StringExtensions.Format(culture, format, value);
						identicalValueFormatWithCulture.Should().Be(string.Format(culture, format, value), "using culture " + culture);
					}
				}

				foreach (var value in differentValues)
				{
					var differentValueFormat = StringExtensions.Format(format, value);
					differentValueFormat.Should().NotBe(string.Format(format, value), "using current culture");

					foreach (var culture in cultures)
					{
						var differenceValueFOrmatWithCulture = StringExtensions.Format(culture, format, value);
						differenceValueFOrmatWithCulture
							.Should().NotBe(string.Format(culture, format, value), "using culture " + culture);
					}
				}
			}
		}

		[TestMethod]
		public void CustomStringFormatIsTolerant()
		{
			using var _ = new AssertionScope();

			var formats = new[]
			{
				"This has no value",
				"This is the value: {0}",
				"{0} is the value",
				"The value {0} is nice",
				"{0}{0}",
				"Twice {0}{0}",
				"{0} and {0}",
				"We have {0} and {0}!!!"
			};

			var expectedResults = new[]
			{
				"This has no value",
				"This is the value: 42",
				"42 is the value",
				"The value 42 is nice",
				"4242",
				"Twice 4242",
				"42 and 42",
				"We have 42 and 42!!!"
			};

			foreach (var pair in formats.Zip(expectedResults, (format, expected) => new { format, expected }))
			{
				StringExtensions.Format(pair.format, 42).Should().Be(pair.expected);
			}
		}

		[TestMethod]
		public void StringFormatHasEscapedChars()
		{
			using var _ = new AssertionScope();

			var formats = new[]
			{
				@"{0:\;#.##\;;(#.##);\;\-\-\;;\'XXX\'}",
				@"{0:{{#.##}};{{(#.##)}};{{000}};{{XXX}}}",
				@"{0:\\#.##\\;\\(#.##)\\;\\000\\;\\XXX\\}"
			};

			var valuesWithoutOnes = new[] { -55.99, -1.1, 0, 0.9, 42.42 };

			foreach (var format in formats)
			{
				foreach (var value in valuesWithoutOnes)
				{
					var original = string.Format(format, value);
					var custom = StringExtensions.Format(format, value);

					custom.Should().Be(original);
				}
			}
		}

		[TestMethod]
		public void StringFormatsErrorHandlingAreIdentical()
		{
			using var _ = new AssertionScope();

			var formats = new[]
			{
				@"{0:#.##;(#.##);ZERO",
				@"{0:#.##;{#.##};ZERO}",
				@"{}",
				@"bad } I mean {0}"
			};

			foreach (var format in formats)
			{
				try
				{
					var original = string.Format(format, 42);

					try
					{
						var custom = StringExtensions.Format(format, 42);

						custom.Should().Be(original);
					}
					catch
					{
						AssertionScope.Current.FailWith("string.Format did not fail for {0}. StringExtensions.Format shouldn't have failed either.", format);
					}
				}
				catch(Exception originalEx)
				{
					try
					{
						StringExtensions.Format(format, 42);

						AssertionScope.Current.FailWith("string.Format failed for {0}. StringExtensions.Format should have failed too.", format);
					}
					catch(Exception exceptionEx)
					{
						exceptionEx.Should().BeOfType(originalEx.GetType());
					}
				}
			}
		}

		[TestMethod]
		public void CustomStringFormatsWithMultipleValues()
		{
			using var _ = new AssertionScope();

			StringExtensions
				.Format("For {0} and {1}?", 42, 3.1416)
				.Should().Be("For 42 and 3.1416?");
			StringExtensions
				.Format("For {0:#.##;(#.##);ZERO} and {1:#.##;(#.##);ZERO}?", 42, 3.1416)
				.Should().Be("For 42 and 3.14?");
			StringExtensions
				.Format("For {0:#.##;(#.##);ZERO;ONE}, {1:#.##;(#.##);ZERO;ONE} and {2:#.##;(#.##);ZERO;ONE}?", 42, 3.1416, 1.004)
				.Should().Be("For 42, 3.14 and ONE?");
		}
	}
}
