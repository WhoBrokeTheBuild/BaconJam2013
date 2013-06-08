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
    public class Test
    {

        public Test()
        {
            Core.UpdateEvent += Update;
            Core.RenderEvent += Render;

            Input.PressedEvent  += InputPressed;
            Input.ReleasedEvent += InputReleased;
            Input.HeldEvent     += InputHeld;
        }

        public void Update(object sender, UpdateData data)
        {
        }

        public void Render(object sender, RenderData data)
        {
        }

        public void InputPressed(object sender, InputData data)
        {
            Console.WriteLine("Pressed: " + data.Input);
        }

        public void InputReleased(object sender, InputData data)
        {
            Console.WriteLine("Released: " + data.Input);
        }

        public void InputHeld(object sender, InputData data)
        {
        }
    }
}
