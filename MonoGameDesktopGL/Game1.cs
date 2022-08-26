using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System.IO;
using MonoGameDesktopGL;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Timers;
using System.Threading.Tasks;

namespace MonoGameDesktopDX
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        int width = 600;
        int height = 600;
        Texture2D tex;
        Rectangle rect;
        float rotation;
        float velocidadRot = 2f;
        Vector2 Origin;


        bool colision = false;

        //ENEMIGOS
        List<Enemigo> listaEnemigos = new List<Enemigo>();
        Random random = new Random();
        int contador = 0;
        int limite = 30;

        //DISPAROS
        List<Disparo> listaDisparos = new List<Disparo>();
        Vector2 posicionDisparo;
        int contadorDisparos = 0;
        int limiteDisparos = 10;

        //MOUSE
        MouseState raton = new MouseState();

        //FONDO BACKGROUND
        Texture2D fondo;
        Rectangle rectFondo;

        //FUENTE
        SpriteFont fuente;

        //PUNTAJE
        int puntaje = 0;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            VariablesVentana();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            CargarFondo();
            CargarFuente();
            CrearHeroe();
            //CrearEnemigosIniciales();
            // TODO: load content here
        }

        protected override void UnloadContent()
        {
            // TODO: Unload content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (!colision)
            {
                MovimientosTeclado();
            }
            ActualizarEnemigos();
            EliminarEnemigos();
            ComprobarVida();
            DetectarColision();
            Conteo();
            //DetectarRaton();
            DetectarDisparo();
            ActualizarDisparos();
            LimpiarDisparos();
            ClearDisparos();
            MovimientosTeclado();
            base.Update(gameTime);

        }

        protected override void Draw(GameTime time)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            //TODO: Draw your game
            spriteBatch.Begin();
            spriteBatch.Draw(fondo, rectFondo, Color.White);
            DibujarEnemigos();
            DibujarDisparos();
            Origin = new Vector2(tex.Width / 2, tex.Height / 2);
            spriteBatch.Draw(tex, rect, null, Color.White, rotation, Origin,SpriteEffects.None,0f);
            spriteBatch.DrawString(fuente, "Puntaje: " + puntaje.ToString(), Vector2.Zero, Color.White);
            spriteBatch.End();
            base.Draw(time);
        }
        private void ClearDisparos()
        {
            if (listaDisparos.Count > 1)
            {
                listaDisparos.Remove(listaDisparos[0]);
            }
        }

        private void LimpiarDisparos()
        {
            for(int i = 0; i < listaDisparos.Count; i++)
            {
                if (!listaDisparos[i].estaVivo)
                {
                    listaDisparos.Remove(listaDisparos[i]);
                }
            }
        }
        private void DibujarDisparos()
        {
            for(int i = 0; i < listaDisparos.Count; i++)
            {
                listaDisparos[i].Draw(rotation);
            }
        }
        private void ActualizarDisparos()
        {
            for(int i = 0; i < listaDisparos.Count; i++)
            {
                var direction = new Vector2((float)Math.Cos(MathHelper.ToRadians(90) - rotation), -(float)Math.Sin(MathHelper.ToRadians(90) - rotation));
                listaDisparos[i].Update(direction*12f);
            }
        }
        private void Disparar()
        {
            posicionDisparo = new Vector2((rect.Width/2 + rect.X), rect.Y);
            FileStream imagenDisparo = new FileStream("Content/bala.png", FileMode.Open);
            Texture2D texDisparo = Texture2D.FromStream(GraphicsDevice, imagenDisparo);
            imagenDisparo.Dispose();

            Disparo nuevoDisparo = new Disparo(texDisparo, posicionDisparo, spriteBatch);
            listaDisparos.Add(nuevoDisparo);
            Task.Delay(new TimeSpan(0, 0, 5)).ContinueWith(o => { ClearDisparos(); });
            GC.Collect();
        }
        private void DetectarDisparo()
        {
            contadorDisparos++;

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                if(contadorDisparos >= limiteDisparos)
                {
                    Disparar();
                    contadorDisparos = 0;
                }                
            }
        }
        private void CargarFuente()
        {
            fuente = Content.Load<SpriteFont>("FONT");
        }
        private void CargarFondo()
        {
            FileStream imagenFondo = new FileStream("Content/bg2.jpg", FileMode.Open);
            fondo = Texture2D.FromStream(GraphicsDevice, imagenFondo);
            imagenFondo.Dispose();
            rectFondo = new Rectangle(0,0, width, height);
        }
        private void DetectarRaton()
        {
            //var direction = new Vector2((float)Math.Cos(MathHelper.ToRadians(90) - rotation), -(float)Math.Sin(MathHelper.ToRadians(90) - rotation));
            raton = Mouse.GetState();
            rect.X = raton.Position.X;
            rect.Y = raton.Position.Y;
            if (raton.LeftButton == ButtonState.Pressed)
            {
                rotation += MathHelper.ToRadians(velocidadRot);
            }else if(raton.RightButton == ButtonState.Pressed)
            {
                rotation -= MathHelper.ToRadians(velocidadRot);
            }
        }
        private void EliminarEnemigos()
        {
            for(int i = 0; i < listaEnemigos.Count; i++)
            {
                if(listaEnemigos[i].rect.Y > height)
                {
                    listaEnemigos[i].Eliminar();
                }
            }
        }
        private void ComprobarVida()
        {
            for(int i = 0; i < listaEnemigos.Count; i++)
            {
                if (!listaEnemigos[i].estaVivo)
                {
                    listaEnemigos.Remove(listaEnemigos[i]);
                }
            }
        }
        private void DibujarEnemigos()
        {
            for (int i = 0; i < listaEnemigos.Count; i++)
            {
                listaEnemigos[i].Draw();
            }
        }
        private void ActualizarEnemigos()
        {
            for (int i = 0; i < listaEnemigos.Count; i++)
            {
                listaEnemigos[i].Update();
            }
        }
        private void VariablesVentana()
        {
            graphics.PreferredBackBufferWidth = width;
            graphics.PreferredBackBufferHeight = height;
            graphics.IsFullScreen = false;
            IsMouseVisible = true;
            graphics.ApplyChanges();
        }
        private void CrearHeroe()
        {
            FileStream imagenChoco = new FileStream("Content/nave.png", FileMode.Open);
            tex = Texture2D.FromStream(GraphicsDevice, imagenChoco);
            imagenChoco.Dispose();
            rect = new Rectangle(100, 300, 50, 50);
        }
        private void CrearEnemigosIniciales()
        {
            FileStream imagenEnemigo = new FileStream("Content/enemy1.png", FileMode.Open);
            for (int i = 0; i < 5; i++)
            {
                Texture2D texEnemigo = Texture2D.FromStream(GraphicsDevice, imagenEnemigo);
                Vector2 posicionEnemigo = new Vector2(random.Next(0, width), -200);
                Vector2 velocidadEnemigo = new Vector2(0, random.Next(1, 3));
                Enemigo nuevoEnemigo = new Enemigo(
                    texEnemigo,
                    posicionEnemigo,
                    velocidadEnemigo,
                    spriteBatch);

                listaEnemigos.Add(nuevoEnemigo);
                GC.Collect();
            }
            imagenEnemigo.Dispose();
        }
        private void Conteo()
        {
            contador++;
            if(contador >= limite)
            {
                CrearEnemigo();
                contador = 0;
            }
        }
        private void CrearEnemigo()
        {
            int texAUsar = random.Next(0, 3);
            FileStream imagenEnemigo;

            if (texAUsar == 0)
            {
                imagenEnemigo = new FileStream("Content/enemy1.png", FileMode.Open);
            }
            else if (texAUsar == 1)
            {
                imagenEnemigo = new FileStream("Content/enemy2.png", FileMode.Open);
            }
            else
            {
                imagenEnemigo = new FileStream("Content/enemy3.png", FileMode.Open);
            }

            
            Texture2D texEnemigo = Texture2D.FromStream(GraphicsDevice, imagenEnemigo);
            Vector2 posicionEnemigo = new Vector2(random.Next(0, width), -200);
            Vector2 velocidadEnemigo = new Vector2(0, random.Next(1, 3));
            Enemigo nuevoEnemigo = new Enemigo(
                 texEnemigo,
                 posicionEnemigo,
                 velocidadEnemigo,
                 spriteBatch);
            listaEnemigos.Add(nuevoEnemigo);            
            imagenEnemigo.Dispose();
            GC.Collect();
        }
        private void DetectarColision()
        {
            for(int i = 0; i < listaEnemigos.Count; i++)
            {
                for (int f = 0; f < listaDisparos.Count; f++)
                {
                    if (listaEnemigos[i].rect.Intersects(listaDisparos[f].rect))
                    {
                        listaEnemigos[i].Eliminar();
                        listaDisparos[f].Eliminar();
                        puntaje++;
                        break;
                    }
                }                
            }
        }
        private void MovimientosTeclado()
        {
            var direction = new Vector2((float)Math.Cos(MathHelper.ToRadians(90) - rotation), -(float)Math.Sin(MathHelper.ToRadians(90) - rotation));
            FileStream movimiento;
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                if((int)Math.Round(direction.X * velocidadRot) == 0)
                {
                    movimiento = new FileStream("Content/naveder.png", FileMode.Open);
                    tex = Texture2D.FromStream(GraphicsDevice, movimiento);
                    movimiento.Dispose();
                    rect.X += 3;
                }
                else
                {
                    movimiento = new FileStream("Content/naveder.png", FileMode.Open);
                    tex = Texture2D.FromStream(GraphicsDevice, movimiento);
                    movimiento.Dispose();
                    rect.X -= (int)Math.Round(direction.X * velocidadRot);
                    rect.Y += (int)Math.Round(direction.Y * velocidadRot);
                }

            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                movimiento = new FileStream("Content/naveizq.png", FileMode.Open);
                tex = Texture2D.FromStream(GraphicsDevice, movimiento);
                movimiento.Dispose();
                rect.X += (int)Math.Round(direction.X * velocidadRot);
                rect.Y -= (int)Math.Round(direction.Y * velocidadRot);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                    movimiento = new FileStream("Content/nave.png", FileMode.Open);
                    tex = Texture2D.FromStream(GraphicsDevice, movimiento);
                    movimiento.Dispose();
                    rect.Y += (int)Math.Round(direction.Y * velocidadRot);
                    rect.X += (int)Math.Round(direction.X * velocidadRot);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                    movimiento = new FileStream("Content/nave.png", FileMode.Open);
                    tex = Texture2D.FromStream(GraphicsDevice, movimiento);
                    movimiento.Dispose();
                    rect.Y -= (int)Math.Round(direction.Y * velocidadRot);
                    rect.X -= (int)Math.Round(direction.X * velocidadRot);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Q))
            {
                rotation+=MathHelper.ToRadians(velocidadRot);
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.E))
            {
                rotation -= MathHelper.ToRadians(velocidadRot);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }
        }
    }
}