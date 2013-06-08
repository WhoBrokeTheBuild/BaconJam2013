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
            _jumpVelMax,
            _jumpVelStart,
            _jumpVelLeft,
            _moveAcc,
            _speedMax,
            _damping,
            _airDamping;

        public Player(Vector2 pos)
            : base(Assets.Animations["attackflower-bright-idle"], pos, Vector2.Zero, Vector2.Zero, Color.White)
        {
            _gravity = 0.8f;

            _jumpVelStart = 12.0f;
            _jumpVelMax = 20.0f;
            _jumpVelLeft = _jumpVelMax;

            _damping = 1.0f;
            _airDamping = 1.5f;

            _moveAcc = 2.0f;
            _speedMax = 5.0f;

            _vertState = VertState.Air;
            _state = State.Jump;
        }

        public override void Update(object sender, UpdateData data)
        {
            base.Update(sender, data);

            if (_vertState == VertState.Air)
            {
                Vel.Y += _gravity;

                if (Pos.Y > Core.HEIGHT)
                {
                    Pos.Y = Core.HEIGHT;
                    Vel = Vector2.Zero;
                    _vertState = VertState.Ground;
                }
            }

            if (Vel.X != 0.0f)
            {
                float damp;

                if (_vertState == VertState.Ground)
                    damp = _damping;
                else
                    damp = _airDamping;

                Vel.X -= damp * Util.Sign(Vel.X);
            }

            if (Math.Abs(Vel.X) > _speedMax)
                Vel.X = _speedMax * Util.Sign(Vel.X);
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
                        Vel.Y -= _jumpVelStart;
                        _jumpVelLeft = _jumpVelMax - _jumpVelStart;
                    }

                    break;
                case GameInputs.Left:

                    Vel.X -= _moveAcc;

                    break;
                case GameInputs.Right:

                    Vel.X += _moveAcc;

                    break;
            }
        }
    }
}
