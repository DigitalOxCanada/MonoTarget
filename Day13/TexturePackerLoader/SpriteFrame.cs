namespace TexturePackerLoader
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class SpriteFrame
    {
        public Color[] BGMaterialColorData { get; set; }

        public SpriteFrame(Texture2D texture, Rectangle sourceRect, Vector2 size, Vector2 pivotPoint, bool isRotated, Texture2D maskTexture = null)
        {
            this.Texture = texture;
            this.MaskTexture = maskTexture;
            this.SourceRectangle = sourceRect;
            this.Size = size;
            //            this.Origin = Vector2.Zero;
            this.Origin = isRotated ? new Vector2(sourceRect.Width * (1 - pivotPoint.Y), sourceRect.Height * pivotPoint.X)
                                    : new Vector2(sourceRect.Width * pivotPoint.X, sourceRect.Height * pivotPoint.Y);
            this.IsRotated = isRotated;
        }

        public Texture2D Texture { get; private set; }
        public Texture2D MaskTexture { get; private set; }

        public Rectangle SourceRectangle { get; private set; }

        public Vector2 Size { get; private set; }

        public bool IsRotated { get; private set; }

        public Vector2 Origin { get; private set; }

        internal void BuildHitMap()
        {
            if (MaskTexture != null)
            {
                BGMaterialColorData = new Color[(int)Size.X * (int)Size.Y];
                MaskTexture.GetData<Color>(0, 0, this.SourceRectangle, BGMaterialColorData, 0, (int)Size.X * (int)Size.Y);
            }
        }
    }
}
