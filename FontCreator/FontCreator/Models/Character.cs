using System;
using System.Collections.Generic;
using System.Text;

namespace FontCreator.Models
{
    public class Character
    {
        public int Width { get; private set; }
        public int Height { get; private set; }

        public int AsciiValue { get; set; }

        public byte[] Data { get; set; }

        public Character()
        {
        }

        public Character(int rows, int columns, int asciiValue)
        {
            Data = new byte[rows * columns / 8];

            Width = columns;
            Height = rows;

            AsciiValue = asciiValue;
        }

        public bool IsPixelSet(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height)
            {
                return false;
            }

            var byteIndex = (y * Width + x) / 8;
            var bitIndex = (y * Width + x) % 8;

            var value = Data[byteIndex];

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
                Data[byteIndex] |= mask;
            }
            else
            {
                Data[byteIndex] &= (byte)~mask;
            }
        }

        public void ShiftLeft()
        {
            var temp = new bool[Height];

            for (int i = 0; i < Width - 1; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    if (i == 0) { temp[j] = IsPixelSet(0, j); }

                    SetPixel(i, j, IsPixelSet(i + 1, j));
                }
            }

            for (int j = 0; j < Height; j++)
            {
                SetPixel(Width - 1, j, temp[j]);
            }
        }

        public void ShiftRight()
        {
            var temp = new bool[Height];

            for (int i = Width - 1; i > 0; i--)
            {
                for (int j = 0; j < Height; j++)
                {
                    if (i == Width - 1) { temp[j] = IsPixelSet(Width - 1, j); }

                    SetPixel(i, j, IsPixelSet(i - 1, j));
                }
            }

            for (int j = 0; j < Height; j++)
            {
                SetPixel(0, j, temp[j]);
            }
        }

        public void ShiftUp()
        {
            var temp = new bool[Width];

            for (int j = 0; j < Height - 1; j++)
            {
                for (int i = 0; i < Width; i++)
                {
                    if (j == 0) { temp[i] = IsPixelSet(i, j); }

                    SetPixel(i, j, IsPixelSet(i, j + 1));
                }
            }

            for (int i = 0; i < Width; i++)
            {
                SetPixel(i, Height - 1, temp[i]);
            }
        }

        public void ShiftDown()
        {
            var temp = new bool[Width];

            for (int j = Height - 1; j > 0; j--)
            {
                for (int i = 0; i < Width; i++)
                {
                    if (j == Height - 1) { temp[i] = IsPixelSet(i, Height - 1); }

                    SetPixel(i, j, IsPixelSet(i, j - 1));
                }
            }

            for (int i = 0; i < Width; i++)
            {
                SetPixel(i, 0, temp[i]);
            }
        }

        public void Clear()
        {
            for (int i = 0; i < Data.Length; i++)
            {
                Data[i] = 0;
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

            this.Data = data.ToArray();
        }

        public string GetLineText()
        {
            var line = new StringBuilder();

            line.Append(@"{0x");
            var hex = BitConverter.ToString(Data).Replace("-", ", 0x");
            line.Append(hex);
            line.Append(@"}, //");
            line.Append(AsciiValue.ToString("X4"));
            line.Append($"({(char)(AsciiValue)})");

            return line.ToString();
        }
    }
}