﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SC = SimConnectClient;

using FS20_HudBar.Triggers.Base;
using FSimClientIF.Modules;

namespace FS20_HudBar.Triggers
{
  /// <summary>
  /// Waypoint ETE Trigger: a float trigger where 60,30,10 seconds to go
  /// 
  ///  triggers one event each time it changed
  ///  
  ///  One need to add FloatEventProc for Levels
  /// </summary>
  class T_WaypointETE : TriggerFloat
  {
    /// <summary>
    /// Calls to register for dataupdates
    /// </summary>
    public override void RegisterObserver( )
    {
      RegisterObserver_low( SC.SimConnectClient.Instance.GpsModule, OnDataArrival ); // use generic
    }
    /// <summary>
    /// Calls to un-register for dataupdates
    /// </summary>
    public override void UnRegisterObserver( )
    {
      UnregisterObserver_low( SC.SimConnectClient.Instance.GpsModule ); // use generic
    }

    /// <summary>
    /// Update the internal state from the datasource
    /// </summary>
    /// <param name="dataSource">An IGps object from the FSim library</param>
    protected override void OnDataArrival( string dataRefName )
    {
      if (!Enabled) return; // not enabled
      if (!SC.SimConnectClient.Instance.IsConnected) return; // sanity, capture odd cases
      if (SC.SimConnectClient.Instance.HudBarModule.Sim_OnGround) return; // not while on ground

      var ds = SC.SimConnectClient.Instance.GpsModule;
      if (ds.IsGpsFlightplan_active) {
        DetectStateChange( ds.WYP_Ete );
      }
    }

    // Implements the means to speak out the Waypoint ETE in seconds

    /// <summary>
    /// cTor: 
    /// </summary>
    /// <param name="speaker">A valid Speech obj to speak from</param>
    public T_WaypointETE( GUI.GUI_Speech speaker )
      : base( speaker )
    {
      m_name = "GPS Waypoint sec.";
      m_test = "Waypoint in 60";

      // add the proc most likely to be hit as the first - saves some computing time on the long run
      this.AddProc( new EventProcFloat( ) { TriggerStateF = new TriggerBandF( 90.0f, 8.0f ), Callback = Say, Text = "Waypoint in 90" } );
      this.AddProc( new EventProcFloat( ) { TriggerStateF = new TriggerBandF( 60.0f, 6.0f ), Callback = Say, Text = "Waypoint in 60" } );
      this.AddProc( new EventProcFloat( ) { TriggerStateF = new TriggerBandF( 30.0f, 5.0f ), Callback = Say, Text = "Waypoint in 30" } );
    }

  }

}
