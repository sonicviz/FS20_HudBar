﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using FS20_HudBar.Bar;
using FS20_HudBar.GUI.Templates.Base;

namespace FS20_HudBar.Config
{
  /// <summary>
  /// The Configuration Form
  ///  Note: some values need to be set before Loading the Form !!!
  /// </summary>
  public partial class frmConfig : Form
  {
    /// <summary>
    /// HudBar References must be set by HudBar when creating the Config Window
    /// </summary>
    internal HudBar HudBarRef { get; set; } = null;

    /// <summary>
    /// Profile References must be set by HudBar when creating the Config Window
    /// </summary>
    internal IList<CProfile> ProfilesRef { get; set; } = null;
    /// <summary>
    /// Temp Profiles used while editing
    /// </summary>
    private List<CProfile> ProfilesCache = new List<CProfile>( );


    /// <summary>
    /// The currently selected Profile must be set by HudBar before loading the Config Window
    /// </summary>
    internal int SelectedProfile { get; set; } = 0;

    // the number of profiles to edit
    private const int c_NumProfilesShown = 5;
    // starting number of the supported sections 
    private readonly int[] c_ProfileSectionStart = new int[] { 0, 0 + c_NumProfilesShown };
    // index to the section in use 0 OR 1 for now
    private int m_profileSectionInUse = 0;

    // per profile indexed access
    private FlowLayoutPanel[] m_flps = new FlowLayoutPanel[c_NumProfilesShown];
    private TextBox[] m_pName = new TextBox[c_NumProfilesShown];
    private FlpHandler[] m_flpHandler = new FlpHandler[c_NumProfilesShown];
    private ComboBox[] m_pFont = new ComboBox[c_NumProfilesShown];
    private ComboBox[] m_pPlace = new ComboBox[c_NumProfilesShown];
    private ComboBox[] m_pKind = new ComboBox[c_NumProfilesShown];
    private ComboBox[] m_pCondensed = new ComboBox[c_NumProfilesShown];
    private ComboBox[] m_pTransparency = new ComboBox[c_NumProfilesShown];
    private TextBox[] m_pHotkey = new TextBox[c_NumProfilesShown];

    // internal temporary list only
    private WinHotkeyCat m_hotkeys = new WinHotkeyCat( );
    private FrmHotkey HKdialog = new FrmHotkey( );

    private FrmFonts FONTSdialog;
    private GUI.GUI_Fonts m_configFonts;
    private bool m_applyFontChanges = false;

    private ToolTip_Base m_tooltip = new ToolTip_Base( );

    // concurency avoidance
    private bool initDone = false;

    // section start offset 
    private int SectionStart { get => c_ProfileSectionStart[m_profileSectionInUse]; }

    // fill the list with items and check them from the Instance
    private void PopulateASave( ComboBox cbx )
    {
      cbx.Items.Clear( );
      cbx.Items.Add( "AutoBackup DISABLED" );
      cbx.Items.Add( "AutoBackup (5 Min)" );
      cbx.Items.Add( "AutoBackup + ATC" );
    }

