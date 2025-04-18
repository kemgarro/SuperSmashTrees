using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;
using System.Numerics;




namespace UI
{
    public class Menu
    {
        private readonly string[] opciones = { "START", "EXIT" };
        private int seleccion = 0;

        private Texture2D fondo;

        public Menu()
        {
            fondo = Raylib.LoadTexture("Assets/Sprites/Backgrounds/MenuBackground.png");
        }

        public bool Mostrar()
        {
            Rectangle source = new Rectangle(0, 0, fondo.Width, fondo.Height);
            Rectangle dest = new Rectangle(0, 0, Raylib.GetScreenWidth(), Raylib.GetScreenHeight());
            Vector2 origin = new Vector2(0, 0);

            Raylib.DrawTexturePro(fondo, source, dest, origin, 0, Color.White);



            for (int i = 0; i < opciones.Length; i++)
            {
                int width = 180;
                int height = 50;
                int y = 530;
                int x = (i == 0) ? 175 : 440;

                Rectangle boton = new Rectangle(x, y, width, height);
                bool mouseSobre = Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), boton);

                // Cambiar selección si el mouse está encima
                if (mouseSobre)
                    seleccion = i;

                // Dibujar botón
                Color bgColor = mouseSobre ? Color.DarkGreen : (i == seleccion ? Color.DarkGreen : Color.Beige);
                Raylib.DrawRectangleRec(boton, bgColor);
                Raylib.DrawRectangleLinesEx(boton, 2, Color.Black);

                // Texto centrado
                int textWidth = Raylib.MeasureText(opciones[i], 24);
                int textX = x + (width - textWidth) / 2;
                int textY = y + (height - 24) / 2;

                Color textColor = (i == seleccion) ? Color.White : Color.Black;
                Raylib.DrawText(opciones[i], textX, textY, 24, textColor);

                // Clic con mouse
                if (mouseSobre && Raylib.IsMouseButtonPressed(MouseButton.Left))
                {
                    if (opciones[seleccion] == "Salir") Raylib.CloseWindow();
                    return true;
                }
            }




            // Navegación con teclas
            if (Raylib.IsKeyPressed(KeyboardKey.Down)) seleccion++;
            if (Raylib.IsKeyPressed(KeyboardKey.Up)) seleccion--;

            if (seleccion < 0) seleccion = opciones.Length - 1;
            if (seleccion >= opciones.Length) seleccion = 0;

            // Enter = seleccionar
            if (Raylib.IsKeyPressed(KeyboardKey.Enter))
            {
                if (opciones[seleccion] == "Salir") Raylib.CloseWindow();
                return true; // empieza el juego
            }

            return false;
        }
        
    }
}


