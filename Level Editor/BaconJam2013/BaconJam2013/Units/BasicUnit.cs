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

    public delegate void AnimationCompleteHandler(object sender, EventArgs data);

    public class BasicUnit
    {

        private Animation
            _animation;

        private double
            _animationTimeout;

        private int
            _frame;

        private bool
            _animationComplete;

        public event AnimationCompleteHandler AnimationCompleteEvent;

        public Vector2
            Pos,
            Origin,
            Size;

        public Color
            BlendColor;

        public float
            Rot,
            Depth;

        public bool
            Animating,
            Looping;

        public BasicUnit(Animation animation, Vector2 pos, Color blendColor, float rot = 0.0f, float depth = 1.0f)
        {
            SetAnimation(animation);

            if (blendColor == null)
                blendColor = Color.White;

            Pos = pos;
            Rot = rot;
            BlendColor = blendColor;

            _frame = 0;
            _animationComplete = false;

            Core.UpdateEvent += Update;
            Core.RenderEvent += Render;
            AnimationCompleteEvent += AnimationComplete;
        }

        public virtual void Update(object sender, UpdateData data)
        {
            if (Animating && !(Looping && _animationComplete))
            {
                UpdateAnimation(data);
            }
        }

        public virtual void Render(object sender, RenderData data)
        {
            Sprite currentFrame = _animation.Frame(_frame);

            if (currentFrame == null)
                return;

            data.SpriteBatch.Draw(currentFrame.Texture, Pos, currentFrame.Source, BlendColor, Rot, Origin, Vector2.One, SpriteEffects.None, Depth);
        }

        public virtual void AnimationComplete(object sender, EventArgs data)
        {
            _animationComplete = true;
        }

        public Rectangle Bounds()
        {
            return new Rectangle((int)Pos.X, (int)Pos.Y, (int)Size.X, (int)Size.Y);
        }

        public virtual void Reset()
        {
            _frame = 0;
            _animationTimeout = _animation.FrameTime;
            _animationComplete = false;
        }

        public void SetAnimation(Animation animation, bool useDefaults = true)
        {
            _animation = animation;

            _frame = 0;
            Size = _animation.FrameSize;

            if (useDefaults)
            {
                _animationTimeout = _animation.FrameTime;
                Origin = _animation.Origin;
                Animating = _animation.Animating;
                Looping = _animation.Looping;
                _animationComplete = false;
            }
        }

        private void UpdateAnimation(UpdateData data)
        {
            _animationTimeout -= data.GameTime.ElapsedGameTime.Milliseconds;
            if (_animationTimeout < 0)
            {
                if (!Looping)
                {
                    if (_frame == _animation.Length() - 1)
                    {
                        _frame = (int)MathHelper.Clamp(_frame, 0, _animation.Length() - 1);

                        if (AnimationCompleteEvent != null)
                            AnimationCompleteEvent(this, new EventArgs());

                        return;
                    }
                }
                _frame = (_frame + 1) % _animation.Length();
                _animationTimeout = _animation.FrameTime;
            }
        }

    }
}
