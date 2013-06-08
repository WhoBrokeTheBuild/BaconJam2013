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

using System.IO;

namespace BaconJam2013
{

    public class ConfigLevel<K, T>
    {

        private Dictionary<K, T>
            mData;

        private Dictionary<K, ConfigLevel<K, T>>
            mSubLevels;

        private Dictionary<K, List<T>>
            mLists;

        public ConfigLevel()
        {

            mData = new Dictionary<K, T>();
            mSubLevels = new Dictionary<K, ConfigLevel<K, T>>();
            mLists = new Dictionary<K, List<T>>();

        }

        public void Clear()
        {
            mData.Clear();
            mSubLevels.Clear();
            mLists.Clear();
        }

        public void Add(K pKey, T pData)
        {
            if (mData.ContainsKey(pKey))
                mData[pKey] = pData;
            else
                mData.Add(pKey, pData);
        }

        public void AddLevel(K pKey)
        {
            mSubLevels.Add(pKey, new ConfigLevel<K, T>());
        }

        public void AddList(K pKey)
        {
            mLists.Add(pKey, new List<T>());
        }

        public void AddToList(K pKey, T pData)
        {
            if (!ContainsList(pKey))
                AddList(pKey);
            mLists[pKey].Add(pData);
        }

        public bool ContainsKey(K pKey)
        {
            return mData.ContainsKey(pKey);
        }

        public bool ContainsLevel(K pKey)
        {
            return mSubLevels.ContainsKey(pKey);
        }

        public bool ContainsList(K pKey)
        {
            return mLists.ContainsKey(pKey);
        }

        public T Get(K pKey)
        {
            return mData[pKey];
        }

        public ConfigLevel<K, T> GetLevel(K pKey)
        {
            return mSubLevels[pKey];
        }

        public K[] GetSubLevelKeys()
        {
            return mSubLevels.Keys.ToArray<K>();
        }

        public List<T> GetList(K pKey)
        {
            return mLists[pKey];
        }

    }

    public class Config
    {

        private static ConfigLevel<string, string>
            mConfig;

        private static char[]
            space = { ' ' },
            slash = { '/' },
            colon = { ':' };

        public Config()
        {

            mConfig = new ConfigLevel<string, string>();
            LoadContent();

        }

        public static void LoadContent()
        {

            mConfig.Clear();
            ReadConfigFile("Game");

        }

        public static void ReadConfigFile(string file)
        {

            using (StreamReader sr = new StreamReader("Content/Config/" + file + ".cfg"))
            {

                string line;
                Stack<string> level = new Stack<string>();
                string levelString = "";
                string[] levelArray;
                string listName = "";

                while ((line = sr.ReadLine()) != null)
                {

                    if (line == "#" || line == "")
                        continue;

                    if (line[0] == '!')
                    {

                        string[] parts = Util.Split(line, space, 2);

                        if (parts.Length > 1)
                        {
                            switch (parts[0])
                            {
                                case "!Load":

                                    ReadConfigFile(parts[1]);

                                    break;
                                case "!Start":

                                    level.Push(parts[1]);

                                    break;
                                case "!End":

                                    level.Pop();

                                    break;
                                case "!StartList":

                                    listName = parts[1];

                                    break;
                                case "!EndList":

                                    listName = "";

                                    break;
                            }

                            levelArray = level.ToArray();
                            levelString = "";
                            for (int i = levelArray.Length - 1; i >= 0; --i)
                                levelString += levelArray[i] + "/";

                            if (listName != "")
                                levelString += listName + ": ";

                        }

                    }
                    else
                        ParseLine(levelString + line);

                }

                sr.Close();

            }

        }

        public static void ParseLine(string line)
        {

            string[] parts = Util.Split(line, space, 2);

            if (parts.Length < 2)
                return;

            string[] levels = parts[0].Split(slash);

            ConfigLevel<string, string> tmp = mConfig;

            for (int i = 0; i < levels.Length - 1; ++i)
            {

                if (levels[i] == "")
                    break;

                if (!tmp.ContainsLevel(levels[i]))
                    tmp.AddLevel(levels[i]);

                tmp = tmp.GetLevel(levels[i]);

            }

            if (parts[0].Contains(':'))
            {
                tmp.AddToList(levels[levels.Length - 1].Substring(0, levels[levels.Length - 1].Length - 1), parts[1]);
            }
            else
            {
                tmp.Add(levels[levels.Length - 1], parts[1]);
            }

        }

