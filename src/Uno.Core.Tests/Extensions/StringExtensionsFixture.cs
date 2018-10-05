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
using System.Globalization;
using System.Linq;
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
			Assert.IsTrue(((string)null).IsNullOrEmpty());
			Assert.IsTrue("".IsNullOrEmpty());
			Assert.IsFalse("A".IsNullOrEmpty());
		}

		[TestMethod]
		public void HasValue()
		{
			Assert.IsFalse(((string)null).HasValue());
			Assert.IsFalse("".HasValue());
			Assert.IsTrue("A".HasValue());
		}

		[TestMethod]
		public void IsNumber()
		{
			Assert.IsTrue("123".IsNumber());
			Assert.IsTrue("123 ".IsNumber());
			Assert.IsTrue(" 123".IsNumber());
			Assert.IsTrue("\t\t123".IsNumber());

			Assert.IsTrue("₀₁₂₃₄₅₆₇₈₉".IsNumber(), "subscripts");
			Assert.IsTrue("⁰¹²³⁴⁵⁶⁷⁸⁹".IsNumber(), "superscripts");

			Assert.IsTrue("¼½¾".IsNumber(), "fractions 1");
			Assert.IsTrue("⅓⅔⅕⅖⅗⅘⅙⅚⅛⅜⅝⅞".IsNumber(), "fractions 2");
			//Assert.IsTrue("⅐⅑⅒".IsNumber());  THOSE ARE NOT SUPPORTED BY .NET!

			// Roman numerals http://www.unicode.org/charts/PDF/U2150.pdf
			Assert.IsTrue("ⅠⅡⅢⅣⅤⅥⅦⅧⅨⅩⅪⅫⅬⅭⅮⅯ".IsNumber(), "romans upper"); // watch-out, those are not letters! ;-)
			Assert.IsTrue("ⅰⅱⅲⅳⅴⅵⅶⅷⅸⅹⅺⅻⅼⅽⅾⅿ".IsNumber(), "romans lower"); // watch-out, those are not letters! ;-)

			// Umbrella are not supporting Aegean numbers, as they are 32bits unicode characters

			Assert.IsTrue("٠١٢٣٤٥٦٧٨٩".IsNumber(), "Eastern Arabic numerals");
			Assert.IsTrue("۰۱۲۳۴۵۶۷۸۹".IsNumber(), "Persian numerals");

			Assert.IsFalse("1A2".IsNumber());
			Assert.IsFalse("1A2".IsNumber());
		}

		[TestMethod]
		public void IsDigit()
		{
			Assert.IsTrue("0123456789".IsDigit(), "Normal digits");
			Assert.IsTrue("٠١٢٣٤٥٦٧٨٩".IsDigit(), "Eastern Arabic numerals");
			Assert.IsTrue("۰۱۲۳۴۵۶۷۸۹".IsDigit(), "Persian numerals");

			Assert.IsTrue(" 123".IsDigit(), "space before");
			Assert.IsTrue("123 ".IsDigit(), "space after");
			Assert.IsTrue("\t\t123".IsDigit(), "tab before");

			Assert.IsFalse("₀₁₂₃₄₅₆₇₈₉".IsDigit());
			Assert.IsFalse("⁰¹²³⁴⁵⁶⁷⁸⁹".IsDigit());
			Assert.IsFalse("¼½¾".IsDigit());
			Assert.IsFalse("⅓⅔⅕⅖⅗⅘⅙⅚⅛⅜⅝⅞".IsDigit());
			Assert.IsFalse("⅐⅑⅒".IsDigit());
			Assert.IsFalse("ⅠⅡⅢⅣⅤⅥⅦⅧⅨⅩⅪⅫⅬⅭⅮⅯ".IsDigit()); // watch-out, those are not letters! ;-)
			Assert.IsFalse("ⅰⅱⅲⅳⅴⅵⅶⅷⅸⅹⅺⅻⅼⅽⅾⅿ".IsDigit()); // watch-out, those are not letters! ;-)

			// Full Width digits http://www.unicode.org/charts/PDF/UFF00.pdf
			var fullWidthDigits = "\uFF10\uFF11\uFF12\uFF13\uFF14\uFF15\uFF16\uFF17\uFF18\uFF19";
			Assert.IsTrue(fullWidthDigits.IsNumber(), "Full Width Digits");

			Assert.IsFalse("1A2".IsDigit());
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
				new CultureInfo("ru")
			};

			foreach (var format in formats)
			{
				foreach (var value in values)
				{
					Assert.AreEqual(
						string.Format(format, value),
						StringExtensions.Format(format, value));

					foreach (var culture in cultures)
					{
						Assert.AreEqual(
							string.Format(culture, format, value),
							StringExtensions.Format(culture, format, value));
					}
				}
			}
		}

		[TestMethod]
		public void StringFormatsAreComplementary()
		{
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
					Assert.AreEqual(
						string.Format(format, value),
						StringExtensions.Format(format, value));

					foreach (var culture in cultures)
					{
						Assert.AreEqual(
							string.Format(culture, format, value),
							StringExtensions.Format(culture, format, value));
					}
				}

				foreach (var value in differentValues)
				{
					Assert.AreNotEqual(
						string.Format(format, value),
						StringExtensions.Format(format, value));

					foreach (var culture in cultures)
					{
						Assert.AreNotEqual(
							string.Format(culture, format, value),
							StringExtensions.Format(culture, format, value));
					}
				}
			}
		}

		[TestMethod]
		public void CustomStringFormatIsTolerant()
		{
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
				Assert.AreEqual(StringExtensions.Format(pair.format, 42), pair.expected);
			}
		}

		[TestMethod]
		public void StringFormatHasEscapedChars()
		{
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

					Assert.AreEqual(original, custom);
				}
			}
		}

		[TestMethod]
		public void StringFormatsErrorHandlingAreIdentical()
		{
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

						Assert.AreEqual(original, custom);
					}
					catch
					{
						Assert.Fail($"string.Format did not fail for {format}. StringExtensions.Format shouldn't have failed either.");
					}
				}
				catch
				{
					try
					{
						StringExtensions.Format(format, 42);

						Assert.Fail($"string.Format failed for {format}. StringExtensions.Format should have failed too.");
					}
					catch
					{
					}
				}
			}
		}

		[TestMethod]
		public void CustomStringFormatsWithMultipleValues()
		{
			var previous = CultureInfo.CurrentUICulture;
			try
			{
				CultureInfo.CurrentUICulture = new CultureInfo("en-US", false);

				Assert.AreEqual(
					StringExtensions.Format("For {0} and {1}?", 42, 3.1416),
					"For 42 and 3.1416?");
				Assert.AreEqual(
					StringExtensions.Format("For {0:#.##;(#.##);ZERO} and {1:#.##;(#.##);ZERO}?", 42, 3.1416),
					"For 42 and 3.14?");
				Assert.AreEqual(
					StringExtensions.Format(
						"For {0:#.##;(#.##);ZERO;ONE}, {1:#.##;(#.##);ZERO;ONE} and {2:#.##;(#.##);ZERO;ONE}?", 42,
						3.1416, 1.004),
					"For 42, 3.14 and ONE?");
			}
			finally
			{
				CultureInfo.CurrentUICulture = previous;
			}
		}
	}
}
