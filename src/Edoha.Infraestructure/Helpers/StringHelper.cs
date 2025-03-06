using System.Text.RegularExpressions;

namespace Edoha.Infraestructure.Helpers
{
    public class StringHelper
    {
        public static string PascalToSnakeCase(string pascalCase)
        {
            string spacedString = Regex.Replace(pascalCase, "(?<!^)([A-Z])", " $1");
            string snakeCase = spacedString.Replace(" ", "_").ToLower();

            return snakeCase;
        }
    }
}
