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
    public enum VertState
    {
        None = -1,
        Ground,
        Air,
        Cling
    }

    public enum State
    {
        None = -1,
        Idle,
        Walk,
        Jump,
        Climb,
        Detach,
        Shoot
    }

    public class Player
        : ActiveUnit
    {

        private VertState
            _vertState;

        private State
            _state;

        private float
            _gravity,
            _jumpVel,
            _speed;

        public Player(Vector2 pos)
            : base(Assets.Animations["attackflower-bright-idle"], pos, Vector2.Zero, Vector2.Zero, Color.White)
        {
            _gravity = 0.2f;
            _jumpVel = 4.0f;
            _speed = 10.0f;

            Acc.Y = _gravity;

            _vertState = VertState.Air;
            _state = State.Jump;
        }

        public override void Update(object sender, UpdateData data)
        {
            base.Update(sender, data);

            if (Pos.Y > Core.HEIGHT)
            {
                Pos.Y = Core.HEIGHT;
                Vel = Vector2.Zero;
                _vertState = VertState.Ground;
            }
        }

        public override void InputPressed(object sender, InputData data)
        {
        }

        public override void InputReleased(object sender, InputData data)
        {
        }

        public override void InputHeld(object sender, InputData data)
        {
            switch (data.Input)
            {
                case GameInputs.Jump:

                    if (_vertState == VertState.Ground)
                    {
                        Vel.Y -= _jumpVel;
                        _vertState = VertState.Air;
                        _state = State.Jump;
                    }

                    break;
                case GameInputs.Left:

                    Vel.X = -_speed;

                    break;
                case GameInputs.Right:

                    Vel.X = _speed;

                    break;
            }
        }
    }
}
