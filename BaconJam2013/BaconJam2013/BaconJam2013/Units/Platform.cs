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
    class Platform
    {

        public bool
            JumpThrough;

        public Vector2
            Pos,
            Size;

        public Platform(Vector2 pos, Vector2 size, bool jumpThrough = false)
        {
            Pos = pos;
            Size = size;
            JumpThrough = jumpThrough;

            Core.RenderEvent += Render;
        }

        public Rectangle Bounds()
        {
            return new Rectangle((int)Pos.X, (int)Pos.Y, (int)Size.X, (int)Size.Y);
        }

        public void Render(object sender, RenderData data)
        {
            Rectangle bounds = Bounds();

            bounds.X -= (int)Viewport.Pos.X;
            bounds.Y -= (int)Viewport.Pos.Y;

            data.SpriteBatch.Draw(Assets.Animations["tile-placeholder"].Frame(0).Texture, bounds, Color.Red);
        }

    }
}
