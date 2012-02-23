using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;

namespace DartEm
{
    class Dart
    {
        private Vector2 position;
        private int spriteStage;

        public Dart(Vector2 input)
        {
            spriteStage = 1;
            position = input;
        }
        public void setPosition(Vector2 input)
        {
            position = input;
        }
        public Vector2 getPosition()
        {
            return position;
        }

        public void setSpriteStage(int input)
        {
            spriteStage = input;
        }
        public int getSpriteStage()
        {
            return spriteStage;
        }
    }
}
