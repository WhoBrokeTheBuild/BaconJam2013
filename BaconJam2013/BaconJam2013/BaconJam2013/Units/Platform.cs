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
    public class Platform
    {

        public bool
            JumpThrough,
            Ramp;

        public Direction
            RampDir;

        public Vector2
            Pos,
            Size;

        public Platform(Vector2 pos, Vector2 size, bool jumpThrough = false)
        {
            Pos = pos;
            Size = size;
            JumpThrough = jumpThrough;
            Ramp = false;
            RampDir = Direction.None;

            Core.RenderEvent += Render;
        }

        public Platform(Vector2 pos, Vector2 size, Direction dir)
        {
            Pos = pos;
            Size = size;
            JumpThrough = false;
            Ramp = true;
            RampDir = dir;

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

            Color col = Color.Red;

            if (Ramp)
            {
                if (RampDir == Direction.West)
                {
                    col = Color.Blue;
                }
                else if (RampDir == Direction.East)
                {
                    col = Color.Green;
                }
            }

            if (JumpThrough)
            {
                col = Color.Pink;
            }

            data.SpriteBatch.Draw(Assets.Animations["tile-placeholder"].Frame(0).Texture, bounds, new Rectangle(0, 0, 1, 1), col, 0.0f, Vector2.Zero, SpriteEffects.None, 1.0f);
        }

    }
}
