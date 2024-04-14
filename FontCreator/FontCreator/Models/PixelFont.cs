﻿using System.Collections.Generic;
using System.IO;

namespace FontCreator.Models
{
    class PixelFont
    {
        public List<Character> Characters { get; set; }
        public int Count => Characters.Count;

        public int FontWidth => Characters[0]?.Width ?? 0;

        public int FontHeight => Characters[0]?.Height ?? 0;

        public PixelFont(int rows, int columns, int asciiStart = 0x20, int asciiEnd = 126)
        {
            //Control chars: 0-31
            //127-159 
            var end = asciiEnd;

            if (asciiEnd > 126 && asciiEnd < 160)
                end = 126;

            for (int i = asciiStart; i < end; i++)
            {
                //skip the control characters
                if (i == 127)
                    i += 33;

                Add(new Character(rows, columns, i));
            }
        }

        public void Add(Character character)
        {
            if (Characters == null)
            {
                Characters = new List<Character>();
            }

            Characters.Add(character);
        }

        public Character GetCharacter(int index)
        {
            return Characters[index];
        }

        public void Clear()
        {
            Characters.Clear();
        }

        public void Save()
        {
            if (Characters == null || Characters.Count == 0)
                return;

            var w = Characters[0].Width;
            var h = Characters[0].Height;
            var fileName = ($"font_{w}x{h}.txt");

            using (StreamWriter file = new StreamWriter(Path.Combine(Directory.GetCurrentDirectory(), fileName)))
            {
                foreach (var c in Characters)
                {
                    file.WriteLine(c.GetLineText());
                }
            }

            fileName = ($"font_{w}x{h}.cs");

            using (StreamWriter file = new StreamWriter(Path.Combine(Directory.GetCurrentDirectory(), fileName)))
            {
                foreach (var c in Characters)
                {
                    file.WriteLine("            new byte[]" + c.GetLineText());
                }
            }
        }
    }
}