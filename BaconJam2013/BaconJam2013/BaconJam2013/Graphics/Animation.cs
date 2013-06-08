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
    public class Animation
    {

        private List<Sprite>
            _frames;

        public bool
            Animating,
            Looping;

        public Vector2
            FrameSize;

        public float
            FrameTime;

        public Animation(List<Sprite> frames, Vector2 size, float frameTime, bool animating = false, bool looping = false)
        {
            _frames = frames;
            FrameSize = size;
            FrameTime = frameTime;
            Animating = animating;
            Looping = looping;
        }

        public Sprite Frame(int frame)
        {
            if (frame < 0 || frame > _frames.Count)
                return null;

            return _frames[frame];
        }

        public int Length()
        {
            return _frames.Count;
        }

    }
}
