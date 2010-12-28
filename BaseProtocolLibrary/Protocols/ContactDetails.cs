using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InstantMessage
{
	public class Address
	{
		public Address(string street, string city, string state, string zipcode)
		{
			mStreet = street;
			mCity = city;
			mState = state;
			mZipCode = zipcode;
		}
		public override string ToString()
		{
			return mStreet + ", " + mCity + ", " + mState + " " + mZipCode;
		}
		private string mStreet = "";
		private string mCity = "";
		private string mState = "";
		private string mCountry = "";
		private string mZipCode = "";
	}
	public class PhoneNumber
	{
		public PhoneNumber(uint areaCode, uint part1, uint part2)
		{
			mAreaCode = areaCode;
			mPart1 = part1;
			mPart2 = part2;
		}
		public override string ToString()
		{
			return "+" + mCountyCode + " (" + mAreaCode + ") " + mPart1 + "-" + mPart2;
		}

		private uint mCountyCode = 1;
		private uint mAreaCode;
		private uint mPart1;
		private uint mPart2;
	}
	public class ContactDetails
	{
		private string mFirstName = "";
		private string mLastName = "";
		private string mMiddleName = "";
	}
}