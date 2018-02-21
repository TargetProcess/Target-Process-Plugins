// 
// Copyright (c) 2005-2017 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NUnit.Framework;

namespace Tp.PopEmailIntegration
{
    [TestFixture]
    [Category("PartPlugins1")]
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
        public void TestFormatCommentFromOutlookInRussian()
        {
            string input =
                @"<div class=Section1><p class=MsoNormal><span style='color:#1F497D'>rtwwr<o:p></o:p></span></p>
                             <p class=MsoNormal><span style='color:#1F497D'><o:p>&nbsp;</o:p></span></p><div>
                             <div style='border:none;border-top:solid #B5C4DF 1.0pt;padding:3.0pt 0in 0in 0in'>
                             <p class=MsoNormal><b><span style='font-size:10.0pt;font-family:""Tahoma"",""sans-serif""'>От:</span></b><span
                             style='font-size:10.0pt;font-family:""Tahoma"",""sans-serif""'> Yury Amelchanka[mailto:yura@targetprocess.com] <br>
                             <b>Отправлено:</b> Thursday, January 29, 2009 4:24 PM<br><b>Кому:</b> 'Yury Amelchanka'<br>
                             <b>Тема:</b> Ответ: Test<o:p></o:p></span></p></div></div>";
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

        [Test]
        public void TestFormatCommentFromGmail2()
        {
            string input =
                @"<div dir=""ltr"">This is a reply from Gmail</div><div class=""gmail_extra""><br><div class=""gmail_quote"">On Wed, Sep 20, 2017 at 3:57 PM,  <span dir=""ltr"">&lt;<a href=""mailto:skaskiv.targetprocess@gmail.com"" target=""_blank"">skaskiv.targetprocess@gmail.com</a>&gt;</span> wrote:<br><blockquote class=""gmail_quote"" style=""margin:0 0 0 .8ex;border-left:1px #ccc solid;padding-left:1ex"">







<div style=""padding-left:0;padding-top:0;padding-right:0;padding-bottom:0"">

    <table width=""100%"" align=""center"" bgcolor=""#ffffff"" cellpadding=""0"" cellspacing=""0"" border=""0"" style=""width:100%;margin-left:0;margin-right:0;margin-top:0;margin-bottom:0;font-family:Verdana,sans-serif;font-weight:400;font-style:normal;font-size:14px"">

        <tbody>

            <tr>

                <td align=""left"" valign=""top"" style=""margin:0;padding-left:0;padding-right:10%;padding-top:13px;padding-bottom:11px"">

                    <p style=""font-family:Verdana,sans-serif;font-style:normal;font-size:14px;color:#333333;padding-bottom:0;padding-top:0;margin-top:0;margin-bottom:10px"">

                        <b>New Comment added to Request #164 &quot;THIS IS FIX&quot;</b> in project <b>Test Project</b>                                                </p>

                    <div style=""font-family:Verdana,sans-serif;font-size:14px"">

                                                    <div>This is a test from AV</div>



                                            </div>

                    <p style=""font-family:Verdana,sans-serif;font-style:normal;font-size:12px;color:#808080;padding-bottom:0;padding-top:0;margin-top:5px;margin-bottom:10px"">

                        Added at 03:55 PM by Administrator Administrator                    </p>

                </td>

            </tr>

            <tr>

                <td align=""left"" valign=""top"" style=""padding-left:0;padding-right:0;padding-top:15px;padding-bottom:13px;border-top:1px solid #e6e6e6;font-family:Verdana,sans-serif;font-weight:400;font-style:normal;font-size:12px"">

                    <img width=""112"" height=""20"" src=""http://www.targetprocess.com/img/email/you-was-mentioned/logo.png"" alt=""Targetprocess.See.Change."">

                    <p style=""font-family:Verdana,sans-serif;font-style:normal;font-size:12px;color:#808080;padding-bottom:0;padding-top:0;margin-top:10px;margin-bottom:0"">

                        --- Please do not remove this line! Targetprocess Ticket#164 ---

                    </p>

                </td>

            </tr>

        </tbody>

    </table>

</div>






</blockquote></div><br></div>";
            string output = @"<div dir=""ltr"">This is a reply from Gmail</div>";
            Assert.AreEqual(Utils.FormatComment(input), output);
        }
    }
}
