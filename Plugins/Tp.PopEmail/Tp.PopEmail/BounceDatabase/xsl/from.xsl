<?xml version="1.0" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

  <xsl:template match="/">


  <!-- First, write out the HTML page header information -->
  <HTML>
  <HEAD>
    <TITLE></TITLE>
  </HEAD>
  <BODY>

	<H1>Language code: <xsl:value-of select="from/@lang"/></H1>

  <!-- Create the Table's header row -->
  <TABLE BORDER="1">
	<CAPTION>Name</CAPTION>
    <THEAD>
    <TR>
      <TH>Keyword</TH>
      <TH>Match</TH>
    </TR>
    </THEAD>

  <!-- Now Create the page from the data in the XML File -->
  <xsl:for-each select="from/fromnames/fromaddress">

    <TR>
      <TD><xsl:value-of select="@keyword"/></TD>
      <TD><xsl:value-of select="@match"/></TD>
    </TR>
  </xsl:for-each>

  </TABLE>

  <!-- Create the Table's header row -->
  <TABLE BORDER="1">
	<CAPTION>E-mail</CAPTION>
    <THEAD>
    <TR>
      <TH>Keyword</TH>
      <TH>Match</TH>
    </TR>
    </THEAD>

  <!-- Now Create the page from the data in the XML File -->
  <xsl:for-each select="from/fromemails/fromaddress">

    <TR>
      <TD><xsl:value-of select="@keyword"/></TD>
      <TD><xsl:value-of select="@match"/></TD>
    </TR>
  </xsl:for-each>

  </TABLE>


  <!-- Create the Table's header row -->
  <TABLE BORDER="1">
	<CAPTION>Remark</CAPTION>
    <THEAD>
    <TR>
      <TH>Keyword</TH>
      <TH>Match</TH>
    </TR>
    </THEAD>

  <!-- Now Create the page from the data in the XML File -->
  <xsl:for-each select="from/fromremarks/fromaddress">

    <TR>
      <TD><xsl:value-of select="@keyword"/></TD>
      <TD><xsl:value-of select="@match"/></TD>
    </TR>
  </xsl:for-each>

  </TABLE>


  <!-- Create the Table's header row -->
  <TABLE BORDER="1">
	<CAPTION>Address string</CAPTION>
    <THEAD>
    <TR>
      <TH>Keyword</TH>
      <TH>Match</TH>
    </TR>
    </THEAD>

  <!-- Now Create the page from the data in the XML File -->
  <xsl:for-each select="from/fromstrings/fromaddress">

    <TR>
      <TD><xsl:value-of select="@keyword"/></TD>
      <TD><xsl:value-of select="@match"/></TD>
    </TR>
  </xsl:for-each>

  </TABLE>

  <!-- Finally, put the ending tags for the HTML stuff -->

  </BODY>
  </HTML>

  </xsl:template>

</xsl:stylesheet>
