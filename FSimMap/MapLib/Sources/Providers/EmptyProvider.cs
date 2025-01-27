﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapLib.Sources.Providers
{
  /// <summary>
  /// A dummy Provider
  /// </summary>
  internal sealed class EmptyProvider : MapProviderBase
  {
    public static EmptyProvider Instance => lazy.Value;
    private static readonly Lazy<EmptyProvider> lazy = new Lazy<EmptyProvider>( ( ) => new EmptyProvider( ) );
    private EmptyProvider( )
      : base( MapProvider.DummyProvider )
    {
      MaxZoom = null;
    }

    #region ProviderBase Members

    public override string ContentID => "Empty Provider Master Content ID";

    public override Guid Id => Guid.Empty;

    public override string Name { get; } = "Empty - not to be used";

    public override MapImage GetTileImage( MapImageID mapImageID )
    {
      return null;
    }

    #endregion
  }
}