        public static string[] GetSubLevels(params string[] name)
        {

            ConfigLevel<string, string> tmp = mConfig;

            bool found = true;

            for (int i = 0; i < name.Length; ++i)
            {

                if (name[i] == "")
                    break;

                if (tmp.ContainsLevel(name[i]))
                    tmp = tmp.GetLevel(name[i]);
                else
                {
                    found = false;
                    break;
                }

            }

            if (found)
                return tmp.GetSubLevelKeys();
            else
                return new string[0];

        }

        private static string GetData(params string[] name)
        {

            ConfigLevel<string, string> tmp = mConfig;

            bool found = true;

            for (int i = 0; i < name.Length - 1; ++i)
            {

                if (name[i] == "")
                    break;

                if (tmp.ContainsLevel(name[i]))
                    tmp = tmp.GetLevel(name[i]);
                else
                {
                    found = false;
                    break;
                }

            }

            if (found)
                return tmp.Get(name[name.Length - 1]);
            else
                return "";

        }

        public static string[] GetTextList(params string[] name)
        {
            return GetList(name);
        }

        public static string[] GetList(params string[] name)
        {

            ConfigLevel<string, string> tmp = mConfig;

            bool found = true;

            for (int i = 0; i < name.Length - 1; ++i)
            {

                if (name[i] == "")
                    break;

                if (tmp.ContainsLevel(name[i]))
                    tmp = tmp.GetLevel(name[i]);
                else
                {
                    found = false;
                    break;
                }

            }

            if (found)
            {
                if (tmp.ContainsList(name[name.Length - 1]))
                    return tmp.GetList(name[name.Length - 1]).ToArray();
                else
                    return new string[0];
            }
            else
                return new string[0];

        }

        public static float[] GetFloatList(params string[] name)
        {

            string[] lines = GetList(name);

            List<float> output = new List<float>();

            foreach (string line in lines)
            {

                float tmp;

                if (Util.TryParseFloat(line, out tmp))
                    output.Add(tmp);

            }

            return output.ToArray();

        }

        public static int[] GetIntList(params string[] name)
        {

            string[] lines = GetList(name);

            List<int> output = new List<int>();

            foreach (string line in lines)
            {

                int tmp;

                if (Util.TryParseInt(line, out tmp))
                    output.Add(tmp);

            }

            return output.ToArray();

        }

        public static Vector2[] GetVector2List(params string[] name)
        {

            string[] lines = GetList(name);

            List<Vector2> output = new List<Vector2>();

            foreach (string line in lines)
            {

                Vector2 tmp;
                string[] parts = line.Split(space);

                if (parts.Length >= 2)
                {
                    if (Util.TryParseVector2(parts[0], parts[1], out tmp))
                        output.Add(tmp);
                }

            }

            return output.ToArray();

        }

        public static Vector3[] GetVector3List(params string[] name)
        {

            string[] lines = GetList(name);

            List<Vector3> output = new List<Vector3>();

            foreach (string line in lines)
            {

                Vector3 tmp;
                string[] parts = line.Split(space);

                if (parts.Length >= 3)
                {
                    if (Util.TryParseVector3(parts[0], parts[1], parts[2], out tmp))
                        output.Add(tmp);
                }

            }

            return output.ToArray();

        }

        public static Vector4[] GetVector4List(params string[] name)
        {

            string[] lines = GetList(name);

            List<Vector4> output = new List<Vector4>();

            foreach (string line in lines)
            {

                Vector4 tmp;
                string[] parts = line.Split(space);

                if (parts.Length >= 2)
                {
                    if (Util.TryParseVector4(parts[0], parts[1], parts[2], parts[3], out tmp))
                        output.Add(tmp);
                }

            }

            return output.ToArray();

        }

        public static Color[] GetColorList(params string[] name)
        {

            string[] lines = GetList(name);

            List<Color> output = new List<Color>();

            foreach (string line in lines)
            {

                Color tmp;
                string[] parts = line.Split(space);

                if (parts.Length >= 2)
                {
                    if (Util.TryParseColor(parts[0], parts[1], parts[2], parts[3], out tmp))
                        output.Add(tmp);
                }

            }

            return output.ToArray();

        }

        public static Rectangle[] GetRectangleList(params string[] name)
        {

            string[] lines = GetList(name);

            List<Rectangle> output = new List<Rectangle>();

            foreach (string line in lines)
            {

                Rectangle tmp;
                string[] parts = line.Split(space);

                if (parts.Length >= 2)
                {
                    if (Util.TryParseRectangle(parts[0], parts[1], parts[2], parts[3], out tmp))
                        output.Add(tmp);
                }

            }

            return output.ToArray();

        }

        public static string GetText(params string[] name)
        {

            return GetData(name);

        }

