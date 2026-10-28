namespace PlotSharp;

/// <summary>Shared cell traversal/interpolation; callers own validation and their historical edge convention.</summary>
internal static class ContourKernel
{
    internal static IReadOnlyList<Charts.Segment> Trace(int rows, int columns, Func<int, int, double> sample,
        Func<int, double> x, Func<int, double> y, double level, bool[,]? mask,
        bool forwardOppositeEdges, bool preserveDegenerate, bool skipNonFiniteCornerSum)
    {
        var segments = new List<Charts.Segment>();
        for (int row = 0; row + 1 < rows; row++)
            for (int col = 0; col + 1 < columns; col++)
            {
                if (mask?[row, col] == true || mask?[row, col + 1] == true || mask?[row + 1, col] == true || mask?[row + 1, col + 1] == true) continue;
                double a = sample(row, col), b = sample(row, col + 1), c = sample(row + 1, col + 1), d = sample(row + 1, col);
                // Preserve NDSharp's summed-corner test, including overflow of individually finite values.
                if (skipNonFiniteCornerSum && !double.IsFinite(a + b + c + d)) continue;
                double x0 = x(col), x1 = x(col + 1), y0 = y(row), y1 = y(row + 1);
                var points = new List<(double X, double Y)>(4);
                void Edge(double v1, double v2, double px1, double py1, double px2, double py2)
                {
                    if ((v1 < level) == (v2 < level)) return;
                    double t = (level - v1) / (v2 - v1);
                    points.Add((px1 + t * (px2 - px1), py1 + t * (py2 - py1)));
                }
                Edge(a, b, x0, y0, x1, y0);
                Edge(b, c, x1, y0, x1, y1);
                if (forwardOppositeEdges)
                {
                    Edge(d, c, x0, y1, x1, y1);
                    Edge(a, d, x0, y0, x0, y1);
                }
                else
                {
                    Edge(c, d, x1, y1, x0, y1);
                    Edge(d, a, x0, y1, x0, y0);
                }
                for (int i = 0; i + 1 < points.Count; i += 2)
                {
                    var p = points[i]; var q = points[i + 1];
                    if (preserveDegenerate || p != q) segments.Add(new(p.X, p.Y, q.X, q.Y));
                }
            }
        return segments;
    }
}
