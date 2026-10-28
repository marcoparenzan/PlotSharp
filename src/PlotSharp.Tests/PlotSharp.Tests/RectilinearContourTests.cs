namespace PlotSharp.Tests;

public class RectilinearContourTests
{
    [Fact]
    public void NonUniformCoordinatesAreInterpolatedPerPhysicalEdge()
    {
        var segments = RectilinearContours.Trace([0, 1, 10], [0, 1, 4], [0, 1, 2, 0, 1, 2, 0, 1, 2], 1.5);
        Assert.Equal(new[] { new Charts.Segment(5.5, 0, 5.5, 1), new Charts.Segment(5.5, 1, 5.5, 4) }, segments);
    }

    [Fact]
    public void ExactVertexAndSaddleRetainNDConvention()
    {
        Assert.Equal(new[] { new Charts.Segment(0, 0, 0, 0) }, RectilinearContours.Trace([0, 10], [0, 4], [0, -1, -1, -1], 0));
        Assert.Equal(new[] { new Charts.Segment(2.5, 0, 10, 3), new Charts.Segment(7.5, 4, 0, 1) },
            RectilinearContours.Trace([0, 10], [0, 4], [0, 1, 1, 0], 0.25));
    }

    [Fact]
    public void MatchesPreMigrationEdgeArithmeticExactly()
    {
        double[] xs = [-3.1, 0.2, 8.7], ys = [-2.3, 1.4, 5.9];
        double[] values = [-1.3, 0.7, 2.1, 1.8, -0.4, 0.3, double.NaN, 0.9, -1.1];
        foreach (double level in new[] { -0.3, 0.3, 0.8 })
            Assert.Equal(Legacy(xs, ys, values, level).ToArray(), RectilinearContours.Trace(xs, ys, values, level));
    }

    [Fact]
    public void SummedCornerOverflowIsSkippedAsInND()
        => Assert.Empty(RectilinearContours.Trace([0, 1], [0, 1], [1e308, 1e308, -1e308, -1e308], 0));

    [Fact]
    public void DescendingFiniteAxesAndSingletonsRetainGeometryScope()
    {
        Assert.Equal(Legacy([10, 1, 0], [4, 1, 0], [0, 1, 2, 0, 1, 2, 0, 1, 2], 1.5).ToArray(),
            RectilinearContours.Trace([10, 1, 0], [4, 1, 0], [0, 1, 2, 0, 1, 2, 0, 1, 2], 1.5));
        Assert.Empty(RectilinearContours.Trace([0], [0], [1], 0));
    }

    [Fact]
    public void MalformedAxesAndDataAreRejected()
    {
        Assert.Throws<ArgumentException>(() => RectilinearContours.Trace([0, 1], [0, 1], [1], 0));
        Assert.Throws<ArgumentException>(() => RectilinearContours.Trace([0, double.NaN], [0, 1], [0, 1, 0, 1], 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => RectilinearContours.Trace([0, 1], [0, 1], [0, 1, 0, 1], double.NaN));
    }

    // Baseline arithmetic from the MIT NDSharp DrawLines algorithm before delegation; no drawing/state policy.
    private static List<Charts.Segment> Legacy(double[] xs, double[] ys, double[] z, double level)
    {
        var result = new List<Charts.Segment>();
        for (int r = 0; r + 1 < ys.Length; r++) for (int col = 0; col + 1 < xs.Length; col++)
        {
            double a = z[r * xs.Length + col], b = z[r * xs.Length + col + 1], c = z[(r + 1) * xs.Length + col + 1], d = z[(r + 1) * xs.Length + col];
            if (!double.IsFinite(a + b + c + d)) continue;
            var points = new List<(double X, double Y)>();
            void Edge(double v1, double v2, double x1, double y1, double x2, double y2)
            {
                if ((v1 < level) == (v2 < level)) return;
                double t = (level - v1) / (v2 - v1);
                points.Add((x1 + t * (x2 - x1), y1 + t * (y2 - y1)));
            }
            Edge(a, b, xs[col], ys[r], xs[col + 1], ys[r]);
            Edge(b, c, xs[col + 1], ys[r], xs[col + 1], ys[r + 1]);
            Edge(d, c, xs[col], ys[r + 1], xs[col + 1], ys[r + 1]);
            Edge(a, d, xs[col], ys[r], xs[col], ys[r + 1]);
            for (int i = 0; i + 1 < points.Count; i += 2)
                result.Add(new(points[i].X, points[i].Y, points[i + 1].X, points[i + 1].Y));
        }
        return result;
    }
}
