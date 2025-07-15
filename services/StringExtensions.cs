using System.Text;

namespace climate;

public static class StringExtensions
{
    public static string Rollup(this string[] lines_of_code)
    {
        return lines_of_code.Aggregate(
                new StringBuilder(),
                (sb, next) =>
                {
                    sb.AppendLine(next);
                    return sb;
                })
            .ToString();
    }
}