using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Asteroid
{
    class Asteroid
    {
        public Texture2D texture;
        public Vector2 pos, speed;
        public Rectangle hitbox;
        public float scale, RotationAngle;

        public bool spawnDaughters = false;
        public bool destroyAsteroid = false;

        public int animationTime, fCount, nFrame;

        public Asteroid(Texture2D texture, Vector2 pos, Vector2 speed, Rectangle hitbox, float scale)
        {
            this.texture = texture;
            this.pos = pos;
            this.speed = speed;
            this.hitbox = hitbox;
            this.scale = scale;
        }

        public void Update()
        {
            RotationAngle += 0.01f * speed.Length();
            pos += speed;

            if (speed.X == 0)   //Prevents asteroids from being static or only moving in one dimension
                speed.X = 1;
            if (speed.Y == 0)
                speed.Y = 1;

            if (!destroyAsteroid)
            {
                int hitboxOffset = (int)(texture.Width * scale * 0.1f);             //Make hitbox smaller than sprite
                hitbox = new Rectangle(
                    (int)pos.X + hitboxOffset - (int)(texture.Width * scale) / 2,
                    (int)pos.Y + hitboxOffset - (int)(texture.Height * scale) / 2,
                    (int)(scale * texture.Width) - hitboxOffset * 2,
                    (int)(scale * texture.Height) - hitboxOffset * 2);
            }
            if (destroyAsteroid)
                hitbox = new Rectangle(0, 0, 0, 0);

            if ((pos.X > 1280 && speed.X > 0) || (pos.X < 0 && speed.X < 0))            //Bounce on edge logic
                speed.X = speed.X * -1;
            if ((pos.Y > 720 && speed.Y > 0) || (pos.Y < 0 && speed.Y < 0))
                speed.Y = speed.Y * -1;

            if (pos.X > 1480 || pos.X < -200 || pos.Y > 930 || pos.Y < -200)    //Removes asteroids far outside screen in case they make it out
                destroyAsteroid = true;

            if (destroyAsteroid)        //Starts the counters for the exploding asteroid animation
            {
                animationTime += 1;
                nFrame += 1;
            }

        }

        public void Collision(Rectangle objectHitbox)
        {
            if (objectHitbox.Intersects(hitbox))
            {
                if (scale > 0.6 && !destroyAsteroid)
                    spawnDaughters = true;
                destroyAsteroid = true;
            }
                
        }

        public void Render(SpriteBatch spritebatch)
        {
                    if(!destroyAsteroid)
                    spritebatch.Draw(texture, pos, null, Color.White, RotationAngle, new Vector2(texture.Width / 2, texture.Height / 2), scale, SpriteEffects.None, 1f);
                    if (destroyAsteroid)
                    {

                        int frameWidth = 93;

                        if (nFrame >= 5)    //nFrame decides time between animation frames
                        {
                            if (fCount < 3)
                                ++fCount;
                            nFrame = 0;
                        }
                        spritebatch.Draw(texture, pos, new Rectangle(37+frameWidth*fCount, 399, 93, 91), Color.White, 0f, new Vector2(89/2,89/2), scale, SpriteEffects.None, 1f);

                    }
                    //spritebatch.Draw(texture, hitbox, Color.Red); //Draw hitbox
        }
        public Rectangle getHitbox()
        {
            return hitbox;
        }

    }
}
