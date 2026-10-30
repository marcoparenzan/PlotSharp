using System.Text;
using System.Text.RegularExpressions;

namespace PlotSharp;

public static partial class Charts
{
    public sealed record Segment(double X1, double Y1, double X2, double Y2);
    /// <summary>Segment in the caller's own number type (generic-math contour kernels).</summary>
    public sealed record Segment<T>(T X1, T Y1, T X2, T Y2);
    public sealed record VectorField(double[,] U, double[,] V, double Scale = 1, int Stride = 1, string Color = "#0b0b0b");

    /// <summary>NavierStokes marching squares adapted with validation and masks. Coordinates are sample indices.</summary>
    public static IReadOnlyList<Segment> MarchingSquares(double[,] field, double level, bool[,]? mask = null)
        => MarchingSquares(field, level, mask, preserveDegenerateSegments: false);

    /// <summary>Compatibility mode retains zero-length segments at exact-level vertices; validation, sample coordinates and fixed saddle pairing are unchanged.</summary>
    public static IReadOnlyList<Segment> MarchingSquares(double[,] field, double level, bool[,]? mask, bool preserveDegenerateSegments)
    {
        ValidateSamples(field, mask);
        if (!double.IsFinite(level) || Math.Abs(level) > 1e12) throw new ArgumentOutOfRangeException(nameof(level));
        return ContourKernel.ToDouble(ContourKernel.Trace(field.GetLength(0), field.GetLength(1), (r, c) => field[r, c], c => (double)c, r => (double)r,
            level, mask, forwardOppositeEdges: false, preserveDegenerate: preserveDegenerateSegments, skipNonFiniteCornerSum: false));
    }

    private static void ValidateSamples(double[,] field, bool[,]? mask)
    {
        ArgumentNullException.ThrowIfNull(field);
        if (field.GetLength(0) == 0 || field.GetLength(1) == 0 || field.LongLength > 100000)
            throw new ArgumentException("Fields require 1 to 100000 samples.", nameof(field));
        if (mask is not null && (mask.GetLength(0) != field.GetLength(0) || mask.GetLength(1) != field.GetLength(1)))
            throw new ArgumentException("Mask dimensions must match samples.", nameof(mask));
        for (int r = 0; r < field.GetLength(0); r++)
            for (int c = 0; c < field.GetLength(1); c++)
                if (mask?[r, c] != true && (!double.IsFinite(field[r, c]) || Math.Abs(field[r, c]) > 1e12))
                    throw new ArgumentException("Unmasked samples must be finite and within ±1e12.", nameof(field));
    }

    private static void AppendOverlays(StringBuilder sb, double[,] field, HeatmapOptions o, double plotW, double plotH)
    {
        int rows = field.GetLength(0), cols = field.GetLength(1);
        double X(double col) => 64 + (col + 0.5) / cols * plotW;
        double Y(double row) => 48 + (o.OriginUpper ? (row + 0.5) / rows : 1 - (row + 0.5) / rows) * plotH;
        if (o.ContourLevels is { } levels)
        {
            if (levels.Length > 32 || levels.Any(v => !double.IsFinite(v) || Math.Abs(v) > 1e12))
                throw new ArgumentException("At most 32 finite contour levels within ±1e12 are allowed.", nameof(o));
            if (!Regex.IsMatch(o.ContourColor ?? "", "^#[0-9a-fA-F]{6}$")) throw new ArgumentException("Invalid contour color.", nameof(o));
            sb.Append("<g class='contours' fill='none'>");
            foreach (var level in levels.Distinct())
                foreach (var s in MarchingSquares(field, level, o.Mask))
                    sb.Append($"<path data-level='{Num(level)}' d='M{F(X(s.X1))} {F(Y(s.Y1))}L{F(X(s.X2))} {F(Y(s.Y2))}' stroke='{o.ContourColor}' stroke-width='1.5'/>");
            sb.Append("</g>");
        }
        if (o.Vectors is { } vectors)
        {
            ValidateSamples(vectors.U, o.Mask); ValidateSamples(vectors.V, o.Mask);
            if (vectors.U.GetLength(0) != rows || vectors.U.GetLength(1) != cols || vectors.V.GetLength(0) != rows || vectors.V.GetLength(1) != cols)
                throw new ArgumentException("Vector dimensions must match the heatmap.", nameof(o));
            if (!double.IsFinite(vectors.Scale) || vectors.Scale <= 0 || vectors.Scale > 1e6 || vectors.Stride < 1 || vectors.Stride > 100000 || !Regex.IsMatch(vectors.Color ?? "", "^#[0-9a-fA-F]{6}$"))
                throw new ArgumentException("Invalid vector scale, stride or color.", nameof(o));
            double unitX = o.Extent is { } ex ? plotW / (ex.XMax - ex.XMin) : plotW / cols;
            double unitY = o.Extent is { } ey ? plotH / (ey.YMax - ey.YMin) : plotH / rows;
            sb.Append("<g class='quiver'>");
            for (int r = 0; r < rows; r += vectors.Stride)
                for (int c = 0; c < cols; c += vectors.Stride)
                {
                    if (o.Mask?[r, c] == true) continue;
                    double x0 = X(c), y0 = Y(r);
                    double dx = vectors.U[r, c] * vectors.Scale * unitX;
                    double dy = vectors.V[r, c] * vectors.Scale * unitY * (o.OriginUpper ? 1 : -1);
                    if (!double.IsFinite(dx) || !double.IsFinite(dy) || Math.Abs(dx) > 1e7 || Math.Abs(dy) > 1e7)
                        throw new ArgumentException("Projected vectors must be finite and within ±1e7 pixels.", nameof(o));
                    double length = Math.Sqrt(dx * dx + dy * dy);
                    if (length < 0.5) continue;
                    double ux = dx / length, uy = dy / length, head = Math.Min(8, 0.45 * length), half = head * 0.7 / 2;
                    double x1 = x0 + dx, y1 = y0 + dy;
                    // Geometry adapted from NDSharp.Plot.Quiver: shrink heads on short arrows.
                    sb.Append($"<line x1='{F(x0)}' y1='{F(y0)}' x2='{F(x1 - ux * head * 0.8)}' y2='{F(y1 - uy * head * 0.8)}' stroke='{vectors.Color}' stroke-width='1'/>");
                    sb.Append($"<path d='M{F(x1)} {F(y1)}L{F(x1 - ux * head - uy * half)} {F(y1 - uy * head + ux * half)}L{F(x1 - ux * head + uy * half)} {F(y1 - uy * head - ux * half)}Z' fill='{vectors.Color}'/>");
                }
            sb.Append("</g>");
        }
    }
}
