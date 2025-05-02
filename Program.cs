using Raylib_cs;
using SuperSmashTrees.Core;
using SuperSmashTrees.UI;
using static System.Formats.Asn1.AsnWriter;

class Program
{
    enum Scene
    {
        Menu,
        CharacterSelect,
        Game,
        Exit
    }

    static void Main()
    {
        Raylib.InitWindow(1920, 1080, "Super Smash Trees");
        Raylib.ToggleFullscreen();
        Raylib.SetTargetFPS(60);

        Scene currentScene = Scene.Menu;

        CharacterOption? selected1 = null;
        CharacterOption? selected2 = null;

        while (!Raylib.WindowShouldClose() && currentScene != Scene.Exit)
        {
            switch (currentScene)
            {
                case Scene.Menu:
                    Menu menu = new Menu(Raylib.GetScreenWidth(), Raylib.GetScreenHeight());
                    while (!menu.StartGame && !Raylib.WindowShouldClose())
                    {
                        Raylib.BeginDrawing();
                        Raylib.ClearBackground(Color.Black);
                        menu.Update();
                        menu.Draw();
                        Raylib.EndDrawing();
                    }

                    if (menu.StartGame)
                        currentScene = Scene.CharacterSelect;
                    break;

                case Scene.CharacterSelect:
                    CharacterSelect selector = new CharacterSelect(Raylib.GetScreenWidth(), Raylib.GetScreenHeight());

                    while (!selector.SelectionDone && !Raylib.WindowShouldClose())
                    {
                        Raylib.BeginDrawing();
                        Raylib.ClearBackground(Color.Black);
                        selector.Update();
                        selector.Draw();
                        Raylib.EndDrawing();
                    }

                    if (selector.SelectionDone)
                    {
                        selected1 = selector.Player1Character;
                        selected2 = selector.Player2Character;
                        currentScene = Scene.Game;
                    }
                    break;

                case Scene.Game:
                    if (selected1 != null && selected2 != null)
                    {
                        GameManager game = new GameManager(Raylib.GetScreenWidth(), Raylib.GetScreenHeight(), selected1, selected2);
                        game.Run();

                        // Cuando GameManager.Run() termina, volvemos al menú
                        currentScene = Scene.Menu;
                    }
                    else
                    {
                        currentScene = Scene.Menu;
                    }
                    break;
            }
        }

        Raylib.CloseWindow();
    }
}
