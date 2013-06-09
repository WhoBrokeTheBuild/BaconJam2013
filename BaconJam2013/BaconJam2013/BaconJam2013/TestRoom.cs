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
    class TestRoom
    {

        private class Range
        {
            public int
                Min,
                Max;

            public Range(int min, int max)
            {
                Min = min;
                Max = max;
            }

            public bool Contains(int number)
            {
                return (number >= Min && number <= Max);
            }
        }

        private Player
            _player;

        private List<Platform>
            _platforms;

        private List<BasicUnit>
            _tiles;

        private int[,] 
            _visibleLayer,
            _objectLayer;

        private const int
            TilePlatform = 3,
            TileEastRamp = 2,
            TileWestRamp = 1,
            TilePlayerSpawn = 68;

        private Dictionary<string, Range>
            Tilesets;

        public TestRoom()
        {
            _player = new Player(new Vector2(320, 0));
            Viewport.Follow(_player);

            Tilesets = new Dictionary<string, Range>();

            Tilesets.Add("platform-tiles", new Range(1, 64));
            Tilesets.Add("special-tiles", new Range(65, 128));
            Tilesets.Add("indicator-tiles", new Range(129, 192));
            Tilesets.Add("lights-tiles", new Range(193, 256));
            Tilesets.Add("switch-tiles", new Range(257, 320));
            Tilesets.Add("plants-tiles", new Range(321, 384));
            Tilesets.Add("ground-tiles", new Range(385, 500));

            int width = Config.GetInt("Maps", "test", "Width");
            int height = Config.GetInt("Maps", "test", "Height");
            int tileSize = 32;

            _player.Pos = Config.GetVector2("Maps", "test", "Player") * tileSize;

            int[] objectData = Config.GetIntList("Maps", "test", "objects");
            int[] tileData = Config.GetIntList("Maps", "test", "visual-layers", "visual_01");

            _objectLayer = new int[width, height];
            _visibleLayer = new int[width, height];

            int row = 0,
                col = 0;
            for (int i = 0; i < objectData.Length; ++i)
            {
                _objectLayer[col, row] = objectData[i];
                _visibleLayer[col, row] = tileData[i];

                ++col;
                if (col == width)
                {
                    col = 0;
                    ++row;
                }
            }

            _platforms = new List<Platform>();
            _tiles = new List<BasicUnit>();

            for (row = 0; row < height; ++row)
            {
                for (col = 0; col < width; ++col)
                {
                    if (_objectLayer[col, row] != 0)
                        Console.WriteLine(_objectLayer[col, row]);

                    Vector2 pos = new Vector2(col * tileSize, row * tileSize);

                    if (_objectLayer[col, row] == TilePlatform)
                    {
                        _platforms.Add(new Platform(pos, new Vector2(tileSize), false));
                    }
                    else if (_objectLayer[col, row] == TileEastRamp)
                    {
                        _platforms.Add(new Platform(pos, new Vector2(tileSize), Direction.East));
                    }
                    else if (_objectLayer[col, row] == TileWestRamp)
                    {
                        _platforms.Add(new Platform(pos, new Vector2(tileSize), Direction.West));
                    }
                    else if (_objectLayer[col, row] == TilePlayerSpawn)
                    {
                        _player.Pos = pos;
                    }

                    if (_visibleLayer[col, row] != 0)
                    {
                        foreach (KeyValuePair<string, Range> pair in Tilesets)
                        {
                            if (!pair.Value.Contains(_visibleLayer[col, row]))
                                continue;

                            int sheetInd = _visibleLayer[col, row] - pair.Value.Min;

                            Texture2D texture = Assets.Animations[pair.Key].Frame(0).Texture;

                            Vector2 coord = Vector2.Zero;
                            for (int i = 0; i < sheetInd; ++i)
                            {
                                coord.X += tileSize;
                                if (coord.X >= texture.Width)
                                {
                                    coord.X = 0;
                                    coord.Y += tileSize;
                                }
                            }

                            Sprite sprite = new Sprite(texture, new Rectangle((int)coord.X, (int)coord.Y, tileSize, tileSize));
                            List<Sprite> frames = new List<Sprite>();
                            frames.Add(sprite);

                            Animation newAnim = new Animation(frames, new Vector2(tileSize), 0, Vector2.Zero, false);

                            _tiles.Add(new BasicUnit(newAnim, pos, Color.White));
                        }
                    }

                }
            }

            Core.UpdateEvent += Update;
            Core.RenderEvent += Render;
        }

        public void Update(object sender, UpdateData data)
        {
            Vector2 startPos = _player.Pos;
            bool 
                onSomething = false,
                onRamp = false;

            for (int i = 0; i < _platforms.Count; ++i)
            {
                Platform plat = _platforms[i];

                Rectangle
                    playerBounds = _player.Bounds(),
                    platBounds = plat.Bounds();

                playerBounds.Height += 1;

                if (!onSomething)
                {
                    if (!plat.Ramp && platBounds.Intersects(new Rectangle(playerBounds.X, playerBounds.Bottom, playerBounds.Width, 2)))
                        onSomething = true;
                }

                if (!playerBounds.Intersects(platBounds))
                    continue;

                if (plat.Ramp)
                {
                    if (_player.Pos.X > plat.Pos.X && _player.Pos.X < plat.Pos.X + plat.Size.X)
                    {
                        float rampAmount = (_player.Pos.X - plat.Pos.X) / plat.Size.X;
                        float newY = 0.0f;

                        if (rampAmount > 0.9f)
                            rampAmount = 1.0f;
                        else if (rampAmount < 0.1f)
                            rampAmount = 0.0f;

                        if (plat.RampDir == Direction.West)
                        {
                            newY = plat.Pos.Y + plat.Size.Y - MathHelper.Lerp(plat.Size.Y, 0.0f, rampAmount);
                        }
                        else if (plat.RampDir == Direction.East)
                        {
                            newY = plat.Pos.Y + plat.Size.Y - MathHelper.Lerp(0.0f, plat.Size.Y, rampAmount);
                        }

                        if (_player.VertState == VertState.Air)
                        {
                            if (_player.Pos.Y >= newY)
                            {
                                _player.SetState(State.Idle, VertState.Ground);
                            }
                        }

                        if (_player.VertState == VertState.Ground)
                        {
                            _player.Pos.Y = newY;
                            onSomething = true;
                            onRamp = true;
                        }
                    }
                }
            }

            if (!onRamp)
            {

                for (int i = 0; i < _platforms.Count; ++i)
                {
                    Platform plat = _platforms[i];

                    Rectangle
                        playerBounds = _player.Bounds(),
                        platBounds = plat.Bounds();

                    Side hitSide = Util.Collide(playerBounds, platBounds, _player.Vel);

                    if (plat.JumpThrough && hitSide != Side.Bottom)
                    {
                        continue;
                    }

                    if (plat.Ramp)
                    {
                        continue;
                    }

                    switch (hitSide)
                    {
                        case Side.Top:

                            _player.Pos.Y = platBounds.Bottom + _player.Origin.Y;
                            _player.Vel.Y = 0.0f;

                            break;
                        case Side.Bottom:

                            _player.Pos.Y = platBounds.Top - playerBounds.Height + _player.Origin.Y;
                            _player.Vel.Y = 0.0f;
                            _player.SetState(State.Idle, VertState.Ground);

                            break;
                        case Side.Left:

                            _player.Pos.X = platBounds.Left - playerBounds.Width + _player.Origin.X - 1;
                            _player.Vel.X = 0.0f;

                            break;
                        case Side.Right:

                            _player.Pos.X = platBounds.Right + _player.Origin.X + 1;
                            _player.Vel.X = 0.0f;

                            break;
                    }
                }

            }

            if (_player.VertState == VertState.Ground && !onSomething)
            {
                _player.SetState(State.Jump, VertState.Air);
            }
        }

        public void Render(object sender, RenderData data)
        {
        }

    }
}
