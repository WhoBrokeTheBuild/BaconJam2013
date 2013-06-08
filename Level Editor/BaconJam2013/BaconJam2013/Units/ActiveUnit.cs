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
    public class ActiveUnit
        : BasicUnit
    {

        public Vector2
            Vel,
            Acc;

        public const float
            TERMINAL_VEL = 25.0f;

        public ActiveUnit(Animation animation, Vector2 pos, Vector2 vel, Vector2 acc, Color blendColor, float rot = 0.0f, float depth = 1.0f)
            : base(animation, pos, blendColor, rot, depth)
        {
            Vel = vel;
            Acc = acc;

            Input.PressedEvent += InputPressed;
            Input.ReleasedEvent += InputReleased;
            Input.HeldEvent += InputHeld;
        }

        public override void Update(object sender, UpdateData data)
        {
            base.Update(sender, data);

            Vel += Acc;

            if (Vel.X > TERMINAL_VEL)
                Vel.X = TERMINAL_VEL;

            if (Vel.Y > TERMINAL_VEL)
                Vel.Y = TERMINAL_VEL;

            Pos += Vel;
        }

        public virtual void InputPressed(object sender, InputData data)
        {
        }

        public virtual void InputReleased(object sender, InputData data)
        {
        }

        public virtual void InputHeld(object sender, InputData data)
        {
        }

    }
}
