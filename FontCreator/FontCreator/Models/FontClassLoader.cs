using FontCreator.Models;
using System;

namespace FontCreator
{
    public static class FontClassLoader
    {
        internal static PixelFont LoadFont(string filename, string code)
        {
            //parse the file name for dimensions
            //let's assume strict filename of FontWxH.cs  .... e.e. Font8x12

            int xIndex = filename.IndexOf('x');
            int periodIndex = filename.IndexOf('.');

            //name starts with "Font" (hense the 4)
            int width = int.Parse(filename.Substring(4, xIndex - 4));
            int height = int.Parse(filename.Substring(xIndex + 1, periodIndex  - xIndex - 1));

            //load the pixel buffer data
            //each line starts with new byte[]
            var lines = code.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            string magicString = "new byte[]";

            //our new font
            var newFont = new PixelFont(width, height, 0x20);

            foreach (var line in lines)
            {
                if(line.Contains(magicString) == false)
                {
                    continue;
                }

                //we have a match .... we'll assume the values are continuous starting from 0x20
                int startIndex = line.IndexOf('{');


            }

            return null;
        }
    }
}
