using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace BaconJam2013
{

    public class Assets
    {

        private static Dictionary<string, Texture2D>
            mTextures;

        private static Dictionary<string, Animation>
            mAnimations;

        public static Sprite NoSprite;

        public static Dictionary<string, Animation> Animations
        {
            get
            {
                return mAnimations;
            }
        }

        public Assets()
        {
            mTextures = new Dictionary<string, Texture2D>();
            mAnimations = new Dictionary<string, Animation>();
        }

        public void LoadAssets(ContentManager pContent)
        {

            string[] sprites = Config.GetSubLevels("Assets");

            foreach (string spriteName in sprites)
            {

                string texture = "Sprites/" + Config.GetText("Assets", spriteName, "Texture");
                List<Sprite> frames = new List<Sprite>();

                int
                    count,
                    cols;

                float 
                    frameTime;

                Vector2 
                    size;

                bool 
                    animating = true, 
                    looping;

                if (!mTextures.ContainsKey(texture))
                    mTextures.Add(texture, pContent.Load<Texture2D>(texture));

                Rectangle[] frameOrder = Config.GetRectangleList("Assets", spriteName, "FrameOrder");
                count     = Config.GetInt("Assets", spriteName, "Auto");
                cols      = Config.GetInt("Assets", spriteName, "Cols");
                size      = Config.GetVector2("Assets", spriteName, "Size");
                frameTime = Config.GetFloat("Assets", spriteName, "Speed");
                looping   = (Config.GetText("Assets", spriteName, "Loop") == "T");

                int x = 0, y = 0;
                int col = 0;
                for (int i = 0; i < count; ++i)
                {
                    frames.Add(new Sprite(mTextures[texture], new Rectangle(x, y, (int)size.X, (int)size.Y)));
                    x += (int)size.X;
                    col++;
                    if (col >= cols)
                    {
                        col = 0;
                        y += (int)size.Y;
                    }
                }

                mAnimations.Add(spriteName, new Animation(frames, size, frameTime, animating, looping));

            }

        }

    }

}
