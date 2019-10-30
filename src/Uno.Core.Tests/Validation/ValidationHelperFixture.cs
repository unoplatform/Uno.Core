using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.Extensions;
using System.Globalization;
using Uno.Validation;

namespace Uno.Core.Validation
{
	[TestClass]
	public class ValidationHelperFixture
	{
		[TestMethod]
		public void EmailValidation()
		{
			string[] validEmails =
			{
				"me@example.com",
				"a.nonymous@example.com",
				"email@subdomain.example.com",
				"name+tag@example.com",
				"a.name+tag@example.com",
				"x@example.com",
				"1234567890@example.com",
				"email@example-one.com",
				"email@example.name",
				"email@example.museum",
				"email@example.co.jp",
				"a@a.a",
				"a@a.ca",
				"a.b@a.ca",
				"a.b@a.com",
				//some gTLD
				"a@Something.CATHOLIC",
				"a@Something.BOSTON",
				"a@Something.RMIT",
				// special characters and accent are not yet supported 
				//"a@xyz.kraków.pl",
				//"a@épingle.ca",
			};

			validEmails.ForEach(input => Assert.IsTrue(ValidationHelper.IsEmail(input)));

			string[] invalidEmails =
			{
				"Abc.example.com",       // At least one '@'
				"A@b@c@example.com",     // No more than one '@'
				"john..doe@example.com", // No consecutive dots for local part
				"john.doe@example..com", // No consecutive dots for domain part 
				".me@example.com",       // Dot character should not be first
				"me.@example.com",       // Dot character should not be last
				"  @example.com",        // Must have local part
				"@example.com",          // Must have local part
				"john@   .com",          // Must have domain part
				"john@test.  ",          // Must have domain part
				"jo hn@example.com",     // No un-quoted spaces in local part
				"john@exa mple.com",     // No un-quoted spaces in domain part
				null,
				string.Empty,
				"a@a",
				"a.ca"				
			};

			invalidEmails.ForEach(input => Assert.IsFalse(ValidationHelper.IsEmail(input)));
		}

		[TestMethod]
		public void CanadianPostalCodeValidation()
		{
			string[] invalidPostalCodes =
			{
				"",
				"10H 0H0",
				"H01 0H0",
				"H*H 0H0",
				"H*H?0H0",
				"H*H@0H0",
				"H0H@0H0",
				"H010H0",
				"6L6",
				"@8K5",
				"14J4",
				"J7C6",
				"9D5",
				" 8I3",
				" AF9HJ",
				"H7E7H7Q",
				" 9I1",
				"QH70G9",
				"Z1A 1A9", //Cannot start with DFIOQU WZ
				"W1A 1A9", //Cannot start with DFIOQU WZ
				"A1Q 1A9", //DFIOQU cannot be used at any letter position
				"90210", // US Zip code
			};

			Assert.IsTrue(ValidationHelper.IsCanadianPostalCode("A1A 1A9"));
			Assert.IsTrue(ValidationHelper.IsCanadianPostalCode("a1a 1a9"));
			Assert.IsTrue(ValidationHelper.IsCanadianPostalCode("a1a1a9"));

			Assert.IsFalse(ValidationHelper.IsCanadianPostalCode(null));
			Assert.IsFalse(ValidationHelper.IsCanadianPostalCode(string.Empty));
			invalidPostalCodes.ForEach(pc => Assert.IsFalse(ValidationHelper.IsCanadianPostalCode(pc)));
		}

		[TestMethod]
		public void ZipCodeValidation()
		{
			Assert.IsTrue(ValidationHelper.IsZipCode("90210"));
			Assert.IsTrue(ValidationHelper.IsZipCode("90210-1234"));

			Assert.IsFalse(ValidationHelper.IsZipCode(null));
			Assert.IsFalse(ValidationHelper.IsZipCode(string.Empty));
			Assert.IsFalse(ValidationHelper.IsZipCode("9021"));
			Assert.IsFalse(ValidationHelper.IsZipCode("90210-123"));
			Assert.IsFalse(ValidationHelper.IsZipCode("G1Q 1Q9"));
		}

