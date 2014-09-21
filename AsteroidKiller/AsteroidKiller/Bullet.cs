using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asteroid
{
    class Bullet    //TODO: Check for collision
    {
        public Texture2D texture;
        public Vector2 pos, direction;
        public float speed;
        public Rectangle hitbox;

        int hitboxOffset = 0;       //Makes hitbox smaller by number of pixels on each side
        public bool removeBullet = false;
        public bool bulletHitAsteroid = false;
        
        public Bullet(Texture2D texture, Vector2 pos, Vector2 direction, float speed, Rectangle hitbox)
        {
            this.texture = texture;
            this.pos = pos;
            this.direction = direction;
            this.speed = speed;
            this.hitbox = hitbox;
        }

        public void Update()
        {
            pos += direction * speed;

            hitbox = new Rectangle(
                (int)pos.X + hitboxOffset - texture.Width / 2,      //Texture width/height adjusted to center of sprite
                (int)pos.Y + hitboxOffset - texture.Height / 2,
                texture.Width - hitboxOffset * 2,
                texture.Height - hitboxOffset * 2); 

            if (pos.X > 1300 || pos.X < -20 || pos.Y > 740 || pos.Y < -20) //Mark bullets outside screen for removal
                removeBullet = true;
        }

        public void Collision(Rectangle asteroidHitbox)
        {
            if (asteroidHitbox.Intersects(hitbox))
            {
                bulletHitAsteroid = true;
                removeBullet = true;
            }
        }

        public Rectangle getHitbox()
        {
            return hitbox;
        }

        public void Render(SpriteBatch spritebatch)
        {
            spritebatch.Draw(texture, pos, null, Color.White, 0f, new Vector2(texture.Width / 2, texture.Height / 2), 1f, SpriteEffects.None, 0.5f);
            //spritebatch.Draw(texture, hitbox, Color.CornflowerBlue); //Draw hitbox for debugging
        }

    }

}
