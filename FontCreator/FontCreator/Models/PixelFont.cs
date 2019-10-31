using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FontCreator.Models
{
    class PixelFont
    {
        public List<Character> Characters { get; set; }
        public int Count => Characters.Count;

        public void Add(Character character)
        {
            if(Characters == null)
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
        }
    }
}