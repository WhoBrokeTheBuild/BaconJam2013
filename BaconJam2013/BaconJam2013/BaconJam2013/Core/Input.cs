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

    public class InputData
        : EventArgs
    {
        
        private GameInputs
            _input;

        public GameInputs Input
        {
            get { return _input; }
        }

        public InputData(GameInputs input)
        {
            _input = input;
        }

    }

    public delegate void InputEventHandler(object sender, InputData data);

    public enum GameInputs
    {
        None = -1,
        Up,
        Down,
        Left,
        Right,
        Jump,
        Fire
    }

    public enum InputType
    {
        None = -1,
        Key,
        MouseButton,
        GamePadButton
    }

    public class MultiInput
    {

        private InputType
            _type;

        private Keys
            _key;

        private MouseButtons
            _mouseButton;

        private Buttons
            _gamePadButton;

        public InputType Type
        {
            get { return _type; }
        }

        public Keys Key
        {
            get { return _key; }
        }

        public MouseButtons MouseButton
        {
            get { return _mouseButton; }
        }

        public Buttons GamePadButton
        {
            get { return _gamePadButton; }
        }

        public MultiInput(Keys key)
        {
            _key = key;

            _type = InputType.Key;
        }

        public MultiInput(MouseButtons mouseButton)
        {
            _mouseButton = mouseButton;

            _type = InputType.MouseButton;
        }

        public MultiInput(Buttons gamePadButton)
        {
            _gamePadButton = gamePadButton;

            _type = InputType.GamePadButton;
        }

    }

    public class MultiState
    {

        private KeyboardState
            _keyboardState;

        private MouseButtonState
            _mouseButtonState;

        private GamePadState
            _gamePadState;

        public MultiState()
        {
            _keyboardState = Keyboard.GetState();
            _mouseButtonState = new MouseButtonState(Mouse.GetState());
            _gamePadState = GamePad.GetState(PlayerIndex.One);
        }

        public MultiState(KeyboardState keyboardState, MouseButtonState mouseButtonState, GamePadState gamePadState)
        {
            _keyboardState = keyboardState;
            _mouseButtonState = mouseButtonState;
            _gamePadState = gamePadState;
        }

        public bool IsDown(MultiInput input)
        {
            switch (input.Type)
            {
                case InputType.Key:

                    return _keyboardState.IsKeyDown(input.Key);

                case InputType.MouseButton:

                    return _mouseButtonState.IsButtonDown(input.MouseButton);

                case InputType.GamePadButton:

                    return _gamePadState.IsButtonDown(input.GamePadButton);

            }

            return false;
        }

        public bool IsUp(MultiInput input)
        {
            switch (input.Type)
            {
                case InputType.Key:

                    return _keyboardState.IsKeyUp(input.Key);

                case InputType.MouseButton:

                    return _mouseButtonState.IsButtonUp(input.MouseButton);

                case InputType.GamePadButton:

                    return _gamePadState.IsButtonUp(input.GamePadButton);

            }

            return false;
        }

    }

    public enum MouseButtons
    {
        None = -1,
        Left,
        Middle,
        Right
    }

    public struct MouseButtonState
    {
        private Dictionary<MouseButtons, ButtonState>
            _states;

        public MouseButtonState(MouseState mouseState)
        {
            _states = new Dictionary<MouseButtons, ButtonState>();

            _states.Add(MouseButtons.Left,   mouseState.LeftButton);
            _states.Add(MouseButtons.Middle, mouseState.MiddleButton);
            _states.Add(MouseButtons.Right,  mouseState.RightButton);
        }

        public bool IsButtonDown(MouseButtons button)
        {
            if (button == MouseButtons.None)
                return false;

            return (_states[button] == ButtonState.Pressed);
        }

        public bool IsButtonUp(MouseButtons button)
        {
            if (button == MouseButtons.None)
                return false;

            return (_states[button] == ButtonState.Released);
        }
    }

    public class Input
    {

        public static event InputEventHandler PressedEvent;
        public static event InputEventHandler ReleasedEvent;
        public static event InputEventHandler HeldEvent;

        private MultiState
            _inputState;

        private Dictionary<GameInputs, MultiInput>
            _inputMap;

        public Input()
        {
            _inputState = new MultiState();

            Core.UpdateEvent += new UpdateEventHandler(Update);

            _inputMap = new Dictionary<GameInputs, MultiInput>();

            _inputMap.Add(GameInputs.Up, new MultiInput(Keys.W));
            _inputMap.Add(GameInputs.Left, new MultiInput(Keys.A));
            _inputMap.Add(GameInputs.Down, new MultiInput(Keys.S));
            _inputMap.Add(GameInputs.Right, new MultiInput(Keys.D));

            _inputMap.Add(GameInputs.Jump, new MultiInput(Keys.Space));

            _inputMap.Add(GameInputs.Fire, new MultiInput(MouseButtons.Left));
        }

        public void Update(object sender, UpdateData data)
        {
            MultiState
                newState = new MultiState(),
                oldState = _inputState;

            List<GameInputs> gameInputs = EnumUtil.GetValues<GameInputs>();

            for (int i = 0; i < gameInputs.Count; ++i)
            {
                GameInputs 
                    gameInput = gameInputs[i];

                if (!_inputMap.Keys.Contains(gameInput))
                    continue;

                MultiInput input = _inputMap[gameInput];

                bool
                    isDown = newState.IsDown(input),
                    wasDown = oldState.IsDown(input);

                if (isDown && !wasDown)
                {
                    if (PressedEvent != null)
                        PressedEvent(this, new InputData(gameInput));
                }
                else if (!isDown && wasDown)
                {
                    if (ReleasedEvent != null)
                        ReleasedEvent(this, new InputData(gameInput));
                }

                if (isDown)
                {
                    if (HeldEvent != null)
                        HeldEvent(this, new InputData(gameInput));
                }
            }

            _inputState = newState;
        }

    }
}