		[TestMethod]
		public void UsStatesValidation()
		{
			//Lower case test
			Assert.IsTrue(ValidationHelper.IsUSState("al"));

			//All us states from https://en.wikipedia.org/wiki/List_of_U.S._state_abbreviations USPS column
			var usStates = new string[] {
				"AL",
				"AK",
				"AZ",
				"AR",
				"CA",
				"CO",
				"CT",
				"DE",
				"DC",
				"FL",
				"GA",
				"HI",
				"ID",
				"IL",
				"IN",
				"IA",
				"KS",
				"KY",
				"LA",
				"ME",
				"MD",
				"MA",
				"MI",
				"MN",
				"MS",
				"MO",
				"MT",
				"NE",
				"NV",
				"NH",
				"NJ",
				"NM",
				"NY",
				"NC",
				"ND",
				"OH",
				"OK",
				"OR",
				"PA",
				"RI",
				"SC",
				"SD",
				"TN",
				"TX",
				"UT",
				"VT",
				"VA",
				"WA",
				"WV",
				"WI",
				"WY"};
			usStates.ForEach(s => Assert.IsTrue(ValidationHelper.IsUSState(s)));

			//See above reference
			var usTerritories = new string[] {
				"AS",
				"GU",
				"MP",
				"PR",
				"VI"
			};
			usTerritories.ForEach(s => Assert.IsTrue(ValidationHelper.IsUSState(s, includeTerritories: true)));

			// See above reference
			var usMillitaryMailCodes = new string[] {
				"AA",
				"AE",
				"AP"
			};
			usMillitaryMailCodes.ForEach(s => Assert.IsTrue(ValidationHelper.IsUSState(s, includeMilitary: true)));

			Assert.IsFalse(ValidationHelper.IsUSState(null));
			Assert.IsFalse(ValidationHelper.IsUSState(string.Empty));
			Assert.IsFalse(ValidationHelper.IsUSState("ZZ"));
		}

		[TestMethod]
		public void USCAnadaPhoneValidation()
		{
			Assert.IsTrue(ValidationHelper.IsUSCanadaPhone("555-1234"));
			Assert.IsTrue(ValidationHelper.IsUSCanadaPhone("5551234"));
			Assert.IsTrue(ValidationHelper.IsUSCanadaPhone("1115551234"));
			Assert.IsTrue(ValidationHelper.IsUSCanadaPhone("111555-1234"));
			Assert.IsTrue(ValidationHelper.IsUSCanadaPhone("111-555-1234"));
			Assert.IsTrue(ValidationHelper.IsUSCanadaPhone("111 555-1234"));
			Assert.IsTrue(ValidationHelper.IsUSCanadaPhone("111 555 1234"));
			Assert.IsTrue(ValidationHelper.IsUSCanadaPhone("111 555.1234"));
			Assert.IsTrue(ValidationHelper.IsUSCanadaPhone("111.555.1234"));
			Assert.IsTrue(ValidationHelper.IsUSCanadaPhone("(111)555.1234"));
			Assert.IsTrue(ValidationHelper.IsUSCanadaPhone("1(111)555.1234"));
			Assert.IsTrue(ValidationHelper.IsUSCanadaPhone("+1(111)555.1234"));
			Assert.IsTrue(ValidationHelper.IsUSCanadaPhone("1-123-555-1234"));

			Assert.IsFalse(ValidationHelper.IsUSCanadaPhone(null));
			Assert.IsFalse(ValidationHelper.IsUSCanadaPhone(string.Empty));
			Assert.IsFalse(ValidationHelper.IsUSCanadaPhone("(11)555.1234"));
			Assert.IsFalse(ValidationHelper.IsUSCanadaPhone("11555.1234"));
			Assert.IsFalse(ValidationHelper.IsUSCanadaPhone("11 555 1234"));
			Assert.IsFalse(ValidationHelper.IsUSCanadaPhone("1_123-555-1234"));
		}

		[TestMethod]
		public void CurrencyValidation()
		{
			var culture = new CultureInfo("us-en");
			culture.NumberFormat.CurrencySymbol = "$";

			Assert.IsTrue(ValidationHelper.IsCurrency("$1.2", culture));
			Assert.IsTrue(ValidationHelper.IsCurrency("1,222.20$", culture));
			Assert.IsTrue(ValidationHelper.IsCurrency("$12", culture));
			Assert.IsTrue(ValidationHelper.IsCurrency("1.2", culture));

			Assert.IsTrue(ValidationHelper.IsCurrency("1,2", culture)); // ',' considered thousands seperator
			Assert.IsFalse(ValidationHelper.IsCurrency("1 2", culture)); // wrong thousands seperator

			culture = new CultureInfo("fr-fr");
			culture.NumberFormat.CurrencySymbol = "£";
			Assert.IsTrue(ValidationHelper.IsCurrency("£1,2", culture));
			Assert.IsTrue(ValidationHelper.IsCurrency("1 121,2", culture));

			Assert.IsFalse(ValidationHelper.IsCurrency(null));
			Assert.IsFalse(ValidationHelper.IsCurrency(string.Empty));
			Assert.IsFalse(ValidationHelper.IsCurrency("$1,2", culture));
			Assert.IsFalse(ValidationHelper.IsCurrency("1.2", culture)); //wrong decimal seperator
		}
	}
}
