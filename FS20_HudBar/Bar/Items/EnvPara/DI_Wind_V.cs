﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SC = SimConnectClient;
using static FS20_HudBar.GUI.GUI_Colors;
using static FS20_HudBar.GUI.GUI_Colors.ColorType;

using FS20_HudBar.Bar.Items.Base;
using FS20_HudBar.GUI;
using FS20_HudBar.GUI.Templates;
using FS20_HudBar.GUI.Templates.Base;
using System.Drawing;

namespace FS20_HudBar.Bar.Items
{
  class DI_Wind_V : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.VWIND;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "LIFT";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Wind vertical kt";

    private readonly V_Base _label;
    private readonly V_Base _value1;
    private readonly A_WindDot _wind;

    public DI_Wind_V( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      // Wind Direction, Speed
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );

      var item = VItem.VWIND;
      _value1 = new V_Speed( value2Proto );
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      item = VItem.VWIND_ANI;
      _wind = new A_WindDot( ) { BorderStyle = BorderStyle.FixedSingle, AutoSizeWidth = true };
      this.AddItem( _wind ); vCat.AddLbl( item, _wind );

      m_observerID = SC.SimConnectClient.Instance.HudBarModule.AddObserver( Short, OnDataArrival );
    }
    // Disconnect from updates
    protected override void UnregisterDataSource( )
    {
      UnregisterObserver_low( SC.SimConnectClient.Instance.HudBarModule ); // use the generic one
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        _value1.Value = SC.SimConnectClient.Instance.HudBarModule.WindVert_kt;
        _wind.Value = SC.SimConnectClient.Instance.HudBarModule.WindVert_mPerSec;
      }
    }

  }
}
