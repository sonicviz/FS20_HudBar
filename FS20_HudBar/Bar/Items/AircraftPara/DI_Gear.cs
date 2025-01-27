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
  class DI_Gear : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.GEAR;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "Gear";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Gear";

    private readonly V_Base _label;
    private readonly V_Base _value1;

    public DI_Gear( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      var item = VItem.GEAR;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_Steps( signProto );
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

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
      if ( this.Visible ) {
        if ( SC.SimConnectClient.Instance.HudBarModule.IsGearRetractable ) {
          _value1.Step =
              ( SC.SimConnectClient.Instance.HudBarModule.GearPos == FSimClientIF.GearPosition.Down )
              ? Steps.Down
              : ( ( SC.SimConnectClient.Instance.HudBarModule.GearPos == FSimClientIF.GearPosition.Up )
                  ? Steps.Up
                  : Steps.Unk );
        }
        else {
          _value1.Step = Steps.Down;
        }
      }
    }

  }
}

