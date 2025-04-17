using Raylib_cs;
using System.Numerics;
using UI;              // Asegúrate de tener esta línea
using Entities;

class Program
{
    static void Main()
    {
        int screenWidth = Raylib.GetMonitorWidth(0);
        int screenHeight = Raylib.GetMonitorHeight(0);

        Raylib.InitWindow(screenWidth, screenHeight, "Super Smash Trees 🌲");
        Raylib.ToggleFullscreen();

        Raylib.SetTargetFPS(60);

        Texture2D fondoJuego = Raylib.LoadTexture("C:\\Users\\garro\\OneDrive - Estudiantes ITCR\\Datos 1\\SuperSmashTrees\\SuperSmashTrees\\Assets\\Sprites\\Backgrounds\\GameBackground.png");

        // Cargar texturas de plataformas
        Platform.LoadTextures();

        // Crear plataformas aleatorias
        Random rng = new Random();
        List<Platform> plataformas = new List<Platform>();

        int intentos = 0;

        while (plataformas.Count < 7 && intentos < 100)
        {
            float width = rng.Next(120, 180);
            float height = 32;
            float x = rng.Next(0, Raylib.GetScreenWidth() - (int)width);
            float y = rng.Next(100, Raylib.GetScreenHeight() - 150); // evitar que queden muy abajo

            Rectangle nueva = new Rectangle(x, y, width, height);

            // Verificar que no se superponga con ninguna plataforma existente
            bool separada = true;
            foreach (var p in plataformas)
            {
                Rectangle expandida = new Rectangle(p.Rect.X - 20, p.Rect.Y - 20, p.Rect.Width + 40, p.Rect.Height + 40);

                if (Raylib.CheckCollisionRecs(expandida, nueva))
                {
                    separada = false;
                    break;
                }
            }

            if (separada)
            {
                plataformas.Add(new Platform(x, y, width, height));
            }

            intentos++;
        }


        Menu menu = new Menu();
        Player jugador1 = new Player(new Vector2(200, 300), Color.Red, KeyboardKey.W, KeyboardKey.S, KeyboardKey.A, KeyboardKey.D);
        Player jugador2 = new Player(new Vector2(500, 300), Color.Blue, KeyboardKey.Up, KeyboardKey.Down, KeyboardKey.Left, KeyboardKey.Right);

        bool jugar = false;

        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();

            Rectangle source = new Rectangle(0, 0, fondoJuego.Width, fondoJuego.Height);
            Rectangle dest = new Rectangle(0, 0, Raylib.GetScreenWidth(), Raylib.GetScreenHeight());

            Vector2 origin = new Vector2(0, 0);

            Raylib.DrawTexturePro(fondoJuego, source, dest, origin, 0, Color.White);


            // Dibujar plataformas
            foreach (var plataforma in plataformas)
            {
                plataforma.Draw();
            }



        if (!jugar)
            {
                // Mostrar el menú y esperar a que devuelva true
                jugar = menu.Mostrar();
            }
            else
            {
                Raylib.ClearBackground(Color.DarkGreen);

                jugador1.Update();
                jugador2.Update();

                jugador1.Draw();
                jugador2.Draw();

            }

            Raylib.EndDrawing();
        }

        // Liberar texturas al salir del juego
        Platform.UnloadTextures();


        Raylib.UnloadTexture(fondoJuego);
        Raylib.CloseWindow();
    }
}
