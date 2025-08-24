namespace VisuALS_WPF_App.KeyboardLayouts
{
    public class EnglishClassicQwertyLayout : VKeyboardLayout
    {
        public EnglishClassicQwertyLayout()
        {
            Add("Main", new VQwertyTab());
            Add("Menu", new VMenuTab());
            Add("Special 1", new VSpecialChars1());
            Add("Special 2", new VSpecialChars2());
            Add("Cursor Control", new VCursorControlTab());
            Add("Bar", new VClassicKeyBar());
        }
    }

    public class SpanishClassicQwertyLayout : VKeyboardLayout
    {
        public SpanishClassicQwertyLayout()
        {
            Add("Main", new VQwertyTabEs());
            Add("Menu", new VMenuTab());
            Add("Special 1", new VSpecialChars1Es());
            Add("Special 2", new VSpecialChars2Es());
            Add("Cursor Control", new VCursorControlTab());
            Add("Bar", new VClassicKeyBar());
        }
    }
}
