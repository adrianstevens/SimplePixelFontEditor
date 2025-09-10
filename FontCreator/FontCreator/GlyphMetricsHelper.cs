using FontCreator.Models;

namespace FontCreator
{
    /// <summary>
    /// Computes horizontal trim/bearing/advance for a bitmap glyph.
    /// This is renderer-only style logic (no format change).
    /// </summary>
    public static class GlyphMetricsHelper
    {
        public readonly struct GlyphMetrics
        {
            public readonly int Left;       // first ink column (inclusive) or -1 if none
            public readonly int Right;      // last ink column (inclusive) or -1 if none
            public readonly int TrimWidth;  // Right-Left+1 or 0 if empty
            public readonly int XOffset;    // Left (bearing)
            public readonly int XAdvance;   // advance to next pen X

            public GlyphMetrics(int left, int right, int trimWidth, int xOffset, int xAdvance)
            {
                Left = left;
                Right = right;
                TrimWidth = trimWidth;
                XOffset = xOffset;
                XAdvance = xAdvance;
            }
        }

        /// <summary>
        /// Compute metrics for the given character bitmap.
        /// </summary>
        /// <param name="ch">Character model with IsPixelSet(x,y)</param>
        /// <param name="nominalWidth">Font's fixed width (currentFont.Width)</param>
        /// <param name="height">Font's height (currentFont.Height)</param>
        /// <param name="letterSpacing">Global extra spacing (e.g., 1)</param>
        /// <param name="spaceAdvance">Advance to use if glyph is empty (e.g., nominalWidth/2)</param>
        /// <param name="minAdvance">Clamp to avoid 0-width punctuation on tiny fonts</param>
        public static GlyphMetrics Compute(Character ch, int nominalWidth, int height, int letterSpacing, int spaceAdvance, int minAdvance)
        {
            int left = -1, right = -1;

            for (int x = 0; x < nominalWidth; x++)
            {
                bool any = false;
                for (int y = 0; y < height; y++)
                {
                    if (ch.IsPixelSet(x, y)) { any = true; break; }
                }
                if (any)
                {
                    if (left == -1) left = x;
                    right = x;
                }
            }

            if (left == -1) // empty glyph (likely space)
            {
                int adv = System.Math.Max(spaceAdvance, minAdvance) + letterSpacing;
                return new GlyphMetrics(-1, -1, 0, 0, adv);
            }

            int width = right - left + 1;
            int advance = System.Math.Max(width, minAdvance) + letterSpacing;

            // XOffset is the left-side bearing (how far bitmap sits right of the pen)
            return new GlyphMetrics(left, right, width, xOffset: left, xAdvance: advance);
        }
    }
}