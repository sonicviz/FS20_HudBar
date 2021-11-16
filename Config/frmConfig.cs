﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using FS20_HudBar.Bar;

namespace FS20_HudBar.Config
{
  /// <summary>
  /// The Configuration Form
  ///  Note: some values need to be set before Loading the Form !!!
  /// </summary>
  public partial class frmConfig : Form
  {
    /// <summary>
    /// Profile References must be set by HudBar when creating the Config Window
    /// </summary>
    internal IList<CProfile> ProfilesRef { get; set; } = null;

    /// <summary>
    /// HudBar References must be set by HudBar when creating the Config Window
    /// </summary>
    internal HudBar HudBarRef { get; set; } = null;

    /// <summary>
    /// The currently selected Profile must be set by HudBar before loading the Config Window
    /// </summary>
    internal int SelectedProfile { get; set; } = 0;

    // the number of profiles supported
    private const int c_NumProfiles = 5;

    // per profile indexed access
    private FlowLayoutPanel[] m_flps = new FlowLayoutPanel[c_NumProfiles];
    private TextBox[] m_pName = new TextBox[c_NumProfiles];
    private FlpHandler[] m_flpHandler = new FlpHandler[c_NumProfiles];
    private ComboBox[] m_pFont = new ComboBox[c_NumProfiles];
    private ComboBox[] m_pPlace = new ComboBox[c_NumProfiles];
    private ComboBox[] m_pKind = new ComboBox[c_NumProfiles];
    private ComboBox[] m_pCondensed = new ComboBox[c_NumProfiles];
    private ComboBox[] m_pTransparency = new ComboBox[c_NumProfiles];

    // concurency avoidance
    private bool initDone = false;

    public frmConfig( )
    {
      initDone = false;
      InitializeComponent( );
      // Show the instance name in the Window Border Text
      this.Text = "Hud Bar Configuration - Instance: " + ( string.IsNullOrEmpty( Program.Instance ) ? "Default" : Program.Instance );
      // load the Profile Name Context Menu with items
      ctxMenu.Items.Clear( );
      ctxMenu.Items.Add( "Copy items", null, ctxCopy_Click );
      ctxMenu.Items.Add( "Paste items here", null, ctxPaste_Click );
      ctxMenu.Items.Add( new ToolStripSeparator( ) );

      // Add Aircraft Merges
      var menu = new ToolStripMenuItem( "Aircraft Merges" );
      ctxMenu.Items.Add( menu );
      AcftMerges.AddMenuItems( menu, ctxAP_Click );

      // Add Default Profiles
      menu = new ToolStripMenuItem( "Default Profiles" );
      ctxMenu.Items.Add( menu );
      DefaultProfiles.AddMenuItems( menu, ctxDP_Click );


      // indexed access for profile controls
      m_flps[0] = flp1; m_flps[1] = flp2; m_flps[2] = flp3; m_flps[3] = flp4; m_flps[4] = flp5;
      m_pName[0] = txP1; m_pName[1] = txP2; m_pName[2] = txP3; m_pName[3] = txP4; m_pName[4] = txP5;
      m_pFont[0] = cbxFontP1; m_pFont[1] = cbxFontP2; m_pFont[2] = cbxFontP3; m_pFont[3] = cbxFontP4; m_pFont[4] = cbxFontP5;
      m_pPlace[0] = cbxPlaceP1; m_pPlace[1] = cbxPlaceP2; m_pPlace[2] = cbxPlaceP3; m_pPlace[3] = cbxPlaceP4; m_pPlace[4] = cbxPlaceP5;
      m_pKind[0] = cbxKindP1; m_pKind[1] = cbxKindP2; m_pKind[2] = cbxKindP3; m_pKind[3] = cbxKindP4; m_pKind[4] = cbxKindP5;
      m_pCondensed[0] = cbxCondP1; m_pCondensed[1] = cbxCondP2; m_pCondensed[2] = cbxCondP3; m_pCondensed[3] = cbxCondP4; m_pCondensed[4] = cbxCondP5;
      m_pTransparency[0] = cbxTrans1; m_pTransparency[1] = cbxTrans2; m_pTransparency[2] = cbxTrans3; m_pTransparency[3] = cbxTrans4; m_pTransparency[4] = cbxTrans5;
    }

