﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS20_HudBar
{
  /// <summary>
  /// Some general tools are located here..
  /// </summary>
  class Conversions
  {
    private const float c_nmPerM = 5.399568e-4f;
    private const float c_degF = 9f/5f;
    /// <summary>
    /// Nautical Miles from Meters
    /// </summary>
    /// <param name="meter">Meter</param>
    /// <returns>Nautical Miles</returns>
    public static float NmFromM( float meter )
    {
      return meter * c_nmPerM;
    }

    /// <summary>
    /// Converst from DegC to DegF ((DEG°C * 9/5) + 32 = 32°F)
    /// </summary>
    /// <param name="degC"></param>
    /// <returns>Temp in deg F</returns>
    public static float DegCtoF( float degC )
    {
      return ( degC * c_degF ) + 32.0f;
    }

    /// <summary>
    /// Derives Apt from an ATC Translatable Apt string
    /// </summary>
    /// <param name="atcApt">The ATC Apt string</param>
    /// <returns>The ICAO Apt part</returns>
    public static string AptFromATCApt( string atcApt )
    {
      if ( string.IsNullOrEmpty( atcApt ) ) return AirportMgr.AirportNA_Icao; // seen a null here...

      // arrives as TT:AIRPORTLR.ICAO.name
      string[] e = atcApt.Split ( new char[]{'.'});
      if ( e.Length > 1 )
        return e[1];

      return AirportMgr.AirportNA_Icao;
    }

    /// <summary>
    /// Round the number in Quants given
    /// </summary>
    /// <param name="number">The number</param>
    /// <param name="quant">Quantities to round to</param>
    /// <returns>The rounded number</returns>
    public static float Round( float number, int quant )
    {
      return (float)( Math.Round( number / quant ) * quant );
    }
  }
}