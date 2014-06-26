// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NUnit.Framework;

namespace Tp.PopEmailIntegration
{
	[TestFixture]
    [Category("PartPlugins0")]
	public class UtilsSpecs
	{
		[Test]
		public void TestFormatCommentFromOutlook()
		{
			string input =
				@"<div class=Section1><p class=MsoNormal><span style='color:#1F497D'>rtwwr<o:p></o:p></span></p>
                             <p class=MsoNormal><span style='color:#1F497D'><o:p>&nbsp;</o:p></span></p><div>
                             <div style='border:none;border-top:solid #B5C4DF 1.0pt;padding:3.0pt 0in 0in 0in'>
                             <p class=MsoNormal><b><span style='font-size:10.0pt;font-family:""Tahoma"",""sans-serif""'>From:</span></b><span
                             style='font-size:10.0pt;font-family:""Tahoma"",""sans-serif""'> Yury Amelchanka[mailto:yura@targetprocess.com] <br>
                             <b>Sent:</b> Thursday, January 29, 2009 4:24 PM<br><b>To:</b> 'Yury Amelchanka'<br>
                             <b>Subject:</b> RE: Test<o:p></o:p></span></p></div></div>";
			string output =
				@"<div class=Section1><p class=MsoNormal><span style='color:#1F497D'>rtwwr<o:p></o:p></span></p>
                             <p class=MsoNormal><span style='color:#1F497D'><o:p>&nbsp;</o:p></span></p>";
			Assert.AreEqual(Utils.FormatComment(input), output);
		}

		[Test]
		public void TestFormatCommentFromGmail()
		{
			string input =
				@"aaaaaaaaaa<br>ssssssssssssssssssss<br><br><div class=""gmail_quote"">On Thu, Jan 29, 2009 at 3:28 PM, Yury Amelchanka <span dir=""ltr"">&lt;<a href=""mailto:yura@targetprocess.com"">yura@targetprocess.com</a>&gt;</span> wrote:<br>
                            <blockquote class=""gmail_quote"" style=""border-left: 1px solid rgb(204, 204, 204); margin: 0pt 0pt 0pt 0.8ex; padding-left: 1ex;""><div link=""blue"" vlink=""purple"" lang=""EN-US"">
                            <div><p><span style=""font-size: 11pt; color: rgb(31, 73, 125);"">&nbsp;</span></p>
                            <p><span style=""font-size: 11pt; color: rgb(31, 73, 125);"">&nbsp;</span></p><div>";
			string output = @"aaaaaaaaaa<br>ssssssssssssssssssss";
			Assert.AreEqual(Utils.FormatComment(input), output);
		}
	}
}