        public static int GetInt(params string[] name)
        {

            int def = -1;

            ConfigLevel<string, string> tmp = mConfig;

            bool found = true;

            for (int i = 0; i < name.Length - 1; ++i)
            {

                if (name[i] == "")
                    break;

                if (tmp.ContainsLevel(name[i]))
                    tmp = tmp.GetLevel(name[i]);
                else
                {
                    found = false;
                    break;
                }

            }

            if (found)
            {
                if (tmp.ContainsKey(name[name.Length - 1]))
                {

                    int output;
                    string str = tmp.Get(name[name.Length - 1]);

                    if (Util.TryParseInt(str, out output))
                        return output;

                }
            }

            return def;

        }
        public static float GetFloat(params string[] name)
        {

            float def = 0.0f;

            ConfigLevel<string, string> tmp = mConfig;

            bool found = true;

            for (int i = 0; i < name.Length - 1; ++i)
            {

                if (name[i] == "")
                    break;

                if (tmp.ContainsLevel(name[i]))
                    tmp = tmp.GetLevel(name[i]);
                else
                {
                    found = false;
                    break;
                }

            }

            if (found)
            {
                if (tmp.ContainsKey(name[name.Length - 1]))
                {

                    float output;
                    string str = tmp.Get(name[name.Length - 1]);

                    if (Util.TryParseFloat(str, out output))
                        return output;

                }
            }

            return def;

        }

        public static Vector2 GetVector2(params string[] name)
        {

            Vector2 def = Vector2.Zero;

            ConfigLevel<string, string> tmp = mConfig;

            bool found = true;

            for (int i = 0; i < name.Length - 1; ++i)
            {

                if (name[i] == "")
                    break;

                if (tmp.ContainsLevel(name[i]))
                    tmp = tmp.GetLevel(name[i]);
                else
                {
                    found = false;
                    break;
                }

            }

            if (found)
            {
                if (tmp.ContainsKey(name[name.Length - 1]))
                {

                    Vector2 output;
                    string[] parts = tmp.Get(name[name.Length - 1]).Split(space);

                    if (parts.Length >= 2)
                        if (Util.TryParseVector2(parts[0], parts[1], out output))
                            return output;

                }
            }

            return def;

        }

        public static Vector3 GetVector3(params string[] name)
        {

            Vector3 def = Vector3.Zero;

            ConfigLevel<string, string> tmp = mConfig;

            bool found = true;

            for (int i = 0; i < name.Length - 1; ++i)
            {

                if (name[i] == "")
                    break;

                if (tmp.ContainsLevel(name[i]))
                    tmp = tmp.GetLevel(name[i]);
                else
                {
                    found = false;
                    break;
                }

            }

            if (found)
            {
                if (tmp.ContainsKey(name[name.Length - 1]))
                {

                    Vector3 output;
                    string[] parts = tmp.Get(name[name.Length - 1]).Split(space);

                    if (parts.Length >= 3)
                        if (Util.TryParseVector3(parts[0], parts[1], parts[2], out output))
                            return output;

                }
            }

            return def;

        }

        public static Vector4 GetVector4(params string[] name)
        {

            Vector4 def = Vector4.Zero;

            ConfigLevel<string, string> tmp = mConfig;

            bool found = true;

            for (int i = 0; i < name.Length - 1; ++i)
            {

                if (name[i] == "")
                    break;

                if (tmp.ContainsLevel(name[i]))
                    tmp = tmp.GetLevel(name[i]);
                else
                {
                    found = false;
                    break;
                }

            }

            if (found)
            {
                if (tmp.ContainsKey(name[name.Length - 1]))
                {

                    Vector4 output;
                    string[] parts = tmp.Get(name[name.Length - 1]).Split(space);

                    if (parts.Length >= 4)
                        if (Util.TryParseVector4(parts[0], parts[1], parts[2], parts[3], out output))
                            return output;

                }
            }

            return def;

        }

        public static Color GetColor(params string[] name)
        {

            Color def = Color.White;

            ConfigLevel<string, string> tmp = mConfig;

            bool found = true;

            for (int i = 0; i < name.Length - 1; ++i)
            {

                if (name[i] == "")
                    break;

                if (tmp.ContainsLevel(name[i]))
                    tmp = tmp.GetLevel(name[i]);
                else
                {
                    found = false;
                    break;
                }

            }

            if (found)
            {
                if (tmp.ContainsKey(name[name.Length - 1]))
                {

                    Color output;
                    string[] parts = tmp.Get(name[name.Length - 1]).Split(space);

                    if (parts.Length == 3)
                        if (Util.TryParseColor(parts[0], parts[1], parts[2], "255", out output))
                            return output;
                    else if (parts.Length >= 4)
                        if (Util.TryParseColor(parts[0], parts[1], parts[2], parts[3], out output))
                            return output;

                }
            }

            return def;

        }

    }

}
