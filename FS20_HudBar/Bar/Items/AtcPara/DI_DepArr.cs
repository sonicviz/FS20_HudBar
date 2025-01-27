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
using CoordLib;
using FS20_HudBar.GUI.Templates.Base;

namespace FS20_HudBar.Bar.Items
{
  class DI_DepArr : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.DEPARR;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "RTE";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Departure / Arrival";

    private readonly B_Base _label;
    private readonly V_Base _value1;
    private readonly V_Base _value2;


    public DI_DepArr( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      var item = VItem.DEPARR_DEP;
      _label = new B_Text( item, lblProto ) { Text = Short }; this.AddItem( _label );
      _label.Cursor = Cursors.Hand;
      _label.MouseClick += _label_MouseClick;

      _value1 = new V_ICAO_L( value2Proto );
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      item = VItem.DEPARR_ARR;
      _value2 = new V_ICAO( value2Proto );
      this.AddItem( _value2 ); vCat.AddLbl( item, _value2 );

      m_observerID = SC.SimConnectClient.Instance.HudBarModule.AddObserver( Short, OnDataArrival );// use the Location tracer
    }
    // Disconnect from updates
    protected override void UnregisterDataSource( )
    {
      UnregisterObserver_low( SC.SimConnectClient.Instance.HudBarModule ); // use the generic one
    }

    private void _label_MouseClick( object sender, MouseEventArgs e )
    {
      if (!SC.SimConnectClient.Instance.IsConnected) return;

      var TTX = new Config.frmApt( );
      // load default
      if (SC.SimConnectClient.Instance.FlightPlanModule.FlightPlan.HasFlightPlan) {
        // dest from FPLan
        TTX.DepAptICAO = SC.SimConnectClient.Instance.FlightPlanModule.FlightPlan.Departure;
        TTX.ArrAptICAO = SC.SimConnectClient.Instance.FlightPlanModule.FlightPlan.Destination;
      }
      else {
        // no Flightplan
        if (AirportMgr.IsDepAvailable) {
          // departure from Mgr (prev entry)
          TTX.DepAptICAO = AirportMgr.DepAirportICAO;
        }
        else {
          // no preset
          TTX.DepAptICAO = "";
        }

        if (AirportMgr.IsArrAvailable) {
          // destination from Mgr (prev entry)
          TTX.ArrAptICAO = AirportMgr.ArrAirportICAO;
        }
        else {
          // no preset
          TTX.ArrAptICAO = "";
        }
      }

      if (TTX.ShowDialog( this ) == DialogResult.OK) {
        // Update DEP
        if (string.IsNullOrWhiteSpace( TTX.DepAptICAO )) {
          // empty entry to clear
          if (SC.SimConnectClient.Instance.FlightPlanModule.FlightPlan.HasFlightPlan) {
            // update with FP destination
            AirportMgr.UpdateDep( SC.SimConnectClient.Instance.FlightPlanModule.FlightPlan.Departure );
          }
          else {
            // clear with N.A. airport
            AirportMgr.UpdateDep( AirportMgr.AirportNA_Icao );
          }
        }
        else {
          // user entry - will be checked in the Mgr
          AirportMgr.UpdateDep( TTX.DepAptICAO );
        }
        // Update ARR
        if (string.IsNullOrWhiteSpace( TTX.ArrAptICAO )) {
          // empty entry to clear
          if (SC.SimConnectClient.Instance.FlightPlanModule.FlightPlan.HasFlightPlan) {
            // update with FP destination
            AirportMgr.UpdateArr( SC.SimConnectClient.Instance.FlightPlanModule.FlightPlan.Destination );
          }
          else {
            // clear with N.A. airport
            AirportMgr.UpdateArr( AirportMgr.AirportNA_Icao );
          }
        }
        else {
          // user entry - will be checked in the Mgr
          AirportMgr.UpdateArr( TTX.ArrAptICAO );
        }
      }
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        _value1.Text = AirportMgr.IsDepAvailable ? AirportMgr.DepAirportICAO : "...."; // default text
        _value2.Text = AirportMgr.IsArrAvailable ? AirportMgr.ArrAirportICAO : "...."; // default text
      }
    }

  }
}

