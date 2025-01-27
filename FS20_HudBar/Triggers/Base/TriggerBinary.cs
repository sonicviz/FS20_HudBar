﻿using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS20_HudBar.Triggers.Base
{
  /// <summary>
  /// A template for Binary Triggers
  /// </summary>
  abstract class TriggerBinary : TriggerBase
  {
    // the internal state, nullable to provide a distinct reset/default level
    protected bool? m_lastTriggered = false;

    // the registered callback list
    protected ConcurrentDictionary<bool, EventProcBinary> m_actions = new ConcurrentDictionary<bool, EventProcBinary>( );

    /// <summary>
    /// cTor: get the speaker 
    /// </summary>
    /// <param name="speaker">A GUI_Speech object to talk from</param>
    public TriggerBinary( GUI.GUI_Speech speaker )
      : base( speaker )
    {
    }

    /// <summary>
    /// Detect binary state changes and trigger registered callbacks if there are
    /// </summary>
    /// <param name="state"></param>
    protected void DetectStateChange( bool state )
    {
      if (state != m_lastTriggered) {
        try {
          if (m_actions.ContainsKey( state )) {
            m_actions[state].Callback.Invoke( m_actions[state].Text ); // trigger the callback
          }
          m_lastTriggered = state; // save new state in any case
        }
        catch {
          // ignore, just don't bail out...
        }
      }
    }


    /// <summary>
    /// Add one Callback (a parameterless void method) for a distinct state
    ///  Overwrites any existing one for the new state
    /// </summary>
    /// <param name="callback">A Callback EventProc</param>
    public override void AddProc( EventProc callback )
    {
      if (!(callback is EventProcBinary)) throw new ArgumentException( "Requires a BinaryEventProc as argument" ); // Program ERROR

      // override existing ones
      m_actions.TryRemove( callback.TriggerState, out _ );
      m_actions.TryAdd( callback.TriggerState, (EventProcBinary)callback );

    }

    /// <summary>
    /// Clears the Event Proc Stack
    /// </summary>
    public override void ClearProcs( )
    {
      m_actions.Clear( );
    }

    /// <summary>
    /// Reset the trigger to callout the current state on the next update
    /// </summary>
    public override void Reset( ) => m_lastTriggered = null;

  }
}