    private void SetVoiceCalloutState( )
    {
      clbVoice.Items.Clear( );
      foreach (var vt in HudBarRef.VoicePack.Triggers) {
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
      // added 20220212
      cbx.Items.Add( GUI.FontSize.Plus_18 + " Font Size" );
      cbx.Items.Add( GUI.FontSize.Plus_20 + " Font Size" );
      cbx.Items.Add( GUI.FontSize.Plus_24 + " Font Size" );
      cbx.Items.Add( GUI.FontSize.Plus_28 + " Font Size" );
      // added 20220304
      cbx.Items.Add( GUI.FontSize.Plus_32 + " Font Size" );
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
      foreach (var vn in GUI.GUI_Speech.AvailableVoices) {
        cbx.Items.Add( vn );
      }
    }

    // select the current voice from settings
    public void LoadVoice( ComboBox cbx )
    {
      if (cbx.Items.Contains( HudBarRef.VoiceName ))
        cbx.SelectedItem = HudBarRef.VoiceName;
      else if (cbx.Items.Count > 0) {
        cbx.SelectedIndex = 0;
      }
      else {
        // no voices installed...
      }
    }

    private void SetHotkeyTexts( )
    {
      txHkShowHide.Text = m_hotkeys.ContainsKey( Hotkeys.Show_Hide ) ? m_hotkeys[Hotkeys.Show_Hide].AsString : "";
      txHkShelf.Text = m_hotkeys.ContainsKey( Hotkeys.FlightBag ) ? m_hotkeys[Hotkeys.FlightBag].AsString : "";
      txHkCamera.Text = m_hotkeys.ContainsKey( Hotkeys.Camera ) ? m_hotkeys[Hotkeys.Camera].AsString : "";
      txHkChecklistBox.Text = m_hotkeys.ContainsKey( Hotkeys.ChecklistBox ) ? m_hotkeys[Hotkeys.ChecklistBox].AsString : "";
    }


    // Initializes the selected Profile Section
    // from m_profileSectionInUse
    private void InitProfileSection( )
    {
      LoadProfileSection( SectionStart );
    }

    // Loads a section of profiles 
    // i.e. the number of shown profiles starting from the startProfileNum (1..N)
    private void LoadProfileSection( int startProfileNum )
    {
      // for all profiles
      for (int p = 0; p < c_NumProfilesShown; p++) {
        m_pName[p].Text = ProfilesCache[(p + startProfileNum)].PName;
        m_flpHandler[p]?.Dispose( );
        m_flpHandler[p] = new FlpHandler(
          m_flps[p], p,
          ProfilesCache[(p + startProfileNum)].ProfileString( ),
          ProfilesCache[(p + startProfileNum)].FlowBreakString( ),
          ProfilesCache[(p + startProfileNum)].ItemPosString( )
        );
        m_flpHandler[p].LoadFlp( HudBarRef );
        ProfilesCache[(p + startProfileNum)].LoadFontSize( m_pFont[p] );
        ProfilesCache[(p + startProfileNum)].LoadPlacement( m_pPlace[p] );
        ProfilesCache[(p + startProfileNum)].LoadKind( m_pKind[p] );
        ProfilesCache[(p + startProfileNum)].LoadCond( m_pCondensed[p] );
        ProfilesCache[(p + startProfileNum)].LoadTrans( m_pTransparency[p] );

        m_pHotkey[p].Text = m_hotkeys.ContainsKey( (Hotkeys)(p + startProfileNum) ) ? m_hotkeys[(Hotkeys)(p + startProfileNum)].AsString : "";

        // mark the selected one 
        m_pName[p].BackColor = (SelectedProfile == (p + startProfileNum)) ? Color.LimeGreen : Color.White;
      }
    }

    // temp store the edits of the Section starting with the given profile number
    private void CacheProfileSection( int startProfileNum )
    {
      // record profile Updates from the controls
      for (int p = 0; p < c_NumProfilesShown; p++) {
        ProfilesCache[p + startProfileNum].PName = m_pName[p].Text.Trim( );
        ProfilesCache[p + startProfileNum].GetItemsFromFlp( m_flps[p], p );
        ProfilesCache[p + startProfileNum].GetFontSizeFromCombo( m_pFont[p] );
        ProfilesCache[p + startProfileNum].GetPlacementFromCombo( m_pPlace[p] );
        ProfilesCache[p + startProfileNum].GetKindFromCombo( m_pKind[p] );
        ProfilesCache[p + startProfileNum].GetCondensedFromCombo( m_pCondensed[p] );
        ProfilesCache[p + startProfileNum].GetTransparencyFromCombo( m_pTransparency[p] );

        m_hotkeys.MaintainHotkeyString( (Hotkeys)(p + startProfileNum), m_pHotkey[p].Text );
      }
    }

    /// <summary>
    /// cTor: for the Form
    /// </summary>
    public frmConfig( )
    {
      initDone = false;
      InitializeComponent( );

      // Show the instance name in the Window Border Text
      this.Text = "Hud Bar Configuration - Instance: " + (string.IsNullOrEmpty( Program.Instance ) ? "Default" : Program.Instance);
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
      m_tooltip.ReshowDelay = 100; // pop a bit faster
      m_tooltip.InitialDelay = 300; // pop a bit faster
      m_tooltip.SetToolTip( txHkShowHide, "Hotkey to Show/Hide the Bar\nDouble click to edit the Hotkey" );
      m_tooltip.SetToolTip( txHkProfile1, "Hotkey to select this Profile\nDouble click to edit the Hotkey" );
      m_tooltip.SetToolTip( txHkProfile2, "Hotkey to select this Profile\nDouble click to edit the Hotkey" );
      m_tooltip.SetToolTip( txHkProfile3, "Hotkey to select this Profile\nDouble click to edit the Hotkey" );
      m_tooltip.SetToolTip( txHkProfile4, "Hotkey to select this Profile\nDouble click to edit the Hotkey" );
      m_tooltip.SetToolTip( txHkProfile5, "Hotkey to select this Profile\nDouble click to edit the Hotkey" );
      m_tooltip.SetToolTip( txHkShelf, "Hotkey to toggle the Flight Bag\nDouble click to edit the Hotkey" );
      m_tooltip.SetToolTip( txHkCamera, "Hotkey to toggle the Camera Selector\nDouble click to edit the Hotkey" );
      m_tooltip.SetToolTip( txHkChecklistBox, "Hotkey to toggle the Checklist Box Selector\nDouble click to edit the Hotkey" );

      // indexed access for profile controls in the Form
      m_flps[0] = flp1; m_flps[1] = flp2; m_flps[2] = flp3; m_flps[3] = flp4; m_flps[4] = flp5;
      m_pName[0] = txP1; m_pName[1] = txP2; m_pName[2] = txP3; m_pName[3] = txP4; m_pName[4] = txP5;
      m_pFont[0] = cbxFontP1; m_pFont[1] = cbxFontP2; m_pFont[2] = cbxFontP3; m_pFont[3] = cbxFontP4; m_pFont[4] = cbxFontP5;
      m_pPlace[0] = cbxPlaceP1; m_pPlace[1] = cbxPlaceP2; m_pPlace[2] = cbxPlaceP3; m_pPlace[3] = cbxPlaceP4; m_pPlace[4] = cbxPlaceP5;
      m_pKind[0] = cbxKindP1; m_pKind[1] = cbxKindP2; m_pKind[2] = cbxKindP3; m_pKind[3] = cbxKindP4; m_pKind[4] = cbxKindP5;
      m_pCondensed[0] = cbxCondP1; m_pCondensed[1] = cbxCondP2; m_pCondensed[2] = cbxCondP3; m_pCondensed[3] = cbxCondP4; m_pCondensed[4] = cbxCondP5;
      m_pTransparency[0] = cbxTrans1; m_pTransparency[1] = cbxTrans2; m_pTransparency[2] = cbxTrans3; m_pTransparency[3] = cbxTrans4; m_pTransparency[4] = cbxTrans5;
      m_pHotkey[0] = txHkProfile1; m_pHotkey[1] = txHkProfile2; m_pHotkey[2] = txHkProfile3; m_pHotkey[3] = txHkProfile4; m_pHotkey[4] = txHkProfile5;

      // init combos valud for all profiles
      for (int p = 0; p < c_NumProfilesShown; p++) {
        PopulateFonts( m_pFont[p] );
        PopulatePlacement( m_pPlace[p] );
        PopulateKind( m_pKind[p] );
        PopulateCond( m_pCondensed[p] );
        PopulateTrans( m_pTransparency[p] );
      }

      PopulateASave( cbxASave ); //20211204
    }

    // LOAD is called on any invocation of the Dialog
    // Load all items from HUD to make them editable
    private void frmConfig_Load( object sender, EventArgs e )
    {
      this.TopMost = false; // inherited from parent - we don't want this here

      if (HudBarRef == null) return; // sanity ..
      if (ProfilesRef?.Count < c_NumProfilesShown) return;// sanity ..

      cbxFlightRecorder.Checked = HudBarRef.FlightRecorder;

      cbxASave.SelectedIndex = (int)HudBarRef.FltAutoSave;
      PopulateVoice( cbxVoice );// 20211006
      LoadVoice( cbxVoice );
      _speech.SetVoice( cbxVoice.SelectedItem.ToString( ) );
      _speech.Enabled = true;
      SetVoiceCalloutState( ); // 20211018


      // Hotkeys // 20211211
      m_hotkeys = HudBarRef.Hotkeys.Copy( );
      SetHotkeyTexts( );
      chkKeyboard.Checked = HudBarRef.KeyboardHook; // 20211208
      chkInGame.Checked = HudBarRef.InGameHook; // 20211208

      // make the editable copy of our profiles
      ProfilesCache.Clear( );
      foreach (var profile in ProfilesRef) {
        ProfilesCache.Add( new CProfile( profile ) );
      }

      // init with the first Section 
      InitProfileSection( );

#if DEBUG
      btDumpConfigs.Visible = true; // way to dump the configuration
#endif

      // use a Config Copy to allow Cancel changes
      FONTSdialog = new FrmFonts( );
      m_configFonts = new GUI.GUI_Fonts( HudBarRef.FontRef );

      initDone = true;
    }

    private void frmConfig_FormClosing( object sender, FormClosingEventArgs e )
    {
      // reset Sel Color
      for (int p = 0; p < c_NumProfilesShown; p++) {
        m_pName[p].BackColor = this.BackColor;
      }
      _speech.Enabled = false;
      FONTSdialog.Dispose( );
    }

    // CANCEL - leave unchanged
    private void btCancel_Click( object sender, EventArgs e )
    {
      this.DialogResult = DialogResult.Cancel;
      this.Close( );
    }

    // ACCEPT - transfer all items back to the HUD
    private void btAccept_Click( object sender, EventArgs e )
    {
      // update from edits
      // live update to HUD
      HudBarRef.SetFlightRecorder( cbxFlightRecorder.Checked );

      HudBarRef.SetHotkeys( m_hotkeys );
      HudBarRef.SetKeyboardHook( chkKeyboard.Checked );
      HudBarRef.SetInGameHook( chkInGame.Checked );

      HudBarRef.SetFltAutoSave( (FSimClientIF.FlightPlanMode)cbxASave.SelectedIndex );

      HudBarRef.SetVoiceName( cbxVoice.SelectedItem.ToString( ) );
      int idx = 0;
      foreach (var vt in HudBarRef.VoicePack.Triggers) {
        vt.Enabled = clbVoice.GetItemChecked( idx++ );
      }
      HudBarRef.VoicePack.SaveSettings( );

      // Update global fonts
      if (m_applyFontChanges) {
        HudBarRef.FontRef.FromConfigString( m_configFonts.AsConfigString( ) );
      }

      CacheProfileSection( SectionStart );
      // record profile Updates back to the Master
      for (int p = 0; p < CProfile.c_numProfiles; p++) {
        ProfilesRef[p].PName = ProfilesCache[p].PName;
        ProfilesRef[p].SetProfileString( ProfilesCache[p].ProfileString( ) );
        ProfilesRef[p].SetFlowBreakString( ProfilesCache[p].FlowBreakString( ) );
        ProfilesRef[p].SetItemPosString( ProfilesCache[p].ItemPosString( ) );
        ProfilesRef[p].SetFontSize( ProfilesCache[p].FontSize );
        ProfilesRef[p].SetPlacement( ProfilesCache[p].Placement );
        ProfilesRef[p].SetKind( ProfilesCache[p].Kind );
        ProfilesRef[p].SetCondensed( ProfilesCache[p].Condensed );
        ProfilesRef[p].SetTransparency( ProfilesCache[p].Transparency );
      }


      this.DialogResult = DialogResult.OK;
      this.Close( );
    }

    // local instance for tests
    private GUI.GUI_Speech _speech = new GUI.GUI_Speech( );

    private void cbxVoice_SelectedIndexChanged( object sender, EventArgs e )
    {
      _speech.SetVoice( cbxVoice.SelectedItem.ToString( ) );
    }
    private void cbxVoice_MouseClick( object sender, MouseEventArgs e )
    {
      if (cbxVoice.DroppedDown) return;
      _speech.SetVoice( cbxVoice.SelectedItem.ToString( ) );
      _speech.SaySynched( 100 );
    }

    private void clbVoice_SelectedIndexChanged( object sender, EventArgs e )
    {
      if (clbVoice.SelectedIndex < 0) return;
      if (!clbVoice.GetItemChecked( clbVoice.SelectedIndex )) return;
      if (!initDone) return; // don't talk at startup

      // Test when checked
      HudBarRef.VoicePack.Triggers[clbVoice.SelectedIndex].Test( _speech );
    }


    #region Context Menu

    // Buffer to maintain copied items
    private ProfileStore m_copyBuffer;

    // Copy Items is clicked
    private void ctxCopy_Click( object sender, EventArgs e )
    {
      var ctx = (sender as ToolStripItem).Owner as ContextMenuStrip;
      var col = tlp.GetColumn( ctx.SourceControl );
      // col is the profile index assuming Col 0..4 carry the profiles...
      if (col > c_NumProfilesShown) return; // sanity
      m_copyBuffer = m_flpHandler[col].GetItemsFromFlp( );
    }

    // Paste items is clicked
    private void ctxPaste_Click( object sender, EventArgs e )
    {
      var ctx = (sender as ToolStripItem).Owner as ContextMenuStrip;
      var col = tlp.GetColumn( ctx.SourceControl );
      // col is the profile index assuming Col 0..4 carry the profiles...
      if (col > c_NumProfilesShown) return; // sanity

      if (m_copyBuffer != null) {
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
      while (!(item is ContextMenuStrip)) {
        if (item is ToolStripDropDownMenu)
          item = (item as ToolStripDropDownMenu).OwnerItem;
        else if (item is ToolStripMenuItem)
          item = (item as ToolStripMenuItem).Owner;
        else
          return; // not an expected menu tree 
      }
      var ctx = item as ContextMenuStrip;
      // col is the profile index assuming Col 0..4 carry the profiles...
      var col = tlp.GetColumn( ctx.SourceControl );
      if (col > c_NumProfilesShown) return; // sanity

      var dp = DefaultProfiles.GetDefaultProfile( tsi.Text );
      if (dp != null) {
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
      while (!(item is ContextMenuStrip)) {
        if (item is ToolStripDropDownMenu)
          item = (item as ToolStripDropDownMenu).OwnerItem;
        else if (item is ToolStripMenuItem)
          item = (item as ToolStripMenuItem).Owner;
        else
          return; // not an expected menu tree 
      }
      var ctx = item as ContextMenuStrip;
      // col is the profile index assuming Col 0..4 carry the profiles...
      var col = tlp.GetColumn( ctx.SourceControl );
      if (col > c_NumProfilesShown) return; // sanity

      var dp = AcftMerges.GetAircraftProfile( tsi.Text );
      if (dp != null) {
        m_flpHandler[col].MergeProfile( dp.Profile );
        m_flpHandler[col].LoadFlp( HudBarRef );
        m_pName[col].Text = dp.Name;
      }
    }

    #endregion

    #region Hotkey Configuration

    private void chkKeyboard_CheckedChanged( object sender, EventArgs e )
    {
      txHkShowHide.Visible = chkKeyboard.Checked;
      txHkProfile1.Visible = chkKeyboard.Checked;
      txHkProfile2.Visible = chkKeyboard.Checked;
      txHkProfile3.Visible = chkKeyboard.Checked;
      txHkProfile4.Visible = chkKeyboard.Checked;
      txHkProfile5.Visible = chkKeyboard.Checked;
      txHkShelf.Visible = chkKeyboard.Checked;
      txHkCamera.Visible = chkKeyboard.Checked;
      txHkChecklistBox.Visible = chkKeyboard.Checked;
    }


    // Handle the hotkey entry for the given Key item
    private string HandleHotkey( Hotkeys hotkey )
    {
      // Setup of the Input Form
      if (m_hotkeys.ContainsKey( hotkey ))
        HKdialog.Hotkey = m_hotkeys[hotkey];
      else
        HKdialog.Hotkey = new Win.WinHotkey( ); // not set -> empty
      var old = HKdialog.Hotkey.AsString;

      HKdialog.ProfileName = $"{hotkey} Hotkey";
      if (HKdialog.ShowDialog( this ) == DialogResult.OK) {
        m_hotkeys.MaintainHotkeyString( hotkey, HKdialog.Hotkey.AsString );
        return HKdialog.Hotkey.AsString;
      }
      else {
        // cancelled
        return old; // the one we started with
      }
    }

    private void txHkProfile1_DoubleClick( object sender, EventArgs e )
    {
      txHkProfile1.Text = HandleHotkey( (Hotkeys)((int)Hotkeys.Profile_1 + c_ProfileSectionStart[m_profileSectionInUse]) );
      txHkProfile1.Select( 0, 0 );
    }

    private void txHkProfile2_DoubleClick( object sender, EventArgs e )
    {
      txHkProfile2.Text = HandleHotkey( (Hotkeys)((int)Hotkeys.Profile_2 + c_ProfileSectionStart[m_profileSectionInUse]) );
      txHkProfile2.Select( 0, 0 );
    }

    private void txHkProfile3_DoubleClick( object sender, EventArgs e )
    {
      txHkProfile3.Text = HandleHotkey( (Hotkeys)((int)Hotkeys.Profile_3 + c_ProfileSectionStart[m_profileSectionInUse]) );
      txHkProfile3.Select( 0, 0 );
    }

    private void txHkProfile4_DoubleClick( object sender, EventArgs e )
    {
      txHkProfile4.Text = HandleHotkey( (Hotkeys)((int)Hotkeys.Profile_4 + c_ProfileSectionStart[m_profileSectionInUse]) );
      txHkProfile4.Select( 0, 0 );
    }

    private void txHkProfile5_DoubleClick( object sender, EventArgs e )
    {
      txHkProfile5.Text = HandleHotkey( (Hotkeys)((int)Hotkeys.Profile_5 + c_ProfileSectionStart[m_profileSectionInUse]) );
      txHkProfile5.Select( 0, 0 );
    }

    private void txHkShowHide_DoubleClick( object sender, EventArgs e )
    {
      txHkShowHide.Text = HandleHotkey( Hotkeys.Show_Hide );
      txHkShowHide.Select( 0, 0 );
    }

    private void txHkShelf_DoubleClick( object sender, EventArgs e )
    {
      txHkShelf.Text = HandleHotkey( Hotkeys.FlightBag );
      txHkShelf.Select( 0, 0 );
    }

    private void txHkCamera_DoubleClick( object sender, EventArgs e )
    {
      txHkCamera.Text = HandleHotkey( Hotkeys.Camera );
      txHkCamera.Select( 0, 0 );
    }

    private void txHkChecklistBox_DoubleClick( object sender, EventArgs e )
    {
      txHkChecklistBox.Text = HandleHotkey( Hotkeys.ChecklistBox );
      txHkChecklistBox.Select( 0, 0 );
    }

    #endregion

    #region Dump Profile (R&D Mode..)

    // For Debug and Setup only
    private void btDumpConfigs_Click( object sender, EventArgs e )
    {
      DumpProfiles( );
    }

    /// <summary>
    /// Dump Profiles for embedding after adding items
    /// </summary>
    internal void DumpProfiles( )
    {
      using (var sw = new StreamWriter( DefaultProfiles.DefaultProfileName )) {
        sw.WriteLine( "# HEADER: 4 lines for each profile (Name, profile, order, flowbreak) All lines must be semicolon separated" );
        for (int p = 0; p < CProfile.c_numProfiles; p++) {
          // 4 lines
          sw.WriteLine( ProfilesCache[p].PName );
          sw.WriteLine( ProfilesCache[p].ProfileString( ) );
          sw.WriteLine( ProfilesCache[p].ItemPosString( ) );
          sw.WriteLine( ProfilesCache[p].FlowBreakString( ) );
        }
      }
    }

    #endregion

    private void frmConfig_VisibleChanged( object sender, EventArgs e )
    {
      ;
    }

    private void timer1_Tick( object sender, EventArgs e )
    {
      this.BringToFront( );

      ;
    }

    private void btFonts_Click( object sender, EventArgs e )
    {
      FONTSdialog.ProtoLabelRef = HudBarRef.ProtoLabelRef;
      FONTSdialog.ProtoValueRef = HudBarRef.ProtoValueRef;
      FONTSdialog.ProtoValue2Ref = HudBarRef.ProtoValue2Ref;
      FONTSdialog.Fonts?.Dispose( );
      FONTSdialog.Fonts = new GUI.GUI_Fonts( m_configFonts ); // let the Config use a clone to apply changes for preview

      if (FONTSdialog.ShowDialog( this ) == DialogResult.OK) {
        // store fonts
        m_configFonts.Dispose( );
        m_configFonts = new GUI.GUI_Fonts( FONTSdialog.Fonts ); // maintain the changes
        m_applyFontChanges = true; // if called multiple times it only stores the Accept one 
      }
    }

    private void btOtherProfileSet_Click( object sender, EventArgs e )
    {
      // store current
      CacheProfileSection( SectionStart );
      // then switch
      m_profileSectionInUse = (m_profileSectionInUse + 1) % c_ProfileSectionStart.Length; // walks through the sets
      InitProfileSection( );
    }

  }
}
