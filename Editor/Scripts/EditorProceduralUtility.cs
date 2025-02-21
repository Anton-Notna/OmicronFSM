using UnityEngine;

namespace OmicronFSM.Editor
{
    public static class EditorProceduralUtility
    {
        public static Texture2D CreateButtonRoundTexture(Color color, Color edgeColor, int resolution = 16)
        {
            Color[] pixels = new Color[resolution * resolution];
            FillPixelsRound(pixels, new Vector2Int(resolution, resolution), color, edgeColor);

            var result = new Texture2D(resolution, resolution);
            result.SetPixels(pixels);
            result.Apply();
            return result;
        }

        public static RectOffset ComputeRoundOffset(this Texture2D texture, float relativeOffset = 0.5f)
        {
            int borderX = Mathf.RoundToInt(texture.width * relativeOffset);
            int borderY = Mathf.RoundToInt(texture.height * relativeOffset);
            return new RectOffset(borderX, borderX, borderY, borderY);
        }

        private static void FillPixelsRound(Color[] pixels, Vector2Int resolution, Color color, Color colorEdge)
        {
            float radius = Mathf.Min(resolution.x, resolution.y) * 0.5f;
            float smoothRadiusMin = radius - 1f;
            Vector2 center = new Vector2(resolution.x, resolution.y) * 0.5f;

            for (int y = 0; y < resolution.y; y++)
            {
                for (int x = 0; x < resolution.x; x++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), center);

                    float alpha = (distance - smoothRadiusMin) / (radius - smoothRadiusMin);
                    alpha = 1f - Mathf.Clamp01(alpha);

                    float colorT = Mathf.Clamp01(distance / radius);

                    Color pixelColor = Color.Lerp(color, colorEdge, colorT);
                    pixelColor.a *= alpha;

                    int index = y * resolution.x + x;
                    pixels[index] = pixelColor;
                }
            }
        }
    }
}