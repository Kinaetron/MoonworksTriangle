using BasicTriangle;
using MoonWorks;

internal class Program
{
    static void Main()
    {
        var windowCreateInfo = new WindowCreateInfo(
             "Triangle Example Game",
             1280,
             720,
             ScreenMode.Windowed
         );

        var framePacingSettings = FramePacingSettings.CreateLatencyOptimized(60);

        var game = new BasicTriangleGame(
            new AppInfo("Triangle Example Game", "TriangleExampleGame"),
            windowCreateInfo,
            framePacingSettings);

        game.Run();
    }
}