    // fill the list with items and check them from the Instance
    private void PopulateVoiceCallouts( )
    {
      clbVoice.Items.Clear( );
      foreach ( var vt in HudBarRef.VoicePack.Triggers ) {
        var idx = clbVoice.Items.Add( vt.Name );
        clbVoice.SetItemChecked( idx, vt.Enabled );
      }
    }

    private void PopulateFonts( ComboBox cbx )
    {
      cbx.Items.Clear( );
      cbx.Items.Add( GUI.FontSize.Regular + " Font Size" );
      cbx.Items.Add( GUI.FontSize.Plus_2 + " Font Size" );
      cbx.Items.Add( GUI.FontSize.Plus_4 + " Font Size" );
      cbx.Items.Add( GUI.FontSize.Plus_6 + " Font Size" );
      cbx.Items.Add( GUI.FontSize.Plus_8 + " Font Size" );
      cbx.Items.Add( GUI.FontSize.Plus_10 + " Font Size" );
      cbx.Items.Add( GUI.FontSize.Minus_2 + " Font Size" );
      cbx.Items.Add( GUI.FontSize.Minus_4 + " Font Size" );
      cbx.Items.Add( GUI.FontSize.Plus_12 + " Font Size" );
      cbx.Items.Add( GUI.FontSize.Plus_14 + " Font Size" );
    }

    private void PopulatePlacement( ComboBox cbx )
    {
      cbx.Items.Clear( );
      cbx.Items.Add( GUI.Placement.Bottom + " bound" );
      cbx.Items.Add( GUI.Placement.Left + " bound" );
      cbx.Items.Add( GUI.Placement.Right + " bound" );
      cbx.Items.Add( GUI.Placement.Top + " bound" );
    }

    private void PopulateKind( ComboBox cbx )
    {
      cbx.Items.Clear( );
      cbx.Items.Add( "Bar" );
      cbx.Items.Add( "Tile" );
      cbx.Items.Add( "Window" ); // 20210718
      cbx.Items.Add( "Window no border" ); // 20211022
    }

    private void PopulateCond( ComboBox cbx )
    {
      cbx.Items.Clear( );
      cbx.Items.Add( "Regular Font" );
      cbx.Items.Add( "Condensed Font" );
    }

    private void PopulateTrans( ComboBox cbx )
    {
      cbx.Items.Clear( );
      cbx.Items.Add( "Opaque" ); // GUI.Transparent.T0
      cbx.Items.Add( $"{(int)GUI.Transparent.T10 * 10}%  Transparent" );
      cbx.Items.Add( $"{(int)GUI.Transparent.T20 * 10}%  Transparent" );
      cbx.Items.Add( $"{(int)GUI.Transparent.T30 * 10}%  Transparent" );
      cbx.Items.Add( $"{(int)GUI.Transparent.T40 * 10}%  Transparent" );
      cbx.Items.Add( $"{(int)GUI.Transparent.T50 * 10}%  Transparent" );
      cbx.Items.Add( $"{(int)GUI.Transparent.T60 * 10}%  Transparent" );
      cbx.Items.Add( $"{(int)GUI.Transparent.T70 * 10}%  Transparent" );
      cbx.Items.Add( $"{(int)GUI.Transparent.T80 * 10}%  Transparent" );
      cbx.Items.Add( $"{(int)GUI.Transparent.T90 * 10}%  Transparent" );
    }

    // Load the combo from installed voices
    private void PopulateVoice( ComboBox cbx )
    {
      cbx.Items.Clear( );
      foreach ( var vn in GUI.GUI_Speech.AvailableVoices ) {
        cbx.Items.Add( vn );
      }
    }

    // select the current voice from settings
    public void LoadVoice( ComboBox cbx )
    {
      if ( cbx.Items.Contains( HudBarRef.VoiceName ) )
        cbx.SelectedItem = HudBarRef.VoiceName;
      else if ( cbx.Items.Count > 0 ) {
        cbx.SelectedIndex = 0;
      }
      else {
        // no voices installed...
      }
    }

