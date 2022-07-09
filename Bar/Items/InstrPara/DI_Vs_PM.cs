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

namespace FS20_HudBar.Bar.Items
{
  class DI_Vs_PM : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.VS_PM;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "VS";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Aircraft VS fpm (+- signs)";

    private readonly V_Base _label;
    private readonly V_Base _value1;

    public DI_Vs_PM( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      var item = VItem.VS_PM;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_VSpeedPM( valueProto );
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      m_observerID = SC.SimConnectClient.Instance.HudBarModule.AddObserver( Short, OnDataArrival );
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    public void OnDataArrival( string dataRefName )
    {
      if ( this.Visible ) {
        _value1.Value = Conversions.Round( SC.SimConnectClient.Instance.HudBarModule.VS_ftPmin, 20 ); // steps 20
      }
    }

    // Disconnect from updates
    protected override void UnregisterDataSource( )
    {
      SC.SimConnectClient.Instance.HudBarModule.RemoveObserver( m_observerID );
    }

  }
}
