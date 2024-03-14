using System.Text;

namespace Starry.Source.Client.Colour;

public class ColourClient
{
    public ColourClient()
    {
        Stream? stdout = Console.OpenStandardOutput();
        StreamWriter? writer = new StreamWriter(stdout, Encoding.ASCII)
        {
            AutoFlush = true
        };

        Console.SetOut(writer);
    }

    public string ColourText(string text, Colours colour)
            => $"\x1b[1;{(byte)colour}m{text}\x1b[0m";
}
