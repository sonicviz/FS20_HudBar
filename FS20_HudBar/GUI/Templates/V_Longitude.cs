﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using FS20_HudBar.GUI.Templates.Base;

namespace FS20_HudBar.GUI.Templates
{
  /// <summary>
  /// Longitude W180 .. E180
  /// </summary>
  class V_Longitude : V_Base
  {
    /// <summary>
    /// cTor:
    /// </summary>
    /// <param name="proto"></param>
    public V_Longitude( Label proto )
    : base( proto )
    {
      m_unit = "";
      m_default = DefaultString( "E___°__'" );
      Text = UnitString( m_default );
    }

    /// <summary>
    /// Set the value of the Control
    /// </summary>
    override public float? Value {
      set {
        if ( value == null ) {
          this.Text = UnitString( m_default );
        }
        else if ( float.IsNaN( (float)value ) ) {
          this.Text = m_default;
        }
        else {
          string l = CoordLib.Dms.ToLon( (double)value, "dm", 0 );
          this.Text = $"{l,8}";
        }
      }
    }

  }
}
