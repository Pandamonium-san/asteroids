using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asteroid
{
    class Powerup
    {
        public Texture2D texture;
        public Vector2 pos, speed;
        public Rectangle hitbox;
        public int id;

        public bool playerGotPowerup = false;

        public Powerup(Texture2D texture, Vector2 pos, Vector2 speed, Rectangle hitbox, int id)
        {
            this.texture = texture;
            this.pos = pos;
            this.speed = speed;
            this.hitbox = hitbox;
            this.id = id;
        }

        public void Update()
        {
            pos += speed;
            
            hitbox = new Rectangle(                    
                (int)pos.X - (int)24 / 2,                
                (int)pos.Y - (int)24 / 2,
                (int)24,
                (int)24);

            if (pos.X > 1280 && speed.X > 0)
                speed.X = speed.X * -1;
            if (pos.X < 0 && speed.X < 0)
                speed.X = speed.X * -1;
            if (pos.Y > 720 && speed.Y > 0)
                speed.Y = speed.Y * -1;
            if (pos.Y < 0 && speed.Y < 0)
                speed.Y = speed.Y * -1;

            if (pos.X > 1480 || pos.X < -200 || pos.Y > 930 || pos.Y < -200)
                playerGotPowerup = true;

        }

        public void Collision(Rectangle objectHitbox)
        {
            if (objectHitbox.Intersects(hitbox))
            {
                playerGotPowerup = true;
            }

        }

        public void Render(SpriteBatch spritebatch)
        {
            int frameWidth = 24;
            spritebatch.Draw(texture, pos, new Rectangle(frameWidth * (id - 1), 0, 24, 24), Color.White, 0f, new Vector2(24 / 2, 24 / 2), 1f, SpriteEffects.None, 1f);
            //spritebatch.Draw(texture, hitbox, Color.Red); //Draw hitbox
        }
        public Rectangle getHitbox()
        {
            return hitbox;
        }
    }
}
