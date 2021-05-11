﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FS20_HudBar.GUI
{
  class V_RPM : V_Base
  {
    /// <summary>
    /// cTor:
    /// </summary>
    /// <param name="proto"></param>
    public V_RPM( Label proto, bool showUnit )
    : base( proto, showUnit )
    {
      m_unit = "rpm";
      m_default = "_____";
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
          this.Text = UnitString( $"{value,5:####0}" ); // positive only 5 digits
        }
      }
    }

  }
}
    
