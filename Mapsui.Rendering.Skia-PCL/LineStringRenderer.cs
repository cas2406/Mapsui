using System.Collections.Generic;
using Mapsui.Geometries;
using Mapsui.Providers;
using Mapsui.Rendering.Skia.ExtensionMethods;
using Mapsui.Styles;
using SkiaSharp;

namespace Mapsui.Rendering.Skia
{
    public static class LineStringRenderer
    {
        public static void Draw(SKCanvas canvas, IViewport viewport, IStyle style, IFeature feature, IGeometry geometry,
            float opacity)
        {
            if (style is LabelStyle labelStyle)
            {
                var worldCenter = geometry.GetBoundingBox().GetCentroid();
                var center = viewport.WorldToScreen(worldCenter);
                LabelRenderer.Draw(canvas, labelStyle, feature, (float) center.X, (float) center.Y, opacity);
            }
            else
            {

                var lineString = ((LineString) geometry).Vertices;

                float lineWidth = 1;
                var lineColor = new Color();

                var vectorStyle = style as VectorStyle;
                var strokeCap = PenStrokeCap.Butt;
                var strokeStyle = PenStyle.Solid;

                if (vectorStyle != null)
                {
                    lineWidth = (float) vectorStyle.Line.Width;
                    lineColor = vectorStyle.Line.Color;
                    strokeCap = vectorStyle.Line.PenStrokeCap;
                    strokeStyle = vectorStyle.Line.PenStyle;
                }

                var line = WorldToScreen(viewport, lineString);
                var path = ToSkia(line);
                
                using (var paint = new SKPaint())
                {
                    paint.IsStroke = true;
                    paint.StrokeWidth = lineWidth;
                    paint.Color = lineColor.ToSkia(opacity);
                    paint.StrokeJoin = SKStrokeJoin.Round;
                    paint.StrokeCap = strokeCap.ToSkia();
                    if (strokeStyle != PenStyle.Solid)
                        paint.PathEffect = strokeStyle.ToSkia(lineWidth);
                    canvas.DrawPath(path, paint);
                }
            }
        }

        private static SKPath ToSkia(List<Point> vertices)
        {
            var points = new SKPath();

            for (var i = 0; i < vertices.Count; i++)
            {
                if (i == 0)
                {
                    points.MoveTo((float)vertices[i].X, (float)vertices[i].Y);
                }
                else
                {
                    points.LineTo((float)vertices[i].X, (float)vertices[i].Y);
                }
            }
            return points;
        }

        private static List<Point> WorldToScreen(IViewport viewport, IEnumerable<Point> points)
        {
            var result = new List<Point>();
            foreach (var point in points)
            {
                result.Add(viewport.WorldToScreen(point.X, point.Y));
            }
            return result;
        }
    }
}