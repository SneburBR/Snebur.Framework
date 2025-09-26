using System.Text;

namespace Snebur.Utilidade;

public class IndentStringBuilder
{
    private readonly StringBuilder _sb = new();
    private int indentLevel = 0;

    public void AppendLine(string line)
    {
        var tab = new string(' ', indentLevel * 4);
        _sb.AppendLine($"{tab}{line}");
    }

    public void OpenCurlyBrace()
    {
        AppendLine("{");
        indentLevel++;
    }

    public void CloseCurlyBrace()
    {
        indentLevel--;
        AppendLine("}");
    }

    public void IncreaseIndent()
        => indentLevel++;
    public void DecreaseIndent()
        => indentLevel--;

    public void Clear()
        => _sb.Clear();
    public override string ToString()
    => _sb.ToString();

}
