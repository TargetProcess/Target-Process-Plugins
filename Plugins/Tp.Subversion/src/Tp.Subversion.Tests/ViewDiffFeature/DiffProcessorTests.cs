// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NUnit.Framework;
using Tp.SourceControl.Diff;
using Tp.Testing.Common.NUnit;

namespace Tp.Subversion.ViewDiffFeature
{
	[TestFixture]
	public class DiffProcessorTests
	{
		[Test]
		public void TestStringIsNotUpdated()
		{
			var processor = new DiffProcessor();
			var source = "123";
			var diff = "123";

			var result = processor.GetDiff(source, diff);

			var leftFirstString = result.LeftPan[0];
			leftFirstString.Action.Should(Be.EqualTo(DiffActionType.None));
			leftFirstString.Line.Should(Be.EqualTo(source));
			leftFirstString.LineNumber.Should(Be.EqualTo(0));

			var rightFirstString = result.RightPan[0];
			rightFirstString.Action.Should(Be.EqualTo(DiffActionType.None));
			rightFirstString.Line.Should(Be.EqualTo(diff));
			rightFirstString.LineNumber.Should(Be.EqualTo(0));
		}

		[Test]
		public void TestStringUpdate()
		{
			var processor = new DiffProcessor();
			var source = "123\r\n456";
			var diff = "123\r\n789";

			var result = processor.GetDiff(source, diff);

			result.LeftPan.Count.Should(Be.EqualTo(2));
			result.RightPan.Count.Should(Be.EqualTo(2));

			var leftSecondString = result.LeftPan[1];
			leftSecondString.Action.Should(Be.EqualTo(DiffActionType.Updated));
			leftSecondString.Line.Should(Be.EqualTo("456"));
			leftSecondString.LineNumber.Should(Be.EqualTo(1));

			var rightSecondString = result.RightPan[1];
			rightSecondString.Action.Should(Be.EqualTo(DiffActionType.Updated));
			rightSecondString.Line.Should(Be.EqualTo("789"));
			rightSecondString.LineNumber.Should(Be.EqualTo(1));
		}

		[Test]
		public void TestStringAdded()
		{
			var processor = new DiffProcessor();
			var source = "123";
			var diff = "123\r\n789";

			var result = processor.GetDiff(source, diff);

			result.LeftPan.Count.Should(Be.EqualTo(2));
			result.RightPan.Count.Should(Be.EqualTo(2));

			var leftSecondString = result.LeftPan[1];
			leftSecondString.Action.Should(Be.EqualTo(DiffActionType.None));
			leftSecondString.Line.Should(Be.Empty);
			leftSecondString.LineNumber.Should(Be.EqualTo(-1));

			var rightSecondString = result.RightPan[1];
			rightSecondString.Action.Should(Be.EqualTo(DiffActionType.Added));
			rightSecondString.Line.Should(Be.EqualTo("789"));
			rightSecondString.LineNumber.Should(Be.EqualTo(1));
		}

		[Test]
		public void TestStringDeleted()
		{
			var processor = new DiffProcessor();
			var source = "123\r\n456";
			var diff = "123";

			var result = processor.GetDiff(source, diff);

			result.LeftPan.Count.Should(Be.EqualTo(2));
			result.RightPan.Count.Should(Be.EqualTo(2));

			var leftSecondString = result.LeftPan[1];
			leftSecondString.Action.Should(Be.EqualTo(DiffActionType.Deleted));
			leftSecondString.Line.Should(Be.EqualTo("456"));
			leftSecondString.LineNumber.Should(Be.EqualTo(1));

			var rightSecondString = result.RightPan[1];
			rightSecondString.Action.Should(Be.EqualTo(DiffActionType.None));
			rightSecondString.Line.Should(Be.Empty);
			rightSecondString.LineNumber.Should(Be.EqualTo(-1));
		}
	}
}