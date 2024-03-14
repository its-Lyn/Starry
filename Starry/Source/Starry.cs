using Starry.Source.Client;
using Starry.Source.Client.Colour;
using Starry.Source.Config;

namespace Starry.Source;

public static class Starry
{
    public static ColourClient Colour = new ColourClient();

    public static void Main(string[] args)
    {
        StarConfig.EnsureExists();
        new StarParser().Parse(args);
    }
}