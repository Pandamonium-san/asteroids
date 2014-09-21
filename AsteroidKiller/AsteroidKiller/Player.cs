using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asteroid
{
    class Player    //TODO: Ship explosion. Maybe acceleration.
    {
        public Texture2D texture;
        public Vector2 pos, speed, acc;
        public float angle;
        public Rectangle hitbox;

        public bool dead = false;
        public bool invulnerable = false;

        public int animationTime, fCount, nFrame, frameWidth;

        float maxSpeed = 8;
        float friction = 0.04f;

        int hitboxOffset = 6;  //Makes hitbox smaller on every side by number of pixels

        public Player(Texture2D texture, Vector2 pos, Vector2 speed, Vector2 acc)
        {
            this.texture = texture;
            this.pos = pos;
            this.speed = speed;
            this.acc = acc;
        }

        public void Update()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            if (!dead)
            {
                //Player bounce on edge logic
                if (pos.Y < 0)
                    speed.Y = -speed.Y * 0.5f + 1;
                if (pos.X < 0)
                    speed.X = -speed.X * 0.5f + 1;
                if (pos.Y > 720)
                    speed.Y = -speed.Y * 0.5f - 1;
                if (pos.X > 1280)
                    speed.X = -speed.X * 0.5f - 1;

                if (speed.X > maxSpeed)
                    speed.X = maxSpeed;
                if (speed.X < -maxSpeed)
                    speed.X = -maxSpeed;
                if (speed.Y > maxSpeed)
                    speed.Y = maxSpeed;
                if (speed.Y < -maxSpeed)
                    speed.Y = -maxSpeed;

                pos += speed;

                if (speed.X > 0)
                    speed.X -= friction;
                if (speed.X < 0)
                    speed.X += friction;
                if (speed.Y > 0)
                    speed.Y -= friction;
                if (speed.Y < 0)
                    speed.Y += friction;

                if (keyboardState.IsKeyDown(Keys.W))
                    speed.Y -= acc.Y;
                if (keyboardState.IsKeyDown(Keys.A))
                    speed.X -= acc.X;
                if (keyboardState.IsKeyDown(Keys.S))
                    speed.Y += acc.Y;
                if (keyboardState.IsKeyDown(Keys.D))
                    speed.X += acc.X;


                MouseState mouseState = Mouse.GetState();
                Vector2 mousePos = new Vector2(mouseState.X, mouseState.Y);

                Vector2 direction = mousePos - pos;

                angle = (float)(Math.Atan2(direction.Y, direction.X)) + (float)Math.PI / 2;

                hitbox = new Rectangle(
                    (int)pos.X + hitboxOffset - texture.Width / 4,  // Minus width/height in order to make hitbox match sprite
                    (int)pos.Y + hitboxOffset - texture.Height / 2,
                    texture.Width / 2 - hitboxOffset * 2,
                    texture.Height - hitboxOffset * 2);

            }

            nFrame += 1;    //Counter for sprite animation
            if (dead)
            {
                animationTime += 1;     //Counter for explosion animation duration
            }
        }

        public Vector2 getPos()
        {
            return pos;
        }
        

        public Rectangle getHitbox()
        {
            return hitbox;
        }

        public void Collision(Rectangle asteroidHitbox)
        {
            if (asteroidHitbox.Intersects(hitbox) && !invulnerable)
                dead=true;
        }

        public void Render(SpriteBatch spritebatch)
        {

            if (dead)
            {
                frameWidth = 59;

                if (nFrame >= 5)
                {
                    if (fCount < 8)
                        ++fCount;
                    nFrame = 0;
                }
                spritebatch.Draw(texture, pos, new Rectangle(frameWidth * fCount, 0, 59, 55), Color.White, 0f, new Vector2(59 / 2, 55 / 2), 1f, SpriteEffects.None, 1f);
            }

            if (!dead)
            {
                frameWidth = 33;
                if (fCount > 1)
                    fCount = 0;
                if (nFrame >= 10)
                {
                    if (fCount < 1)
                        ++fCount;
                    else
                        --fCount;
                    nFrame = 0;
                }

                if (!invulnerable)
                    spritebatch.Draw(texture, pos, new Rectangle(frameWidth * fCount, 0, 31, 27), Color.White, angle, new Vector2(31 / 2, 26 / 2), 1f, SpriteEffects.None, 0f);
                if (invulnerable)
                    spritebatch.Draw(texture, pos, new Rectangle(frameWidth * fCount, 0, 31, 27), Color.CornflowerBlue*0.8f, angle, new Vector2(31 / 2, 26 / 2), 1f, SpriteEffects.None, 0f);
            }
           // spritebatch.Draw(texture, pos, new Rectangle(59, 0, 59, 55), Color.Red); //Draw hitbox for debugging
        }

    }
}
