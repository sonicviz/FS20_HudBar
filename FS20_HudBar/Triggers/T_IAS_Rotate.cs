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
  class T_IAS_Rotate : TriggerFloat
  {
    private TSmoother _smooth = new TSmoother( );

    /// <summary>
    /// Calls to register for dataupdates
    /// </summary>
    public override void RegisterObserver( )
    {
      RegisterObserver_low( SC.SimConnectClient.Instance.HudBarModule, OnDataArrival ); // use generic
    }
    /// <summary>
    /// Calls to un-register for dataupdates
    /// </summary>
    public override void UnRegisterObserver( )
    {
      UnregisterObserver_low( SC.SimConnectClient.Instance.HudBarModule ); // use generic
    }

    /// <summary>
    /// Update the internal state from the datasource
    /// </summary>
    /// <param name="dataSource">An IAircraft object from the FSim library</param>
    protected override void OnDataArrival( string dataRefName )
    {
      if (!Enabled) return; // not enabled
      if (!SC.SimConnectClient.Instance.IsConnected) return; // sanity, capture odd cases

      var ds = SC.SimConnectClient.Instance.HudBarModule;

      // Rotate is only triggered while OnGround and accelerating (else it calls on touchdown)
      if (!ds.Sim_OnGround) return; // not on ground
      if (ds.Accel_AcftZ_fps2 < 0.1f) return; // not accelerating (enough to be considered as takeoff..)

      var rotSpeed = ds.DesingSpeedMinRotation_kt;
      if (rotSpeed < 10) rotSpeed = ds.DesingSpeedTakeoff_kt; // try takeoff speed (some don't have Rot..)
      if (rotSpeed < 10) return; // not properly set or otherwise not meaningful value from SIM

      this.m_actions.ElementAt( 0 ).Value.TriggerStateF.Level = rotSpeed; // set the trigger level
      _smooth.Add( ds.IAS_kt ); // smoothen
      DetectStateChange( _smooth.GetFloat );

      // reset when again on ground and below 10 kt - Rotate will only trigger if within limits +-2
      if (ds.IAS_kt < 10) {
        this.Reset( );
      }
    }

    // Implements the means to speak out the IAS Rotate

    /// <summary>
    /// cTor: 
    /// </summary>
    /// <param name="speaker">A valid Speech obj to speak from</param>
    public T_IAS_Rotate( GUI.GUI_Speech speaker )
      : base( speaker )
    {
      m_name = "IAS Rotate";
      m_test = "Rotate";

      m_lastTriggered = 100.0f; // set triggered when starting up
      // add the proc most likely to be hit as the first - saves some computing time on the long run
      this.AddProc( new EventProcFloat( ) { TriggerStateF = new TriggerBandF( 100.0f, 2.0f ), Callback = Say, Text = "Rotate" } ); // take from design speeds
    }

  }

}

