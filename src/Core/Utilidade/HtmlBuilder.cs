namespace Snebur.Utilidade;

public class HtmlBuilder : IndentedStringBuilder
{
    public void OpenTag(string tagName, string? attributes = null)
    {
        if (string.IsNullOrEmpty(attributes))
        {
            AppendLine($"<{tagName}>");
        }
        else
        {
            AppendLine($"<{tagName} {attributes}>");
        }
        IncreaseIndent();
    }
    public void Tag(
        string tagName,
        Action action)
    {
        OpenTag(tagName);
        action();
        CloseTag(tagName);
    }
    public void Tag(
       string tagName,
       string content)
    {
        OpenTag(tagName);
        AppendLine(content);
        CloseTag(tagName);
    }

    public void Tag(string tagName,
        string? attributes,
        string content)
    {
        OpenTag(tagName, attributes);
        AppendLine(content);
        CloseTag(tagName);
    }
    public void Tag(string tagName,
        string? attributes,
        Action action)
    {
        OpenTag(tagName, attributes);
        action();
        CloseTag(tagName);
    }
     
    public void CloseTag(string tagName)
    {
        DecreaseIndent();
        AppendLine($"</{tagName}>");
    }
}
