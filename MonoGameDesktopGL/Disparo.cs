using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGameDesktopGL
{
    public class Disparo
    {
        Texture2D tex;
        Vector2 posicion;
        Vector2 origin;
        public Rectangle rect;
        SpriteBatch spriteBatch;
        public bool estaVivo = true;

        public Disparo(
            Texture2D _tex,
            Vector2 _posicion,
            SpriteBatch _spriteBatch)
        {
            tex = _tex;
            posicion = _posicion;
            spriteBatch = _spriteBatch;

            rect = new Rectangle((int)posicion.X, (int)posicion.Y, 20, 20);
        }

        public void Update(Vector2 pos)
        {
            rect.X +=(int) pos.X;
            rect.Y += (int) pos.Y;
        }

        public void Draw(float rotation)
        {
            spriteBatch.Draw(tex, rect, null, Color.White, rotation, origin, SpriteEffects.None, 0f);
        }

        public void Eliminar()
        {
            estaVivo = false;
        }
    }
}
