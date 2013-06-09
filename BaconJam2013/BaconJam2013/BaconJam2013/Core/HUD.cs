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
using Microsoft.Xna.Framework.Media;

namespace BaconJam2013
{
    class HUD
    {

        private Animation
            _border;

        public HUD()
        {
            _border = Assets.Animations["hud-border"];
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_border.Frame(0).Texture, Vector2.Zero, Color.White);
        }
    }
}
