namespace CraigsCorner.Utils
{
    public class RichTextFormating
    {
        // Rich text formating for Utility (blue) text 
        public static string UT<T>(T inputText)
        {
            return $"<style=cIsUtility>{inputText}</style>";
        }

        // Adds rich text formating for Stacking (grey) text 
        public static string ST<T>(T inputText)
        {
            return $"<style=cStack>{inputText}</style>";
        }

        // Adds rich text formating for Damage (yellow) text 
        public static string DT<T>(T inputText)
        {
            return $"<style=cIsDamage>{inputText}</style>";
        }
    }
}