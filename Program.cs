using Raylib_cs;
using System.Numerics;
using SuperSmashTrees.Entities;
using SuperSmashTrees.UI;
using SuperSmashTrees.Core;

class Program
{
    static void Main()
    {
        // Inicializá la ventana con un tamaño mínimo primero
        Raylib.InitWindow(800, 600, "Super Smash Trees 🌲");

        // Ahora que Raylib está listo, obtené el tamaño real del monitor
        int screenWidth = Raylib.GetMonitorWidth(0);
        int screenHeight = Raylib.GetMonitorHeight(0);

        // Aplicá pantalla completa después
        Raylib.SetWindowSize(screenWidth, screenHeight);
        Raylib.ToggleFullscreen();

        Raylib.SetTargetFPS(60);

        Menu menu = new Menu(screenWidth, screenHeight);
        CharacterSelect? characterSelect = null;
        GameManager? game = null;

        while (!Raylib.WindowShouldClose())
        {
            if (!menu.StartGame)
            {
                menu.Update();
                Raylib.BeginDrawing();
                menu.Draw();
                Raylib.EndDrawing();
            }
            else if (characterSelect == null)
            {
                characterSelect = new CharacterSelect(screenWidth, screenHeight);
            }
            else if (!characterSelect.SelectionDone)
            {
                characterSelect.Update();
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.DarkGray);
                characterSelect.Draw();
                Raylib.EndDrawing();
            }
            else if (game == null)
            {
                // Aquí pasamos ambos personajes seleccionados
                game = new GameManager(screenWidth, screenHeight,
                       characterSelect.Player1Character,
                       characterSelect.Player2Character);
                game.Run();
            }
        }
    }
}
