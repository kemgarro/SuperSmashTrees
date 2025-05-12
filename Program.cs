using Raylib_cs;
using SuperSmashTrees.Core;
using SuperSmashTrees.UI;

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
                    var menu = new Menu(Raylib.GetScreenWidth(), Raylib.GetScreenHeight());

                    while (!menu.StartGame && !menu.ShouldExit && !Raylib.WindowShouldClose())
                    {
                        Raylib.BeginDrawing();
                        Raylib.ClearBackground(Color.Black);
                        menu.Update();
                        menu.Draw();
                        Raylib.EndDrawing();
                    }

                    if (menu.ShouldExit)
                        currentScene = Scene.Exit;
                    else if (menu.StartGame)
                        currentScene = Scene.CharacterSelect;
                    break;

                case Scene.CharacterSelect:
                    var selector = new CharacterSelect(Raylib.GetScreenWidth(), Raylib.GetScreenHeight());

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
                    else
                    {
                        currentScene = Scene.Menu;
                    }
                    break;

                case Scene.Game:
                    if (selected1 != null && selected2 != null)
                    {
                        GameManager game = new GameManager(
                            Raylib.GetScreenWidth(), Raylib.GetScreenHeight(),
                            selected1, selected2
                        );
                        game.Run();  // ✅ El juego se crea desde cero cada vez
                    }

                    currentScene = Scene.Menu;
                    break;
            }
        }

        if (Raylib.IsWindowReady())
        {
            SuperSmashTrees.Utils.TextureManager.UnloadAll();
            Raylib.CloseWindow();
        }
    }
}
 