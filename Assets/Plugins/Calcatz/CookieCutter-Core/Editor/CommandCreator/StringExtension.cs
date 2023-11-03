using System.Text;

namespace Calcatz.CookieCutter {
    public static class StringExtension {

        public static string ConvertToCamelCase(this string _str) {
            string[] splittedPhrase = _str.Split(' ', '-', '.');
            var sb = new StringBuilder();

            if (splittedPhrase[0][0] >= '0' && splittedPhrase[0][0] <= '9') {
                splittedPhrase[0] = "_" + splittedPhrase[0];
            }
            sb.Append(splittedPhrase[0].ToLower());
            splittedPhrase[0] = string.Empty;

            foreach (var s in splittedPhrase) {
                char[] splittedPhraseChars = s.ToCharArray();
                if (splittedPhraseChars.Length > 0) {
                    splittedPhraseChars[0] = ((new string(splittedPhraseChars[0], 1)).ToUpper().ToCharArray())[0];
                }
                sb.Append(new string(splittedPhraseChars));
            }
            return sb.ToString().RemoveSpecialCharacters();
        }

        public static string ConvertToPascalCase(this string _str) {
            string[] splittedPhrase = _str.Split(' ', '-', '.', '_');
            var sb = new StringBuilder();

            foreach (var s in splittedPhrase) {
                char[] splittedPhraseChars = s.ToLower().ToCharArray();
                if (splittedPhraseChars.Length > 0) {
                    splittedPhraseChars[0] = ((new string(splittedPhraseChars[0], 1)).ToUpper().ToCharArray())[0];
                }
                sb.Append(new string(splittedPhraseChars));
            }
            return sb.ToString().RemoveSpecialCharacters();
        }

        public static string ConvertToMacroCase(this string _str) {
            string[] splittedPhrase = _str.ToUpper().Split(' ', '-', '.');
            var sb = new StringBuilder();

            foreach (var s in splittedPhrase) {
                char[] splittedPhraseChars = s.ToCharArray();
                sb.Append(new string(splittedPhraseChars) + "_");
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        public static string RemoveSpecialCharacters(this string _str) {
            StringBuilder sb = new StringBuilder();
            foreach (char c in _str) {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '_') {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

    }
}