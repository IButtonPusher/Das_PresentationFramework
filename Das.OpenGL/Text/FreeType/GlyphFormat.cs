namespace Das.OpenGL.Text.FreeType
{
    public enum GlyphFormat : uint
    {
        /// <summary>
        /// The value 0 is reserved.
        /// </summary>
        None = 0,
        
        Composite = ('c' << 24 | 'o' << 16 | 'm' << 8 | 'p'),
        Bitmap = ('b' << 24 | 'i' << 16 | 't' << 8 | 's'),
        Outline = ('o' << 24 | 'u' << 16 | 't' << 8 | 'l'),

        /// <summary>
        /// The glyph image is a vectorial path with no inside and outside contours. Some Type 1 fonts, like those in
        /// the Hershey family, contain glyphs in this format. These are described as <see cref="Outline"/>, but
        /// FreeType isn't currently capable of rendering them correctly.
        /// </summary>
        Plotter = ('p' << 24 | 'l' << 16 | 'o' << 8 | 't')
    }
}
