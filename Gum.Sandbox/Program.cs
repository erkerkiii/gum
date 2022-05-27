using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Text.Json.Serialization;
using Gum.WebRequest;
using LZ4;

namespace Gum.Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            unsafe
            {
                LevelPack20 levelPack20 = new LevelPack20();
                fixed (Level* level = &levelPack20[0])
                {
                    for (int index = 0; index < 10; index++)
                    {
                        Console.WriteLine($"{index} ///////////////////////////////////////////////////");
                        using MemoryStream memoryStream = new MemoryStream();
                        binaryFormatter.Serialize(memoryStream, JsonSerializer.Serialize(level[index]));
                        Console.WriteLine(LZ4Codec.Encode(memoryStream.ToArray(), 0, (int)memoryStream.Length).Length);
                    }
                }
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    public struct LevelPack20
    {
        public Level l1;
        public Level l2;
        public Level l3;
        public Level l4;
        public Level l5;
        public Level l6;
        public Level l7;
        public Level l8;
        public Level l9;
        public Level l10;
        
        public ref Level this[int index] {
            get
            {
                unsafe
                {
                    fixed (Level* level = &l1)
                    {
                        return ref level[index];
                    }
                }
            }
        }
    }

    [System.Serializable]
    public struct Level
    {
        public int id;
        public int name;
        public int moves;
        public int is_tutorial;

        public double level_bar;
    }

    [Serializable]
    public struct LevelObject
    {
        public byte category_id;

        public int challenge_id;
        public int count;
        public int status;
        public int object_id;
        public int is_jelly;
        public int is_bottom;
        public int is_chained;

        public float width;
        public float height;
        public float obj_width;

        public float obj_degree;
    }

    [Serializable]
    public struct MissionDTO
    {
        public byte mission_type_id;

        public int mission_type_module_id;
        public int mission_value;
    }
    
    [System.Serializable]
    public struct HelperConfiguration
    {
        public int move;
        public int value;
        public int count;
    }
    
    [System.Serializable]
    public struct HelperDTO
    {
        public int helper_id;

        public HelperConfiguration[] configures;
    }

    [Serializable]
    public struct Matchable
    {
        public int id;

        public bool isJelly;
    }

    [System.Serializable]
    public struct Section
    {
        public LevelObject[] data;
    }
}