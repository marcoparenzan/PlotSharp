// Copyright (c) 2026 Marco Parenzan
// Licensed under the MIT License. See the LICENSE file in the project root.
using System.Numerics;
namespace PlotSharp;

/// <summary>Compatibility adapter only; reusable contour mathematics lives in MathemaSharp.Geometry.</summary>
internal static class ContourKernel
{
    internal static IReadOnlyList<Charts.Segment<T>> Trace<T>(int rows,int columns,Func<int,int,T> sample,
        Func<int,T> x,Func<int,T> y,T level,bool[,]? mask,
        bool forwardOppositeEdges,bool preserveDegenerate,bool skipNonFiniteCornerSum) where T:INumber<T>
    {
        var source=MathemaSharp.Geometry.IsoContours.Trace(rows,columns,sample,x,y,level,mask,forwardOppositeEdges,preserveDegenerate,skipNonFiniteCornerSum);
        var result=new Charts.Segment<T>[source.Count];
        for(var i=0;i<result.Length;i++){var s=source[i];result[i]=new(s.X1,s.Y1,s.X2,s.Y2);}return result;
    }
    internal static IReadOnlyList<Charts.Segment> ToDouble(IReadOnlyList<Charts.Segment<double>> segments)
    {
        var result=new Charts.Segment[segments.Count];
        for(var i=0;i<result.Length;i++)result[i]=new(segments[i].X1,segments[i].Y1,segments[i].X2,segments[i].Y2);return result;
    }
}
