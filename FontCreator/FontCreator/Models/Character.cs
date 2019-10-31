using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FontCreator.Models
{
    public class Character
    {
        public int Width { get; private set; }
        public int Height { get; private set; }

        public int AsciiValue { get; set; }

        byte[] data;

        public Character()
        {
            
        }

        public bool IsPixelSet(int x, int y)
        {
            if(x < 0 || y < 0 || x >= Width || y >= Height)
            {
                return false;
            }

            var byteIndex = (y * Width + x) / 8;
            var bitIndex = (y * Width + x) % 8;

            var value = data[byteIndex];

            return (value & (1 << bitIndex)) != 0;
        }

        public void SetPixel(int x, int y, bool set)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height)
            {
                return;
            }

            var byteIndex = (y * Width + x) / 8;
            var bitIndex = (y * Width + x) % 8;
            var mask = (byte)(1 << bitIndex);

            if (set)
            {
                data[byteIndex] |= mask;
            }
            else
            {
                data[byteIndex] &= (byte)~mask;
            }
        }

        public void Clear()
        {
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = 0;
            }
        }

        public void ParseCharData(int width, int height, string line)
        {
            Width = width;
            Height = height;

            var index = line.IndexOf("0x");

            List<byte> data = new List<byte>();

            while (index != -1)
            {
                var character = line.Substring(index, 4);

                var value = Convert.ToByte(character, 16);

                data.Add(value);
                
                index = line.IndexOf("0x", index + 4);
            }

            this.data = data.ToArray();
        }

        public string GetLineText()
        {
            var line = new StringBuilder();

            line.Append(@"{0x");
            var hex = BitConverter.ToString(data).Replace("-", ", 0x");
            line.Append(hex);
            line.Append(@"}, //");
            line.Append(AsciiValue.ToString("X4"));
            line.Append($"({(char)(AsciiValue)})");

            return line.ToString();
        }
    }
}
