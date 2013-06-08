using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace BaconJam2013
{
    public class Player
        : ActiveUnit
    {

        private float
            _gravity,
            _jumpVel,
            _speed;

        public Player(Vector2 pos)
            : base(Assets.Animations["attackflower-bright-idle"], pos, Vector2.Zero, Vector2.Zero, Color.White)
        {
            _gravity = 0.2f;
            _jumpVel = 5.0f;
            _speed = 10.0f;

            Acc.Y = _gravity;
        }

        public override void Update(object sender, UpdateData data)
        {
            base.Update(sender, data);

            if (Pos.Y > Core.HEIGHT)
            {
                Pos.Y = Core.HEIGHT;
                Vel = Vector2.Zero;
            }
        }

        public override void InputPressed(object sender, InputData data)
        {
            switch (data.Input)
            {
                case GameInputs.Jump:

                    Vel.Y -= _jumpVel;

                    break;
            }
        }

        public override void InputReleased(object sender, InputData data)
        {
        }

        public override void InputHeld(object sender, InputData data)
        {
        }
    }
}
