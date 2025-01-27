﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bm98_Map.Drawing
{
  /// <summary>
  /// Interface for Items to be drawn by the Drawing processor
  /// </summary>
  internal interface IDrawing
  {
    /// <summary>
    /// A Key for an element
    /// </summary>
    int Key { get; set; }

    /// <summary>
    /// The Paint Method
    /// </summary>
    /// <param name="g">Graphics Context</param>
    /// <param name="MapToPixel">Function which converts Coordinates to canvas pixels</param>
    void Paint( Graphics g, Func<CoordLib.LatLon, Point> MapToPixel );
  }
}
