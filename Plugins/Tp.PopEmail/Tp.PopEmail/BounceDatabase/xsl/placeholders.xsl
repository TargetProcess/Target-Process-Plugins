<?xml version="1.0" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

  <xsl:template match="/">


  <!-- First, write out the HTML page header information -->
  <HTML>
  <HEAD>
    <TITLE></TITLE>
  </HEAD>
  <BODY>



  <!-- Create the Table's header row -->
  <TABLE BORDER="1">
	<CAPTION>Language code: <xsl:value-of select="placeholders/@lang"/></CAPTION>
    <THEAD>
    <TR>
      <TH>Name</TH>
      <TH>RegExp</TH>
    </TR>
    </THEAD>

  <!-- Now Create the page from the data in the XML File -->
  <xsl:for-each select="placeholders/regexps/regexp">

    <TR>
      <TD><xsl:value-of select="@name"/></TD>
      <TD><xsl:value-of select="../mask"/></TD>
    </TR>
  </xsl:for-each>

  </TABLE>



  <!-- Finally, put the ending tags for the HTML stuff -->

  </BODY>
  </HTML>

  </xsl:template>

</xsl:stylesheet>