    // LOAD is called on any invocation of the Dialog
    private void frmConfig_Load( object sender, EventArgs e )
    {
      this.TopMost = false;

      if ( HudBarRef == null ) return; // sanity ..
      if ( ProfilesRef?.Count < c_NumProfiles ) return;// sanity ..

      cbxUnits.Checked = HudBarRef.ShowUnits;
      cbxFltSave.Checked = HudBarRef.FltAutoSave; // 20210821

      PopulateVoice( cbxVoice );// 20211006
      LoadVoice( cbxVoice );
      speech.SetVoice( cbxVoice.SelectedItem.ToString( ) );

      PopulateVoiceCallouts( ); // 20211018

      // for all profiles
      for ( int p = 0; p < c_NumProfiles; p++ ) {
        m_pName[p].Text = ProfilesRef[p].PName;
        m_flpHandler[p] = new FlpHandler( m_flps[p], p + 1,
                              ProfilesRef[p].ProfileString( ), ProfilesRef[p].FlowBreakString( ), ProfilesRef[p].ItemPosString( ) );
        m_flpHandler[p].LoadFlp( HudBarRef );
        PopulateFonts( m_pFont[p] );
        ProfilesRef[p].LoadFontSize( m_pFont[p] );
        PopulatePlacement( m_pPlace[p] );
        ProfilesRef[p].LoadPlacement( m_pPlace[p] );
        PopulateKind( m_pKind[p] );
        ProfilesRef[p].LoadKind( m_pKind[p] );
        PopulateCond( m_pCondensed[p] );
        ProfilesRef[p].LoadCond( m_pCondensed[p] );
        PopulateTrans( m_pTransparency[p] );
        ProfilesRef[p].LoadTrans( m_pTransparency[p] );

        // mark the selected one 
        if ( SelectedProfile == p ) {
          m_pName[p].BackColor = Color.LimeGreen;
        }
      }

      initDone = true;
    }

    private void frmConfig_FormClosing( object sender, FormClosingEventArgs e )
    {
      // reset Sel Color
      for ( int p = 0; p < c_NumProfiles; p++ ) {
        m_pName[p].BackColor = this.BackColor;
      }
    }

    private void btCancel_Click( object sender, EventArgs e )
    {
      this.DialogResult = DialogResult.Cancel;
      this.Close( );
    }

    private void btAccept_Click( object sender, EventArgs e )
    {
      // update from edits
      // live update to HUD
      HudBarRef.SetShowUnits( cbxUnits.Checked );
      HudBarRef.SetFltAutoSave( cbxFltSave.Checked );
      HudBarRef.SetVoiceName( cbxVoice.SelectedItem.ToString( ) );
      int idx = 0;
      foreach ( var vt in HudBarRef.VoicePack.Triggers ) {
        vt.Enabled = clbVoice.GetItemChecked( idx++ );
      }
      HudBarRef.VoicePack.SaveSettings( );

      // record profile Updates
      for ( int p = 0; p < c_NumProfiles; p++ ) {
        ProfilesRef[p].PName = m_pName[p].Text.Trim( );
        ProfilesRef[p].GetItemsFromFlp( m_flps[p] );
        ProfilesRef[p].GetFontSizeFromCombo( m_pFont[p] );
        ProfilesRef[p].GetPlacementFromCombo( m_pPlace[p] );
        ProfilesRef[p].GetKindFromCombo( m_pKind[p] );
        ProfilesRef[p].GetCondFromCombo( m_pCondensed[p] );
        ProfilesRef[p].GetTramsFromCombo( m_pTransparency[p] );
      }

      this.DialogResult = DialogResult.OK;
      this.Close( );
    }

    // local instance for tests
    private GUI.GUI_Speech speech = new GUI.GUI_Speech();

