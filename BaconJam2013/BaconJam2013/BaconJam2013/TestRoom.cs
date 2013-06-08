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

        private Player
            _player;

        private List<Platform>
            _platforms;

        private int[,] _platformLayer;

        private const int
            TileNormal = 1,
            TileEastRamp = 4,
            TileWestRamp = 3;

        public TestRoom()
        {
            _player = new Player(new Vector2(320, 0));
            Viewport.Follow(_player);

            int width = Config.GetInt("Maps", "test", "Width");
            int height = Config.GetInt("Maps", "test", "Height");
            int tileSize = 32;

            _player.Pos = Config.GetVector2("Maps", "test", "Player") * tileSize;

            int[] platformData = Config.GetIntList("Maps", "test", "platforms");
            _platformLayer = new int[width, height];

            int row = 0,
                col = 0;
            for (int i = 0; i < platformData.Length; ++i)
            {
                _platformLayer[col, row] = platformData[i];

                ++col;
                if (col == width)
                {
                    col = 0;
                    ++row;
                }
            }

            _platforms = new List<Platform>();

            for (row = 0; row < height; ++row)
            {
                for (col = 0; col < width; ++col)
                {
                    if (_platformLayer[col, row] == TileNormal)
                    {
                        _platforms.Add(new Platform(new Vector2(col * tileSize, row * tileSize), new Vector2(tileSize), false));
                    }
                    //else if (_platformLayer[col, row] == )
                    //{
                    //    _platforms.Add(new Platform(new Vector2(col * tileSize, row * tileSize), new Vector2(tileSize), true));
                    //}
                    else if (_platformLayer[col, row] == TileEastRamp)
                    {
                        _platforms.Add(new Platform(new Vector2(col * tileSize, row * tileSize), new Vector2(tileSize), Direction.East));
                    }
                    else if (_platformLayer[col, row] == TileWestRamp)
                    {
                        _platforms.Add(new Platform(new Vector2(col * tileSize, row * tileSize), new Vector2(tileSize), Direction.West));
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
