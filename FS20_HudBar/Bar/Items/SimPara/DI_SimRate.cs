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
using FSimClientIF;

namespace FS20_HudBar.Bar.Items
{
  class DI_SimRate : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.SimRate;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "SimRate";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Sim Rate";

    private readonly B_Base _label;
    private readonly V_Base _value1;

    public DI_SimRate( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      TText = "The Sim Rate\nClick to reset to 1x";
      LabelID = LItem;
      var item = VItem.SimRate;
      _label = new B_Text( item, lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_SRate( value2Proto ) { ItemBackColor=cValBG };
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      _label.ButtonClicked += _label_ButtonClicked;
      _value1.MouseWheel += _value1_MouseWheel;
      _value1.Cursor = Cursors.SizeNS;

      m_observerID = SC.SimConnectClient.Instance.HudBarModule.AddObserver( Short, OnDataArrival );
    }
    // Disconnect from updates
    protected override void UnregisterDataSource( )
    {
      UnregisterObserver_low( SC.SimConnectClient.Instance.HudBarModule ); // use the generic one
    }

    private void _value1_MouseWheel( object sender, MouseEventArgs e )
    {
      // activate the form if the HudBar is not active so at least the most scroll goes only to the HudBar
      _value1.ActivateForm( e );

      if (e.Delta > 0) {
        // Up
        SC.SimConnectClient.Instance.HudBarModule.SimRate_setting( FSimClientIF.CmdMode.Inc );
      }
      else if (e.Delta < 0) {
        // Down
        SC.SimConnectClient.Instance.HudBarModule.SimRate_setting( FSimClientIF.CmdMode.Dec );
      }

    }

    private void _label_ButtonClicked( object sender, ClickedEventArgs e )
    {
      if ( SC.SimConnectClient.Instance.IsConnected ) {
        // eval the steps and exec the Inc, Dec needed
        var steps = Calculator.SimRateStepsToNormal(); // returns the needed steps in either direction
        while ( steps != 0 ) {
          CmdMode dir = (steps>0)? CmdMode.Inc : CmdMode.Dec;
          steps += ( steps > 0 ) ? -1 : 1;
          SC.SimConnectClient.Instance.HudBarModule.SimRate_setting( dir );
        }
      }
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if ( this.Visible ) {
        _value1.Value = SC.SimConnectClient.Instance.HudBarModule.SimRate_rate;
        _value1.ItemForeColor = ( SC.SimConnectClient.Instance.HudBarModule.SimRate_rate != 1.0f ) ? cInverse : cInfo;
        _value1.ItemBackColor = ( SC.SimConnectClient.Instance.HudBarModule.SimRate_rate != 1.0f ) ? cSRATE : cValBG;
      }
    }

  }
}
