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
  class DI_CowlFlapsGraph : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.COWL_ANI;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "COWL";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Cowl Flaps Graph";

    private readonly V_Base _label;
    private readonly A_Scale _scale1;
    private readonly A_TwinScale _scale2;

    public DI_CowlFlapsGraph( ValueItemCat vCat, Label lblProto )
    {
      LabelID = LItem;
      var item = VItem.E_COWL_ANI_1;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _scale1 = new A_Scale( ) { Minimum = 0, Maximum = 100, AlertEnabled = false, ItemForeColor = cStep };
      this.AddItem( _scale1 ); vCat.AddLbl( item, _scale1 );

      item = VItem.E_COWL_ANI_2;
      _scale2 = new A_TwinScale( ) { Visible = false, Minimum = 0, Maximum = 100, AlertEnabled = false, ItemForeColor = cStep, ItemForeColor_LScale = cStep };
      this.AddItem( _scale2 ); vCat.AddLbl( item, _scale2 );

      m_observerID = SC.SimConnectClient.Instance.HudBarModule.AddObserver( Short, OnDataArrival );
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    public void OnDataArrival( string dataRefName )
    {
      if ( this.Visible ) {
        _scale1.Visible = ( SC.SimConnectClient.Instance.HudBarModule.NumEngines == 1 );
        _scale2.Visible = ( SC.SimConnectClient.Instance.HudBarModule.NumEngines > 1 );

        if ( _scale1.Visible ) {
          _scale1.Value = SC.SimConnectClient.Instance.HudBarModule.Engine1_cowl_prct; // 0..100
          _scale1.ItemForeColor = ( SC.SimConnectClient.Instance.HudBarModule.Engine1_cowl_prct < 1 ) ? cOK : cStep;
        }
        if ( _scale2.Visible ) {
          _scale2.Value = SC.SimConnectClient.Instance.HudBarModule.Engine1_cowl_prct; // 0..100
          _scale2.ValueLScale = SC.SimConnectClient.Instance.HudBarModule.Engine2_cowl_prct; // 0..100
          _scale2.ItemForeColor = ( SC.SimConnectClient.Instance.HudBarModule.Engine1_cowl_prct < 1 ) ? cOK : cStep;
          _scale2.ItemForeColor_LScale = ( SC.SimConnectClient.Instance.HudBarModule.Engine2_cowl_prct < 1 ) ? cOK : cStep;
        }
      }
    }

    // Disconnect from updates
    protected override void UnregisterDataSource( )
    {
      SC.SimConnectClient.Instance.HudBarModule.RemoveObserver( m_observerID );
    }

  }
}
