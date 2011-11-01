using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NexusIM.Misc;
using System.Globalization;
using System.Diagnostics;
using InstantMessage;

namespace NexusIMTests
{
	[TestClass]
	public class RedBlackTests
	{
		private class Comparer : IComparer<string>
		{
			public int Compare(string x, string y)
			{
				compareCount++;
				return x.CompareTo(y);
			}

			public int compareCount;
		}

		private class TestCollection : AdvancedSet<string>
		{
			public TestCollection() : base(new Comparer())
			{

			}
			
			public bool Find(char find)
			{
				throw new NotImplementedException();
				string f = find.ToString();
				Comparer comp = (Comparer)Comparer;
				//return !String.IsNullOrEmpty(base.SearchNoStack(base.RootNode, (i) => {					
				//	comp.compareCount++;
				//	return i.CompareTo(f);
				//}));
			}
		}

		private void PopulateSet(AdvancedSet<string> set, int count)
		{
			for (char c = char.MinValue; c < count; c++)
			{
				set.Add(c.ToString());
			}
		}

		[TestMethod]
		public void PerfTest()
		{
			TestCollection set = new TestCollection();
			Comparer comp = (Comparer)set.Comparer;

			int items = 65535;
			Stopwatch watch = new Stopwatch();
			watch.Start();
			PopulateSet(set, items);
			comp.compareCount = 0;

			watch.Stop();

			Trace.WriteLine("Total Items: " + items.ToString());
			Trace.WriteLine("Insertion: " + watch.Elapsed.ToString());

			Random r = new Random();

			char search = char.MaxValue;
			//char search = (char)r.Next((int)char.MinValue, (int)char.MaxValue);

			watch.Restart();
			set.Contains(search.ToString());

			watch.Stop();

			Trace.WriteLine(String.Format("Binary Search: Time: {0}, Comparisons: {1}", watch.Elapsed, comp.compareCount));
			comp.compareCount = 0;

			watch.Restart();
			set.Find(search);

			watch.Stop();
			Trace.WriteLine(String.Format("Binary Search (Reflection): Time: {0}, Comparisons: {1}", watch.Elapsed, comp.compareCount));
			comp.compareCount = 0;

			watch.Restart();
			set.Contains(search.ToString());

			watch.Stop();

			Trace.WriteLine(String.Format("Full Search: Time: {0}, Comparisons: {1}", watch.Elapsed, comp.compareCount));
			comp.compareCount = 0;
		}
	}
}