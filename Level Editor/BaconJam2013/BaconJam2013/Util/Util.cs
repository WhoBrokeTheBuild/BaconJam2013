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

    public class Util
    {

        public static List<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToList<T>();
        }

        public static float Rad(int degree)
        {
            return (float)(degree * (Math.PI / 180));
        }

        public static float Rad(float degree)
        {
            return (float)(degree * (Math.PI / 180));
        }

        public static int Deg(float radians)
        {
            return (int)(radians * (180 / Math.PI));
        }

        public static int Sign(int num)
        {
            return (num < 0 ? -1 : 1);
        }

        public static int Sign(float num)
        {
            return (num < 0 ? -1 : 1);
        }

        public static Vector2 Vector2Lerp(Vector2 start, Vector2 end, float amount)
        {
            return new Vector2(MathHelper.Lerp(start.X, end.X, amount), MathHelper.Lerp(start.Y, end.Y, amount));
        }

        public static Vector2 Vector2SmoothStep(Vector2 start, Vector2 end, float amount)
        {
            return new Vector2(MathHelper.SmoothStep(start.X, end.X, amount), MathHelper.SmoothStep(start.Y, end.Y, amount));
        }

        public static bool TryParseInt(string pStr, out int pValue)
        {

            return int.TryParse(pStr, out pValue);

        }

        public static bool TryParseFloat(string pStr, out float pValue)
        {

            return float.TryParse(pStr, out pValue);

        }

        public static bool TryParseVector2(string pStr1, string pStr2, out Vector2 pValue)
        {

            bool succeeded = true;

            float x, y;

            succeeded &= TryParseFloat(pStr1, out x);
            succeeded &= TryParseFloat(pStr2, out y);

            if (succeeded)
                pValue = new Vector2(x, y);
            else
                pValue = Vector2.Zero;

            return succeeded;

        }

        public static bool TryParseVector3(string pStr1, string pStr2, string pStr3, out Vector3 pValue)
        {

            bool succeeded = true;

            float x, y, z;

            succeeded &= TryParseFloat(pStr1, out x);
            succeeded &= TryParseFloat(pStr2, out y);
            succeeded &= TryParseFloat(pStr3, out z);

            if (succeeded)
                pValue = new Vector3(x, y, z);
            else
                pValue = Vector3.Zero;

            return succeeded;

        }

        public static bool TryParseVector4(string pStr1, string pStr2, string pStr3, string pStr4, out Vector4 pValue)
        {

            bool succeeded = true;

            float w, x, y, z;

            succeeded &= TryParseFloat(pStr1, out w);
            succeeded &= TryParseFloat(pStr2, out x);
            succeeded &= TryParseFloat(pStr3, out y);
            succeeded &= TryParseFloat(pStr4, out z);

            if (succeeded)
                pValue = new Vector4(w, x, y, z);
            else
                pValue = Vector4.Zero;

            return succeeded;

        }

        public static bool TryParseColor(string pStr1, string pStr2, string pStr3, string pStr4, out Color pValue)
        {

            bool succeeded = true;

            int r, g, b, a;

            succeeded &= TryParseInt(pStr1, out r);
            succeeded &= TryParseInt(pStr2, out g);
            succeeded &= TryParseInt(pStr3, out b);
            succeeded &= TryParseInt(pStr4, out a);

            if (succeeded)
                pValue = new Color(r, g, b, a);
            else
                pValue = Color.White;

            return succeeded;

        }

        public static bool TryParseRectangle(string pStr1, string pStr2, string pStr3, string pStr4, out Rectangle pValue)
        {

            bool succeeded = true;

            int x, y, w, h;

            succeeded &= TryParseInt(pStr1, out x);
            succeeded &= TryParseInt(pStr2, out y);
            succeeded &= TryParseInt(pStr3, out w);
            succeeded &= TryParseInt(pStr4, out h);

            if (succeeded)
                pValue = new Rectangle(x, y, w, h);
            else
                pValue = Rectangle.Empty;

            return succeeded;

        }

        public static string[] Split(string str, char[] separator, int len)
        {

            string[] ret, tmp;

            tmp = str.Split(separator);

            ret = new string[len];

            for (int i = 0; i < len; ++i)
                ret[i] = tmp[i];

            if (tmp.Length > len)
            {
                for (int i = len; i < tmp.Length; ++i)
                    ret[len - 1] += separator[0] + tmp[i];
            }

            return ret;

        }

    }

}
