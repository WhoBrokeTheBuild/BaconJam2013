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
            : base(Assets.Animations["attackflower-dark-idle"], pos, Vector2.Zero, Vector2.Zero, Color.White)
        {
            _gravity = Config.GetFloat("Gravity");

            _jumpVelStart = Config.GetFloat("JumpVelStart");
            _jumpVelMax = Config.GetFloat("JumpVelMax");
            _jumpVelLeft = _jumpVelMax;

            _damping = Config.GetFloat("Damping");
            _airDamping = Config.GetFloat("AirDamping");

            _moveAcc = Config.GetFloat("MovementAcc");
            _speedMax = Config.GetFloat("MaxSpeed");

            _vertState = VertState.Air;
            _state = State.Jump;
        }

        public override void Update(object sender, UpdateData data)
        {
            base.Update(sender, data);

            if (_vertState == VertState.Air)
            {
                Vel.Y -= _gravity;

                if (Pos.Y > Core.HEIGHT)
                {
                    Vel.Y = 0;
                    Pos.Y = Core.HEIGHT;
                    _state = State.Idle;
                    _vertState = VertState.Ground;
                }
            }

            if (Math.Abs(Vel.X) < 0.01f)
                Vel.X = 0.0f;

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
            switch (data.Input)
            {
                case GameInputs.Jump:

                    if (_vertState == VertState.Ground)
                    {
                        Vel.Y -= _jumpVelStart;
                        _jumpVelLeft = _jumpVelMax - _jumpVelStart;
                        _vertState = VertState.Air;
                    }

                    break;
            }
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
                        if (_jumpVelLeft > 0.0f)
                        {
                            //Vel.Y -= 1.0f;
                            _jumpVelLeft -= 1.0f;
                        }
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
