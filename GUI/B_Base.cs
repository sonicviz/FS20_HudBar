﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using FS20_HudBar.Bar;

namespace FS20_HudBar.GUI
{
  /// <summary>
  /// A User Button Base Class
  /// </summary>
  abstract class B_Base : Button, IValue
  {
    protected VItem m_mID = VItem.Ad;
    protected string m_default = "";
    protected string m_unit = "";
    protected bool m_showUnit = false;

    private const string c_numbers="0123456789";
    private Random random = new Random();

    virtual public float? Value { set => throw new NotImplementedException( ); }
    virtual public Steps Step { set => throw new NotImplementedException( ); }
    virtual public int? IntValue { set => throw new NotImplementedException( ); }


    /// <summary>
    /// Event triggered when the push button was clicked
    /// </summary>
    public event EventHandler<ClickedEventArgs> ButtonClicked;
    private void OnButtonClicked( )
    {
      ButtonClicked?.Invoke( this, new ClickedEventArgs( m_mID ) );
    }

    /// <summary>
    /// If true shows the unit of value fields
    /// </summary>
    public bool ShowUnit { get => m_showUnit; set => m_showUnit = value; }

    /// <summary>
    /// Add a Unit if ShowUnit is true
    /// </summary>
    /// <param name="valueString">The formatted Value string</param>
    /// <returns>A formatted string</returns>
    protected string UnitString( string valueString )
    {
      return valueString + ( m_showUnit ? m_unit : "" );
    }

    /// <summary>
    /// Debugging, provide the default string with some numbers
    /// </summary>
    /// <param name="defaultString">The formatted Value string</param>
    /// <returns>A formatted string</returns>
    protected string DefaultString( string defaultString )
    {
      string ret = defaultString;
#if DEBUG
      ret = "";

      for ( int i = 0; i < defaultString.Length; i++ ) {
        if ( defaultString[i] == '_' ) {
          ret += c_numbers[random.Next( 10 )];
        }
        else {
          ret += defaultString[i];
        }
      }
#endif
      return ret;
    }


    /// <summary>
    /// cTor: Create a UserControl..
    /// </summary>
    /// <param name="item">The GITem ID of this one</param>
    /// <param name="proto">A label Prototype to derive from</param>
    public B_Base( VItem item, Label proto )
    {
      m_mID = item;
      // Label props
      Font = proto.Font;
      ForeColor = proto.ForeColor;
      BackColor = proto.BackColor;
      AutoSize = true;
      TextAlign = proto.TextAlign;
      Anchor = proto.Anchor;
      Dock = proto.Dock;
      Margin = proto.Margin;
      Text = m_default;
      UseCompatibleTextRendering = true; // make sure the WingDings an other font special chars display properly
      // Button props
      AutoSizeMode = AutoSizeMode.GrowAndShrink;
      FlatStyle = FlatStyle.Flat;
      FlatAppearance.BorderSize = 0;
      FlatAppearance.BorderColor = BackColor;
      FlatAppearance.MouseDownBackColor = Color.Indigo;
      Cursor = Cursors.Hand; // actionable
      TabStop = false;
      base.Click += B_Prct_Click; // capture Click Event
    }

    // subst with our own handler that submits our ID
    private void B_Prct_Click( object sender, EventArgs e )
    {
      OnButtonClicked( );
    }


  }
}
