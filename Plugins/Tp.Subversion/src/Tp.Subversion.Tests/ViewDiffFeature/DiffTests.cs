// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Tp.SourceControl.Diff;
using Tp.Testing.Common.NUnit;

namespace Tp.Subversion.ViewDiffFeature
{
	[TestFixture]
    [Category("PartPlugins1")]
	public class DiffTests
	{
		[Test]
		public void TestAllChanges()
		{
			var a = "a,b,c,d,e,f,g,h,i,j,k,l".Replace(',', '\n');
			var b = "0,1,2,3,4,5,6,7,8,9".Replace(',', '\n');

			ConvertToString(Diff.DiffText(a, b, false, false, false)).Should(Be.EqualTo("12.10.0.0*"), "ConvertToString(Diff.DiffText(a, b, false, false, false)).Should(Be.EqualTo(\"12.10.0.0*\"))");
		}

		[Test]
		public void TestAllSame()
		{
			var a = "a,b,c,d,e,f,g,h,i,j,k,l".Replace(',', '\n');
			var b = a;

			ConvertToString(Diff.DiffText(a, b, false, false, false)).Should(Be.Empty, "ConvertToString(Diff.DiffText(a, b, false, false, false)).Should(Be.Empty)");
		}

		[Test]
		public void TestSnake()
		{
			var a = "a,b,c,d,e,f".Replace(',', '\n');
			var b = "b,c,d,e,f,x".Replace(',', '\n');

			ConvertToString(Diff.DiffText(a, b, false, false, false)).Should(Be.EqualTo("1.0.0.0*0.1.6.5*"), "ConvertToString(Diff.DiffText(a, b, false, false, false)).Should(Be.EqualTo(\"1.0.0.0*0.1.6.5*\"))");
		}

		[Test]
		public void TestRepro20020920()
		{
			var a = "c1,a,c2,b,c,d,e,g,h,i,j,c3,k,l".Replace(',', '\n');
			var b = "C1,a,C2,b,c,d,e,I1,e,g,h,i,j,C3,k,I2,l".Replace(',', '\n');

			ConvertToString(Diff.DiffText(a, b, false, false, false)).Should(Be.EqualTo("1.1.0.0*1.1.2.2*0.2.7.7*1.1.11.13*0.1.13.15*"), "ConvertToString(Diff.DiffText(a, b, false, false, false)).Should(Be.EqualTo(\"1.1.0.0*1.1.2.2*0.2.7.7*1.1.11.13*0.1.13.15*\"))");
		}

		[Test]
		public void TestRepro20030207()
		{
			var a = "F".Replace(',', '\n');
			var b = "0,F,1,2,3,4,5,6,7".Replace(',', '\n');

			ConvertToString(Diff.DiffText(a, b, false, false, false)).Should(Be.EqualTo("0.1.0.0*0.7.1.2*"), "ConvertToString(Diff.DiffText(a, b, false, false, false)).Should(Be.EqualTo(\"0.1.0.0*0.7.1.2*\"))");
		}

		[Test]
		public void TestRepro20030409()
		{
			var a = "HELLO\nWORLD";
			var b = "\n\nhello\n\n\n\nworld\n";

			ConvertToString(Diff.DiffText(a, b, false, false, false)).Should(Be.EqualTo("2.8.0.0*"), "ConvertToString(Diff.DiffText(a, b, false, false, false)).Should(Be.EqualTo(\"2.8.0.0*\"))");
		}

		[Test]
		public void TestSomeDifferences()
		{
			var a = "a,b,-,c,d,e,f,f".Replace(',', '\n');
			var b = "a,b,x,c,e,f".Replace(',', '\n');

			ConvertToString(Diff.DiffText(a, b, false, false, false)).Should(Be.EqualTo("1.1.2.2*1.0.4.4*1.0.6.5*"), "ConvertToString(Diff.DiffText(a, b, false, false, false)).Should(Be.EqualTo(\"1.1.2.2*1.0.4.4*1.0.6.5*\"))");
		}

		private static string ConvertToString(IList<Diff.Item> f)
		{
			var ret = new StringBuilder();

			for (var n = 0; n < f.Count; n++)
			{
				ret.Append(f[n].deletedA + "." + f[n].insertedB + "." + f[n].StartA + "." + f[n].StartB + "*");
			}

			return (ret.ToString());
		}
	}
}