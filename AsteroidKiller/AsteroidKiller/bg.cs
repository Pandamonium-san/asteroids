using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Asteroid
{
    class bg
    {
        Texture2D texture;
        Rectangle rectangle1, rectangle2;
        float alpha;
        int scrollSpeed;

 
          public bg(Texture2D texture, Rectangle rectangle1, Rectangle rectangle2, float alpha, int scrollSpeed)
        {
            this.texture = texture;
            this.rectangle1 = rectangle1;
            this.rectangle2 = rectangle2;
            this.alpha = alpha;
            this.scrollSpeed = scrollSpeed;
        }

        public void Update()
          {
              if (rectangle1.Y - texture.Height >= 0)
                  rectangle1.Y = rectangle2.Y - texture.Height;
              if (rectangle2.Y - texture.Height >= 0)
                  rectangle2.Y = rectangle1.Y - texture.Height;

                  rectangle1.Y += scrollSpeed-1;
                  rectangle2.Y += scrollSpeed-1;

          }

        public void Render(SpriteBatch spritebatch)
        {
            spritebatch.Draw(texture, rectangle1, Color.White*alpha);
            spritebatch.Draw(texture, rectangle2, Color.White*alpha);
        }
    }
}
