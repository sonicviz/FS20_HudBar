﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FS20_HudBar.GUI
{
  class V_Dist : V_Base
  {
    /// <summary>
    /// cTor:
    /// </summary>
    /// <param name="proto"></param>
    public V_Dist( Label proto, bool showUnit )
    : base( proto, showUnit )
    {
      m_unit = "nm";
      m_default = "___._";
      Text = UnitString( m_default );
    }

    /// <summary>
    /// Set the value of the Control - formatted as +NN'NN0ft
    /// </summary>
    override public float? Value {
      set {
        if ( value == null ) {
          this.Text = UnitString( m_default );
        }
        else {
          this.Text = UnitString( $"{value,5:##0.0}" ); // sign 5 digits, 1000 separator
        }
      }
    }

  }
}