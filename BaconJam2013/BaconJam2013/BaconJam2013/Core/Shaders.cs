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

    class Shaders
    {

        private RenderTarget2D[]
            _renderTargets;

        public static Dictionary<string, Effect>
            _effects;

        public static Dictionary<string, bool>
            _effectsEnabled;

        private ContentManager
            _content;

        private GraphicsDevice
            _graphicsDevice;

        public static float
            FadeAmount = 0.0f;

        private float
            _glowDistance,
            _minGlowDistance,
            _maxGlowDistance;

        public List<Vector2>
            GlowPoints,
            ShadePoints;

        public static bool
            GlobalLight = false;

        public Shaders(ContentManager pContent, GraphicsDevice pGraphicsDevice)
        {

            _content = pContent;
            _graphicsDevice = pGraphicsDevice;

            _effects = new Dictionary<string, Effect>();
            _effectsEnabled = new Dictionary<string, bool>();

            Core.UpdateEvent += Update;

            //Prepare Render Targets
            _renderTargets = new RenderTarget2D[2];
            _renderTargets[0] = new RenderTarget2D(_graphicsDevice, Core.Width, Core.Height);
            _renderTargets[1] = new RenderTarget2D(_graphicsDevice, Core.Width, Core.Height);

            GlowPoints = new List<Vector2>();
            ShadePoints = new List<Vector2>();

            GlowPoints.Add(new Vector2(100));
            GlowPoints.Add(new Vector2(300));

            ShadePoints.Add(new Vector2(100, 300));
            ShadePoints.Add(new Vector2(500));

            UpdateConfig();

            Input.PressedEvent += InputPressed;
        }

        public virtual void InputPressed(object sender, InputData data)
        {
            switch (data.Input)
            {
                case GameInputs.Debug1:

                    GlobalLight = !GlobalLight;

                    break;
                case GameInputs.Debug2:

                    break;
                case GameInputs.Debug3:

                    break;
            }
        }

        public void UpdateConfig()
        {
            _minGlowDistance = 5.0f;
            _maxGlowDistance = 15.0f;
            _glowDistance = _minGlowDistance;
        }

        public void AddShader(string name)
        {

            _effects.Add(name, _content.Load<Effect>("Shaders/" + name));
            _effectsEnabled.Add(name, true);

        }

        public static RenderTarget2D CloneRenderTarget(GraphicsDevice device, int numberLevels)
        {
            return new RenderTarget2D(device,
                device.PresentationParameters.BackBufferWidth,
                device.PresentationParameters.BackBufferHeight);
        }

        public void Update(object sender, UpdateData data)
        {

            _effects["fade"].Parameters["fadeAmount"].SetValue(FadeAmount);

            float amount = (float)Math.Sin(data.GameTime.TotalGameTime.TotalMilliseconds / 400);
            _glowDistance = MathHelper.Lerp(_minGlowDistance, _maxGlowDistance, Math.Abs(amount));

        }

        public int RenderShader(Effect pEffect, SpriteBatch pSpriteBatch, Texture2D pShaderTexture, int renderTargetInd, out Texture2D oShaderTexture)
        {

            _graphicsDevice.SetRenderTarget(_renderTargets[renderTargetInd]);

            pSpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            pEffect.CurrentTechnique.Passes[0].Apply();

            pSpriteBatch.Draw(pShaderTexture, Vector2.Zero, Color.White);

            pSpriteBatch.End();

            oShaderTexture = _renderTargets[renderTargetInd];

            return 1 - renderTargetInd;

        }

        public Texture2D Draw(SpriteBatch pSpriteBatch, Texture2D pShaderTexture)
        {

            int renderTargetInd = 0;

            foreach (KeyValuePair<string, Effect> item in _effects)
            {

                if (!_effectsEnabled[item.Key] || item.Key == "lightsOn" || item.Key == "lightsOff")
                    continue;

                renderTargetInd = RenderShader(item.Value, pSpriteBatch, pShaderTexture, renderTargetInd, out pShaderTexture);

            }

            if (GlobalLight && _effectsEnabled.ContainsKey("lightsOn") && _effectsEnabled["lightsOn"])
            {
                foreach (Vector2 shadePos in ShadePoints)
                {
                    Vector2 newPos = shadePos - Viewport.Pos;

                    float[] pos = 
                        {
                            newPos.X / Core.Width,
                            newPos.Y / Core.Height
                        };

                    _effects["lightsOn"].Parameters["center"].SetValue(pos);
                    renderTargetInd = RenderShader(_effects["lightsOn"], pSpriteBatch, pShaderTexture, renderTargetInd, out pShaderTexture);
                }
            }
            else if (!GlobalLight && _effectsEnabled.ContainsKey("lightsOff") && _effectsEnabled["lightsOff"])
            {
                foreach (Vector2 glowPos in GlowPoints)
                {
                    Vector2 newPos = glowPos - Viewport.Pos;

                    float[] pos = 
                        {
                            newPos.X / Core.Width,
                            newPos.Y / Core.Height
                        };

                    _effects["lightsOff"].Parameters["center"].SetValue(pos);
                    renderTargetInd = RenderShader(_effects["lightsOff"], pSpriteBatch, pShaderTexture, renderTargetInd, out pShaderTexture);
                }
            }

            return pShaderTexture;

        }

    }

}
