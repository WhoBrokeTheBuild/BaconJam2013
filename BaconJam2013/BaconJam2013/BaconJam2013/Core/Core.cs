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

    public class UpdateData
        : EventArgs
    {

        private GameTime
            _gameTime;

        private float
            _frameDelta;

        public GameTime GameTime
        {
            get { return _gameTime; }
        }

        public float FrameDelta
        {
            get { return _frameDelta; }
        }

        public UpdateData(GameTime gameTime, float frameDelta)
        {
            _gameTime = gameTime;
            _frameDelta = frameDelta;
        }

    }

    public class RenderData
        : EventArgs
    {

        private SpriteBatch
            _spriteBatch;

        public SpriteBatch SpriteBatch
        {
            get { return _spriteBatch; }
        }

        public RenderData(SpriteBatch spriteBatch)
        {
            _spriteBatch = spriteBatch;
        }

    }

    public delegate void UpdateEventHandler(object sender, UpdateData data);
    public delegate void RenderEventHandler(object sender, RenderData data);

    public enum Side
    {
        None = -1,
        Top,
        Bottom,
        Left,
        Right
    }

    public enum Direction
    {
        None = -1,
        North,
        South,
        East,
        West
    }

    public class Core 
        : Microsoft.Xna.Framework.Game
    {

        public static event UpdateEventHandler UpdateEvent;
        public static event RenderEventHandler RenderEvent;

        private GraphicsDeviceManager 
            _graphics;

        private SpriteBatch 
            _spriteBatch;

        private Input
            _input;

        private Config
            _config;

        private Assets
            _assets;

        private Viewport
            _viewport;

        private Shaders
            _shaders;

        private HUD
            _hud;

        private Texture2D
            _worldTexture;

        private RenderTargetBinding[]
            _renderTargetBinding;

        private RenderTarget2D
            _worldRenderTarget;

        public const int
            Width = 640,
            Height = 480,
            TargetFPS = 60;

        public const string
            Title = "Phototropy";

        public float
            _currentFPS;

        public Core()
        {
            _graphics = new GraphicsDeviceManager(this);

            _graphics.PreferredBackBufferWidth = Width;
            _graphics.PreferredBackBufferHeight = Height;

            Window.Title = Title;

            IsFixedTimeStep = true;
            TargetElapsedTime = new TimeSpan(TimeSpan.TicksPerSecond / TargetFPS);

            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        TestRoom test;

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _currentFPS = 0.0f;

            _input = new Input();
            _config = new Config();
            _assets = new Assets();
            _viewport = new Viewport();
            _shaders = new Shaders(Content, GraphicsDevice);

            _shaders.AddShader("fade");
            _shaders.AddShader("lightsOff");
            _shaders.AddShader("lightsOn");

            Shaders.FadeAmount = 0.0f;

            _worldRenderTarget = new RenderTarget2D(GraphicsDevice, Width, Height);

            _assets.LoadAssets(Content);

            _hud = new HUD();

            test = new TestRoom();
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            float currentFPS = 1 / (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (UpdateEvent != null)
                UpdateEvent(this, new UpdateData(gameTime, TargetFPS / currentFPS));

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _renderTargetBinding = GraphicsDevice.GetRenderTargets();
            GraphicsDevice.SetRenderTarget(_worldRenderTarget);

            GraphicsDevice.Clear(new Color(00, 170, 170));

            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            if (RenderEvent != null)
                RenderEvent(this, new RenderData(_spriteBatch));

            _spriteBatch.End();

            _worldTexture = _worldRenderTarget;
            _worldTexture = _shaders.Draw(_spriteBatch, _worldTexture);

            GraphicsDevice.SetRenderTargets(_renderTargetBinding);

            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            _spriteBatch.Draw(_worldTexture, Vector2.Zero, Color.White);
            _hud.Draw(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
