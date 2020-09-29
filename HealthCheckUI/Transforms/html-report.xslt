<?xml version="1.0" encoding="iso-8859-1"?>
<xsl:stylesheet
    version="2.0"
    exclude-result-prefixes="msxsl"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt">
  <xsl:output method="html" indent="yes" />

  <!-- Master page area-->
  <xsl:template match="@* | node()">
    <html>
      <head>
        <meta charset="utf-8" />
        <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no" />
        <!-- Latest compiled and minified CSS -->
        <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" integrity="sha384-JcKb8q3iqJ61gNV9KGb8thSsNjpSL0n8PARn9HuZOnIxN0hoP+VmmDGMN5t9UJ0Z" crossorigin="anonymous" />
        <!-- Optional theme -->
        <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap-theme.min.css" integrity="sha384-rHyoN1iRsVXV4nD0JutlnGaslCJuC7uwjduW9SVrLvRYooPp2bWYgmgJQIXwl/Sp" crossorigin="anonymous" />
        <style type="text/css">
          .Passed { color: green; }
          .Inconclusive { color: #BBAA00; }
          .Failed { color: red; }
          ul {
          margin-left: 0px;
          list-style-type: none;
          padding-left: 0;
          }
          ul ul {
          margin-left: 15px;
          }
          label {
          font-weight: normal;
          }
          .counts {
          font-size: .7em;
          color: gray;
          }
          .testmessage{
          font-size:10px;
          }
        </style>
      </head>
      <body>
        <div class="container">
          <div class="card">
            <div class="card-body">
              <h1 class="card-title">
                <span class="badge badge-light">
                  Test Info
                </span>
              </h1>
              <p class="card-text">
                <xsl:value-of select="//test-suite[@type='Assembly']/settings/setting[@name='TestParametersDictionary']/item[@key='url']/@value" />
              </p>

              <p class="card-text">
                <xsl:value-of select="//test-suite[@type='Assembly']/environment/@user" />
              </p>
            </div>
          </div>
          <xsl:apply-templates select="/test-run/test-suite" />
          <div>
            <h1>Run Statistics</h1>
            <p>
              <xsl:value-of select="concat('Start time: ', /test-run/@start-time)" />
            </p>
            <p>
              <xsl:value-of select="concat('End time: ', /test-run/@end-time)" />
            </p>
            <p>
              <xsl:value-of select="concat(' Duration: ', format-number(/test-run/@duration, '0.000'), ' seconds')" />
            </p>
          </div>
        </div>

        <!-- Latest compiled and minified JavaScript -->
        <script src="https://code.jquery.com/jquery-3.5.1.slim.min.js" integrity="sha384-DfXdz2htPH0lsSSs5nCTpuj/zy4C+OGpamoFVy38MVBnE+IbbVYUew+OrCXaRkfj" crossorigin="anonymous"></script>
        <script src="https://cdn.jsdelivr.net/npm/popper.js@1.16.1/dist/umd/popper.min.js" integrity="sha384-9/reFTGAW83EW2RDu2S0VKaIzap3H66lZH81PoYlFhbGU+6BZp6G7niu735Sk7lN" crossorigin="anonymous"></script>
        <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js" integrity="sha384-B4gt1jrGC7Jh4AgTPSdUtOBvfO8shuf57BaghqFfPlYxofvL8/KUEfYiJOMMV+rV" crossorigin="anonymous"></script>
        <script type="text/javascript">
          $("td label").each(function(i, e) {
          $(e).text($(e).text().replace(/_/g, " "));
          });
          $(".description").each(function(i, e) {
          $(e).html($(e).html().trim().replace(/\n/g, '<br />'));
          });
        </script>
      </body>
    </html>
  </xsl:template>

  <!-- Top level test area detailing enture runtime -->
  <xsl:template match="/test-run/test-suite">

    <table class="table table-striped">
      <xsl:apply-templates select="./test-suite" />
    </table>
  </xsl:template>

  <!-- Text fixture level with all tests under it -->
  <xsl:template match="test-suite">
    <xsl:if test="./@type='TestFixture'">
      <tr>
        <td>
          <xsl:attribute name="class">
            <xsl:choose>
              <xsl:when test="./@failed > 0">Failed</xsl:when>
              <xsl:when test="./@inconclusive > 0">Inconclusive</xsl:when>
              <xsl:otherwise>Passed</xsl:otherwise>
            </xsl:choose>
          </xsl:attribute>
          <xsl:attribute name="style">
            padding-left: <xsl:value-of select="count(ancestor::test-suite)*15" />px;
          </xsl:attribute>
          <xsl:value-of select="./@name" />
        </td>

        <td class="counts">
          <xsl:value-of select="./@passed" /> passed,
          <xsl:value-of select="./@inconclusive" /> inconclusive,
          <xsl:value-of select="./@failed" /> failed
        </td>
      </tr>
    </xsl:if>
    <xsl:for-each select="./test-suite">
      <xsl:apply-templates select="." />
    </xsl:for-each>
    <xsl:for-each select="./test-case">
      <xsl:sort select="./properties/property[@name='DocumentationOrder']/@value" />
      <xsl:apply-templates select="." />
    </xsl:for-each>
  </xsl:template>

  <!-- Individual Test Level -->
  <xsl:template match="test-case">
    <tr>
      <td width="90%">
        <xsl:attribute name="style">
          padding-left: <xsl:value-of select="count(ancestor::test-suite)*15" />px;
        </xsl:attribute>
        <label>
          <xsl:attribute name="class">
            <xsl:value-of select="./@result" />
          </xsl:attribute>
          <xsl:value-of select="./@name" />
        </label>
        <xsl:if test="./@result='Passed'">
          <xsl:if test="./properties/property[@name='Description']">
            <div class="description">
              <xsl:value-of select="./properties/property[@name='Description']/@value" />
              <xsl:text> </xsl:text>
            </div>
          </xsl:if>
        </xsl:if>
        <xsl:if test="./@result='Failed'">
          <br />
          <p class="testmessage">
            <xsl:value-of select="./failure/message" />
          </p>
        </xsl:if>
      </td>
      <td class="counts">
        <xsl:value-of select="concat('    Duration: ', ./@duration, ' seconds')" />
      </td>
    </tr>
  </xsl:template>
</xsl:stylesheet>