﻿using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapLib
{
  /// <summary>
  /// A MapImage used for maintaining Maps
  /// </summary>
  sealed class MapImage : StreamImage
  {
    #region MapImage Factory

    /// <summary>
    /// Return a MapImage from a stream
    /// </summary>
    /// <param name="stream">A stream</param>
    /// <param name="mapImageID">A MapImageID</param>
    /// <returns>A MapImage or null</returns>
    public static MapImage FromStream( Stream stream, MapImageID mapImageID )
    {
      var img = new MapImage( stream, mapImageID );
      return img.IsValid ? img : null;
    }


    /// <summary>
    /// Returns a replacement for failed images
    /// </summary>
    /// <param name="mapImageID">A MapImageID</param>
    /// <returns>A MapImage</returns>
    public static MapImage FailedImage( MapImageID mapImageID )
    {
      MapImage mapImage = null;
      using (MemoryStream mstream = new MemoryStream( )) {
        Properties.Resources.RefImage.Save( mstream, ImageFormat.Png );
        mstream.Position = 0;
        mapImage = FromStream( mstream, mapImageID );
      }
      if (mapImage != null)
        mapImage.IsFailedImage = true;

      return mapImage;
    }

    /// <summary>
    /// Return a MapImage from an array of bytes
    /// </summary>
    /// <param name="data">An array of bytes</param>
    /// <param name="mapImageID">A MapImageID</param>
    /// <returns>A MapImage or null</returns>
    public static MapImage FromArray( byte[] data, MapImageID mapImageID )
    {
      // via memory stream from the array
      MapImage mapImage;
      using (var mstream = new MemoryStream( data, 0, data.Length, false, true )) {
        mapImage = FromStream( mstream, mapImageID );
      }
      return mapImage;
    }

    #endregion

    /// <summary>
    /// The ImageID (provider + tile + zoom)
    /// </summary>
    public MapImageID MapImageID { get; private set; }

    /// <summary>
    /// cTor: hidden
    /// </summary>
    private MapImage( ) { }

    /// <summary>
    /// cTor: From Stream (hidden)
    /// </summary>
    /// <param name="stream">An open stream</param>
    /// <param name="mapImageID">An image ID</param>
    private MapImage( Stream stream, MapImageID mapImageID )
      : base( stream )
    {
      MapImageID = mapImageID;
    }


    /// <summary>
    /// Creates an exact Copy of this MapImage
    /// </summary>
    /// <returns>A MapImage or null</returns>
    public new MapImage Clone( )
    {
      if (_image == null) return null;

      return new MapImage( _dataStream, MapImageID );
    }

  }
}