    private void cbxVoice_SelectedIndexChanged( object sender, EventArgs e )
    {
      speech.SetVoice( cbxVoice.SelectedItem.ToString( ) );
    }
    private void cbxVoice_MouseClick( object sender, MouseEventArgs e )
    {
      if ( cbxVoice.DroppedDown ) return;
      speech.SetVoice( cbxVoice.SelectedItem.ToString( ) );
      speech.SaySynched( 100 );
    }

    private void clbVoice_SelectedIndexChanged( object sender, EventArgs e )
    {
      if ( clbVoice.SelectedIndex < 0 ) return;
      if ( !clbVoice.GetItemChecked( clbVoice.SelectedIndex ) ) return;
      if ( !initDone ) return; // don't talk at startup

      // Test when checked
      HudBarRef.VoicePack.Triggers[clbVoice.SelectedIndex].Test( speech );
    }

    #region Context Menu

    // Buffer to maintain copied items
    private ProfileStore m_copyBuffer;

    // Copy Items is clicked
    private void ctxCopy_Click( object sender, EventArgs e )
    {
      var ctx = (sender as ToolStripItem).Owner as ContextMenuStrip;
      var col = tlp.GetColumn(ctx.SourceControl);
      // col is the profile index assuming Col 0..4 carry the profiles...
      if ( col > c_NumProfiles ) return; // sanity
      m_copyBuffer = m_flpHandler[col].GetItemsFromFlp( );
    }

    // Paste items is clicked
    private void ctxPaste_Click( object sender, EventArgs e )
    {
      var ctx = (sender as ToolStripItem).Owner as ContextMenuStrip;
      var col = tlp.GetColumn(ctx.SourceControl);
      // col is the profile index assuming Col 0..4 carry the profiles...
      if ( col > c_NumProfiles ) return; // sanity

      if ( m_copyBuffer != null ) {
        m_flpHandler[col].LoadDefaultProfile( m_copyBuffer );
        m_flpHandler[col].LoadFlp( HudBarRef );
      }
    }

    // A default profile is clicked
    private void ctxDP_Click( object sender, EventArgs e )
    {
      var tsi = (sender as ToolStripItem);
      object item = tsi.Owner;
      // backup the menu tree
      while ( !( item is ContextMenuStrip ) ) {
        if ( item is ToolStripDropDownMenu )
          item = ( item as ToolStripDropDownMenu ).OwnerItem;
        else if ( item is ToolStripMenuItem )
          item = ( item as ToolStripMenuItem ).Owner;
        else
          return; // not an expected menu tree 
      }
      var ctx = item as ContextMenuStrip;
      // col is the profile index assuming Col 0..4 carry the profiles...
      var col = tlp.GetColumn(ctx.SourceControl);
      if ( col > c_NumProfiles ) return; // sanity

      var dp = DefaultProfiles.GetDefaultProfile( tsi.Text );
      if ( dp != null ) {
        m_flpHandler[col].LoadDefaultProfile( dp );
        m_flpHandler[col].LoadFlp( HudBarRef );
        m_pName[col].Text = dp.Name;
      }
    }

    // An aircraft merge profile is clicked
    private void ctxAP_Click( object sender, EventArgs e )
    {
      var tsi = (sender as ToolStripItem);
      object item = tsi.Owner;
      // backup the menu tree
      while ( !(item is ContextMenuStrip ) ) {
        if ( item is ToolStripDropDownMenu )
          item = ( item as ToolStripDropDownMenu ).OwnerItem;
        else if ( item is ToolStripMenuItem )
          item = ( item as ToolStripMenuItem ).Owner;
        else
          return; // not an expected menu tree 
      }
      var ctx = item as ContextMenuStrip;
      // col is the profile index assuming Col 0..4 carry the profiles...
      var col = tlp.GetColumn(ctx.SourceControl);
      if ( col > c_NumProfiles ) return; // sanity

      var dp = AcftMerges.GetAircraftProfile( tsi.Text );
      if ( dp != null ) {
        m_flpHandler[col].MergeProfile( dp.Profile );
        m_flpHandler[col].LoadFlp( HudBarRef );
        m_pName[col].Text = dp.Name;
      }
    }

    #endregion

  }
}