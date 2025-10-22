namespace Snebur.Utilidade;

public static class TableHtmlBuilderExtesions
{
    public static HtmlBuilder OpenTable(this HtmlBuilder HtmlBuilder)
    {
        HtmlBuilder.OpenTag("table", "border=\"1\" cellpadding=\"5\" cellspacing=\"0\"");
        return HtmlBuilder;
    }

    public static HtmlBuilder CloseTable(
        this HtmlBuilder HtmlBuilder)
    {
        HtmlBuilder.CloseTag("table");
        return HtmlBuilder;
    }

    public static HtmlBuilder OpenHeaderRow(
        this HtmlBuilder HtmlBuilder )
    {
        HtmlBuilder.OpenTag("thead");
        return HtmlBuilder;
    }
    public static HtmlBuilder CloseHeaderRow(
       this HtmlBuilder HtmlBuilder)
    {
        HtmlBuilder.CloseTag("thead");
        return HtmlBuilder;
    }

    public static HtmlBuilder OpenTableBody(
        this HtmlBuilder HtmlBuilder )
    {
        HtmlBuilder.OpenTag("tbody");
        return HtmlBuilder;
    }

    public static HtmlBuilder CloseTableBody(
       this HtmlBuilder HtmlBuilder)
    {
        HtmlBuilder.CloseTag("tbody");
        return HtmlBuilder;
    }

    public static HtmlBuilder TableHeader(
        this HtmlBuilder HtmlBuilder,
        string headerContent)
    {
        HtmlBuilder.Tag("th", headerContent);
        return HtmlBuilder;
    }

    public static HtmlBuilder OpenTableHeader(
        this HtmlBuilder HtmlBuilder )
    {
        HtmlBuilder.OpenTag("th");
        return HtmlBuilder;
    }

    public static HtmlBuilder CloseTableHeader(
       this HtmlBuilder HtmlBuilder)
    {
        HtmlBuilder.CloseTag("th");
        return HtmlBuilder;
    }

    public static HtmlBuilder TableRow(
        this HtmlBuilder HtmlBuilder, 
        Action rowAction)
    {
        HtmlBuilder.Tag("tr", rowAction);
        return HtmlBuilder;
    }

    public static HtmlBuilder OpenTableRow(
       this HtmlBuilder HtmlBuilder )
    {
        HtmlBuilder.OpenTag("tr");
        return HtmlBuilder;
    }

    public static HtmlBuilder CloseTableRow(
       this HtmlBuilder HtmlBuilder)
    {
        HtmlBuilder.CloseTag("tr");
        return HtmlBuilder;
    }

    public static HtmlBuilder TableData(
       this HtmlBuilder HtmlBuilder,
       string dataContent)
    {
        HtmlBuilder.Tag("td", dataContent);
        return HtmlBuilder;
    }
    public static HtmlBuilder TableData(
       this HtmlBuilder HtmlBuilder,
       long dataContent)
    {
        HtmlBuilder.Tag("td", dataContent.ToString());
        return HtmlBuilder;
    }
}
