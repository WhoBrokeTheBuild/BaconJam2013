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
    class Viewport
    {

        public static Vector2
            Pos;

        private static BasicUnit
            _follow;

        public Viewport()
        {
            _follow = null;
            Pos = Vector2.Zero;

            Core.UpdateEvent += Update;
        }

        public static void Follow(BasicUnit unit)
        {
            _follow = unit;
        }

        public void Update(object sender, UpdateData data)
        {
            if (_follow == null)
                return;

            Pos.X = (float)(_follow.Pos.X - (Core.Width / 2));
            Pos.Y = (float)(_follow.Pos.Y - (Core.Height / 2));
        }

    }
}
