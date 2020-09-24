// 
// Copyright (c) 2005-2020 TargetProcess. All rights reserved.
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
        public void Test1FormatCommentFromMailWindows()
        {
            string input = @"<div class=""WordSection1"">
         <p class=""MsoNormal"">
            Good afternoon Bogdan,
            <o:p></o:p>
         </p>
         <p class=""MsoNormal"">
            <o:p>&nbsp;</o:p>
         </p>
         <p class=""MsoNormal"">
            Please extend our “Thanks” to the Developer/team on helping us with the data migration of these 2 projects!
            <o:p></o:p>
         </p>
         <p class=""MsoNormal"">
            <o:p>&nbsp;</o:p>
         </p>
         <p class=""MsoNormal"">
            Everything looked spotted on in our esri Production instance for the team!
            <o:p></o:p>
         </p>
         <p class=""MsoNormal"">
            <o:p>&nbsp;</o:p>
         </p>
         <p class=""MsoNormal"">
            Hope you and yours are doing well . . . and enjoy the weekend break!
            <o:p></o:p>
         </p>
         <p class=""MsoNormal"">
            <span style=""font-family:&quot;Segoe UI Emoji&quot;,sans-serif"">&#128521;</span>
            <o:p></o:p>
         </p>
         <p class=""MsoNormal"">
            <o:p>&nbsp;</o:p>
         </p>
         <p class=""MsoNormal"">
            John
            <o:p></o:p>
         </p>
         <p class=""MsoNormal""><o:p>&nbsp;</o:p></p>  <div>  <div style=""border:none;border-top:solid #E1E1E1 1.0pt;padding:3.0pt 0in 0in 0in"">  <p class=""MsoNormal""><b>From:</b> support@targetprocess.com &lt;support@targetprocess.com&gt;  <br>  <b>Sent:</b> Friday, June 26, 2020 4:56 AM<br>  <b>To:</b> John Wiese &lt;jwiese@esri.com&gt;<br>  <b>Subject:</b> Re: #253951 &quot;Migration of 2 Projects between esriCharss to esri instance&quot; - Your Targetprocess Service Desk Ticket  
                  <o:p></o:p>
               </p>
            </div>
         </div>
         <p class=""MsoNormal"">
            <o:p>&nbsp;</o:p>
         </p>
         <div style=""margin-left:7.5pt;margin-top:7.5pt;margin-right:7.5pt;margin-bottom:7.5pt"">
            <table class=""MsoNormalTable"" border=""1"" cellspacing=""0"" cellpadding=""0"" style=""background:white;border:solid #DDDDDD 1.0pt"">
               <tbody>
                  <tr>
                     <td style=""border:none;padding:11.25pt 11.25pt 11.25pt 11.25pt"">
                        <div>
                           <h4>
                              <span style=""font-family:&quot;Arial&quot;,sans-serif;font-weight:normal"">
                                 Hello John,
                                 <o:p></o:p>
                              </span>
                           </h4>
                        </div>
                        <div>
                           <h4>
                              <span style=""font-family:&quot;Arial&quot;,sans-serif;color:black;font-weight:normal"">&nbsp;</span>
                              <span style=""font-family:&quot;Arial&quot;,sans-serif;font-weight:normal"">
                                 <o:p></o:p>
                              </span>
                           </h4>
                        </div>
                        <div>
                           <h4>
                              <span style=""font-family:&quot;Arial&quot;,sans-serif;color:black;font-weight:normal"">We have migrated projects in question from esricharss to esri account. Please check and let us know how it works for you.</span>
                              <span style=""font-family:&quot;Arial&quot;,sans-serif;font-weight:normal"">
                                 <o:p></o:p>
                              </span>
                           </h4>
                        </div>
                        <div>
                           <h4>
                              <span style=""font-family:&quot;Arial&quot;,sans-serif;color:black;font-weight:normal"">&nbsp;</span>
                              <span style=""font-family:&quot;Arial&quot;,sans-serif;font-weight:normal"">
                                 <o:p></o:p>
                              </span>
                           </h4>
                        </div>
                        <div>
                           <h4>
                              <span style=""font-family:&quot;Arial&quot;,sans-serif;color:black;font-weight:normal"">Best regards,<br>  Bogdan</span>
                              <span style=""font-family:&quot;Arial&quot;,sans-serif;font-weight:normal"">
                                 <o:p></o:p>
                              </span>
                           </h4>
                        </div>
                        <h4>
                           <span style=""font-family:&quot;Arial&quot;,sans-serif;color:#666666;font-weight:normal"">Added:</span><span style=""font-family:&quot;Arial&quot;,sans-serif;color:black""><br>  26-Jun-2020 by Bogdan Skaskiv <br>  <a href=""https://urldefense.proofpoint.com/v2/url?u=http-3A__www.targetprocess.com_support_&amp;d=DwMGaQ&amp;c=n6-cguzQvX_tUIrZOS_4Og&amp;r=3XFlskpvwZjU-ao-T6S28A&amp;m=93qkHca1fOME7Gg8Y0aKagNt5iCUJIAKHe8NiLAMWyU&amp;s=H7GmShGkGkHcvCDnm-frBeuLzl_Sy885nbUEOxGC4A8&amp;e="">Targetprocess   Team</a> </span>
                           <span style=""font-family:&quot;Arial&quot;,sans-serif"">
                              <o:p></o:p>
                           </span>
                        </h4>
                     </td>
                  </tr>
               </tbody>
            </table>
            <div style=""margin-left:26.25pt;margin-right:26.25pt"">
               <h4>
                  <span style=""color:#666666;font-weight:normal"">--- Please do not remove this line! Targetprocess Service Desk Ticket#253951 (<a href=""https://urldefense.proofpoint.com/v2/url?u=https-3A__helpdesk.targetprocess.com_request_253951&amp;d=DwMGaQ&amp;c=n6-cguzQvX_tUIrZOS_4Og&amp;r=3XFlskpvwZjU-ao-T6S28A&amp;m=93qkHca1fOME7Gg8Y0aKagNt5iCUJIAKHe8NiLAMWyU&amp;s=DUUw4mfdH79Ha-F0vudJQnlSzbFGD761cq2ZA8eo7uU&amp;e="">check   the status online</a>) ---</span> 
                  <o:p></o:p>
               </h4>
            </div>
         </div>
      </div>";

            string output = @"<div class=""WordSection1"">
         <p class=""MsoNormal"">
            Good afternoon Bogdan,
            <o:p></o:p>
         </p>
         <p class=""MsoNormal"">
            <o:p>&nbsp;</o:p>
         </p>
         <p class=""MsoNormal"">
            Please extend our “Thanks” to the Developer/team on helping us with the data migration of these 2 projects!
            <o:p></o:p>
         </p>
         <p class=""MsoNormal"">
            <o:p>&nbsp;</o:p>
         </p>
         <p class=""MsoNormal"">
            Everything looked spotted on in our esri Production instance for the team!
            <o:p></o:p>
         </p>
         <p class=""MsoNormal"">
            <o:p>&nbsp;</o:p>
         </p>
         <p class=""MsoNormal"">
            Hope you and yours are doing well . . . and enjoy the weekend break!
            <o:p></o:p>
         </p>
         <p class=""MsoNormal"">
            <span style=""font-family:&quot;Segoe UI Emoji&quot;,sans-serif"">&#128521;</span>
            <o:p></o:p>
         </p>
         <p class=""MsoNormal"">
            <o:p>&nbsp;</o:p>
         </p>
         <p class=""MsoNormal"">
            John
            <o:p></o:p>
         </p>";
            Assert.AreEqual(Utils.FormatComment(input), output);
        }

        [Test]
        public void TestFormatCommentFromOutlookMobile()
        {
            string input = @"<div dir=""auto"" style=""direction: ltr; margin: 0; padding: 0; font-family: sans-serif; font-size: 11pt; color: black; "">  I'll get back to you tomorrow morning<span id=""ms-outlook-android-cursor""></span>
    <br>
    <br>
</div>
<div dir=""auto"" style=""direction: ltr; margin: 0; padding: 0; font-family: sans-serif; font-size: 11pt; color: black; "">
    <span id=""OutlookSignature"">
        <div dir=""auto"" style=""direction: ltr; margin: 0; padding: 0; font-family: sans-serif; font-size: 11pt; color: black; "">  Sent from <a href=""https://aka.ms/blhgte"">Outlook Mobile</a></div>
    </span><br>
</div>
<hr style=""display:inline-block;width:98%"" tabindex=""-1"">
<div id=""divRplyFwdMsg"" dir=""ltr"">
    <font face=""Calibri, sans-serif"" style=""font-size:11pt"" color=""#000000"">
        <b>From:</b> support@targetprocess.com &lt;support@targetprocess.com&gt;<br>
        <b>Sent:</b> Monday, June 22, 2020 4:01:14 PM<br>
        <b>To:</b> Martin Belanger [Ext. 271] &lt;Martinbelanger@Rideau.com&gt;<br>
       <b>Subject:</b> Re: #254419 &quot;Rideau: Update notification&quot; - Your Targetprocess Service Desk Ticket
    </font>
   <div>&nbsp;</div>
</div>
<div style=""margin:0px; padding:0px; background:#f1f2f3"">
    <div style=""margin:10px"">
        <table cellpadding=""0"" cellspacing=""0"" style=""background:#fff; padding:15px; margin:0px 0px 5px 0px; border:1px solid #dddddd; font-family:Arial"">
            <tbody>
                <tr>
                    <td>
                        <h4 style=""font-weight:normal"">
                            <div>Should we stop rolling back the version then? Since it won`t be possible to investigate the issue if it is not reproduced.</div>  <div>&nbsp;</div>
                            <div>Best regards,</div>
                            <div>Andrey M.</div>
                        </h4>
                        <h4>
                            <span style=""font-weight:normal; color:#666666"">Added:</span><br>  22-Jun-2020 by Andrey Metelsky <br>
                            <a href=""http://www.targetprocess.com/support/"">Targetprocess Team</a> <br>
                        </h4>
                    </td>
                </tr>
            </tbody>
        </table>
        <div style=""margin:0px 35px; padding:0px 0px 40px 0px"">
            <h4>
                <span style=""font-weight:normal; color:#666666"">--- Please do not remove this line! Targetprocess Service Desk Ticket#254419 (<a href=""https://helpdesk.targetprocess.com/request/254419"">check the status online</a>) ---</span>
            </h4>
        </div>
    </div>
</div>";

			string output = @"<div dir=""auto"" style=""direction: ltr; margin: 0; padding: 0; font-family: sans-serif; font-size: 11pt; color: black; "">  I'll get back to you tomorrow morning<span id=""ms-outlook-android-cursor""></span>
    <br>
    <br>
</div>";
            Assert.AreEqual(Utils.FormatComment(input), output);
        }

        [Test]
        public void Test2FormatCommentFromMailWindows()
        {
            string input =
                @"<div class=""WordSection1"">
    <p class=""MsoNormal"">Hi Bogdan, just tried and still get the “Access Token sliding disabled” error.
        <o:p></o:p>
    </p>
    <p class=""MsoNormal"">
        <o:p>&nbsp;</o:p>
    </p>
    <p class=""MsoNormal"">Sure, I’ll ping you on Monday
        <o:p></o:p>
    </p>
    <p class=""MsoNormal"">
        <o:p>&nbsp;</o:p>
    </p>
    <div>
        <p class=""MsoNormal"">Thanks,
            <o:p></o:p>
        </p>
    </div>
    <p class=""MsoNormal"">Marco
        <o:p></o:p>
    </p>
    <p class=""MsoNormal""><o:p>&nbsp;</o:p></p>  <div style=""border:none;border-top:solid #B5C4DF 1.0pt;padding:3.0pt 0in 0in 0in"">
        <p class=""MsoNormal""><b><span style=""font-size:12.0pt;color:black"">From: </span></b><span style=""font-size:12.0pt;color:black"">&quot;support@targetprocess.com&quot; &lt;support@targetprocess.com&gt;<br>  <b>Date: </b>Friday, November 22, 2019 at 10:36 AM<br>  <b>To: </b>&quot;Marco Wu (yuwu3)&quot; &lt;yuwu3@cisco.com&gt;<br>  <b>Subject: </b>Re: #221653 &quot;Export: Access token sliding disabled&quot; - Your Targetprocess Service Desk Ticket  <o:p></o:p></span></p>
    </div>
    <div>
        <p class=""MsoNormal"">
            <o:p>&nbsp;</o:p>
        </p>
    </div>
    <div style=""margin-left:7.5pt;margin-top:7.5pt;margin-right:7.5pt;margin-bottom:7.5pt"">
        <table class=""MsoNormalTable"" border=""1"" cellspacing=""0"" cellpadding=""0"" style=""background:white;border:solid #DDDDDD 1.0pt"">
            <tbody>
                <tr>
                    <td style=""border:none;padding:11.25pt 11.25pt 11.25pt 11.25pt"">
                        <div>
                            <h4><span style=""font-family:&quot;Arial&quot;,sans-serif;font-weight:normal"">Hi,<o:p></o:p></span></h4> </div>
                        <div>
                            <h4><span style=""font-family:&quot;Arial&quot;,sans-serif;color:black;font-weight:normal"">&nbsp;</span><span style=""font-family:&quot;Arial&quot;,sans-serif;font-weight:normal""><o:p></o:p></span></h4> </div>
                        <div>
                            <h4><span style=""font-family:&quot;Arial&quot;,sans-serif;color:black;font-weight:normal"">Marco, can you please try if this feature is working in your sandbox environment and confirm this to us? Also, i would like to invite you in the  <a href=""https://lc.chat/now/6880831/"" target=""_blank"">Livechat</a> on Monday so we could check everything together in the screensharing session in order to find the solution.</span><span style=""font-family:&quot;Arial&quot;,sans-serif;font-weight:normal""><o:p></o:p></span></h4> </div>
                        <div>
                            <h4><span style=""font-family:&quot;Arial&quot;,sans-serif;color:black;font-weight:normal"">&nbsp;</span><span style=""font-family:&quot;Arial&quot;,sans-serif;font-weight:normal""><o:p></o:p></span></h4> </div>
                        <div>
                            <h4><span style=""font-family:&quot;Arial&quot;,sans-serif;color:black;font-weight:normal"">Best regards,</span><span style=""font-family:&quot;Arial&quot;,sans-serif;font-weight:normal""><o:p></o:p></span></h4> </div>
                        <div>
                            <h4><span style=""font-family:&quot;Arial&quot;,sans-serif;color:black;font-weight:normal"">Bogdan</span><span style=""font-family:&quot;Arial&quot;,sans-serif;font-weight:normal""><o:p></o:p></span></h4> </div>
                        <h4><span style=""font-family:&quot;Arial&quot;,sans-serif;color:#666666;font-weight:normal"">Added:</span><span style=""font-family:&quot;Arial&quot;,sans-serif;color:black""><br>  22-Nov-2019 by Bogdan Skaskiv <br>  <a href=""http://www.targetprocess.com/support/"">Targetprocess Team</a> </span><span style=""font-family:&quot;Arial&quot;,sans-serif""><o:p></o:p></span></h4> </td>
                </tr>
            </tbody>
        </table>
        <div style=""margin-left:26.25pt;margin-right:26.25pt"">
            <h4><span style=""color:#666666;font-weight:normal"">--- Please do not remove this line! Targetprocess Service Desk Ticket#221653 (<a href=""https://helpdesk.targetprocess.com/request/221653"">check the status online</a>) ---</span>  <o:p></o:p></h4> </div>
    </div>
</div>";
            string output = @"<div class=""WordSection1"">
    <p class=""MsoNormal"">Hi Bogdan, just tried and still get the “Access Token sliding disabled” error.
        <o:p></o:p>
    </p>
    <p class=""MsoNormal"">
        <o:p>&nbsp;</o:p>
    </p>
    <p class=""MsoNormal"">Sure, I’ll ping you on Monday
        <o:p></o:p>
    </p>
    <p class=""MsoNormal"">
        <o:p>&nbsp;</o:p>
    </p>
    <div>
        <p class=""MsoNormal"">Thanks,
            <o:p></o:p>
        </p>
    </div>
    <p class=""MsoNormal"">Marco
        <o:p></o:p>
    </p>";
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
