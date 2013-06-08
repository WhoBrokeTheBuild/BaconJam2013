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

        public VertState
            VertState;

        public State
            State;

        private float
            _gravity,
            _jumpVelMax,
            _jumpVelStart,
            _jumpVelLeft,
            _moveAcc,
            _speedMax,
            _damping,
            _airDamping,
            _minSpeedThreshold,
            _floor;

        public Player(Vector2 pos)
            : base(Assets.Animations["attackflower-bright-idle"], pos, Vector2.Zero, Vector2.Zero, Color.White)
        {
            _gravity = Config.GetFloat("Gravity");

            _jumpVelStart = Config.GetFloat("JumpVelStart");
            _jumpVelMax = Config.GetFloat("JumpVelMax");
            _jumpVelLeft = _jumpVelMax;

            _damping = Config.GetFloat("Damping");
            _airDamping = Config.GetFloat("AirDamping");

            _moveAcc = Config.GetFloat("MovementAcc");
            _speedMax = Config.GetFloat("MaxSpeed");

            _minSpeedThreshold = Config.GetFloat("MinSpeedThreshold");

            _floor = 2048;

            VertState = VertState.Air;
            State = State.Jump;
        }

        public override void Update(object sender, UpdateData data)
        {
            base.Update(sender, data);

            if (VertState == VertState.Air)
            {
                Vel.Y -= _gravity;

                if (Pos.Y > _floor)
                {
                    Vel.Y = 0;
                    Pos.Y = _floor;
                    State = State.Idle;
                    VertState = VertState.Ground;
                }
            }

            if (Math.Abs(Vel.X) < _minSpeedThreshold)
                Vel.X = 0.0f;

            if (Vel.X != 0.0f)
            {
                float damp;

                if (VertState == VertState.Ground)
                    damp = _damping;
                else
                    damp = _airDamping;

                Vel.X -= damp * Util.Sign(Vel.X);
            }

            if (Math.Abs(Vel.X) > _speedMax)
                Vel.X = _speedMax * Util.Sign(Vel.X);
        }

        public override void Render(object sender, RenderData data)
        {
            base.Render(sender, data);

            Rectangle bounds = Bounds();

            data.SpriteBatch.Draw(Assets.Animations["tile-placeholder"].Frame(0).Texture, new Vector2(bounds.Left, bounds.Top) - Viewport.Pos, Color.Red);
            data.SpriteBatch.Draw(Assets.Animations["tile-placeholder"].Frame(0).Texture, new Vector2(bounds.Left, bounds.Bottom) - Viewport.Pos, Color.Red);
            data.SpriteBatch.Draw(Assets.Animations["tile-placeholder"].Frame(0).Texture, new Vector2(bounds.Right, bounds.Top) - Viewport.Pos, Color.Red);
            data.SpriteBatch.Draw(Assets.Animations["tile-placeholder"].Frame(0).Texture, new Vector2(bounds.Right, bounds.Bottom) - Viewport.Pos, Color.Red);
        }

        public override void InputPressed(object sender, InputData data)
        {
            switch (data.Input)
            {
                case GameInputs.Jump:

                    if (VertState == VertState.Ground)
                    {
                        Vel.Y -= _jumpVelStart;
                        _jumpVelLeft = _jumpVelMax - _jumpVelStart;
                        VertState = VertState.Air;
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

                    if (VertState == VertState.Air)
                    {
                        if (_jumpVelLeft > 0.0f)
                        {
                            Vel.Y -= 1.0f;
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
