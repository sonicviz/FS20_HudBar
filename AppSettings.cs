﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS20_HudBar
{
  sealed class AppSettings : ApplicationSettingsBase
  {

    // Singleton
    private static readonly Lazy<AppSettings> m_lazy = new Lazy<AppSettings>( () => new AppSettings( ) );
    public static AppSettings Instance { get => m_lazy.Value; }

    private AppSettings( )
    {
      if ( this.FirstRun ) {
        // migrate the settings to the new version if the app runs the first time
        try {
          this.Upgrade( );
        }
        catch { }
        this.FirstRun = false;
        this.Save( );
      }
    }

    #region Setting Properties

    // manages Upgrade
    [UserScopedSetting( )]
    [DefaultSettingValue( "True" )]
    public bool FirstRun {
      get { return (bool)this["FirstRun"]; }
      set { this["FirstRun"] = value; }
    }


    // Control bound settings
    [UserScopedSetting( )]
    [DefaultSettingValue( "10, 10" )]
    public Point FormLocation {
      get { return (Point)this["FormLocation"]; }
      set { this["FormLocation"] = value; }
    }

    // User Config Settings

    [UserScopedSetting( )]
    [DefaultSettingValue( "False" )]
    public bool ShowUnits {
      get { return (bool)this["ShowUnits"]; }
      set { this["ShowUnits"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "False" )]
    public bool Opaque {
      get { return (bool)this["Opaque"]; }
      set { this["Opaque"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "0" )]
    public int FontSize {
      get { return (int)this["FontSize"]; }
      set { this["FontSize"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "0" )]
    public int SelProfile {
      get { return (int)this["SelProfile"]; }
      set { this["SelProfile"] = value; }
    }


    [UserScopedSetting( )]
    [DefaultSettingValue( "Profile 1" )]
    public string Profile_1_Name {
      get { return (string)this["Profile_1_Name"]; }
      set { this["Profile_1_Name"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "" )]
    public string Profile_1 {
      get { return (string)this["Profile_1"]; }
      set { this["Profile_1"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "Profile 2" )]
    public string Profile_2_Name {
      get { return (string)this["Profile_2_Name"]; }
      set { this["Profile_2_Name"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "" )]
    public string Profile_2 {
      get { return (string)this["Profile_2"]; }
      set { this["Profile_2"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "Profile 3" )]
    public string Profile_3_Name {
      get { return (string)this["Profile_3_Name"]; }
      set { this["Profile_3_Name"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "" )]
    public string Profile_3 {
      get { return (string)this["Profile_3"]; }
      set { this["Profile_3"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "Profile 4" )]
    public string Profile_4_Name {
      get { return (string)this["Profile_4_Name"]; }
      set { this["Profile_4_Name"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "" )]
    public string Profile_4 {
      get { return (string)this["Profile_4"]; }
      set { this["Profile_4"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "Profile 5" )]
    public string Profile_5_Name {
      get { return (string)this["Profile_5_Name"]; }
      set { this["Profile_5_Name"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "" )]
    public string Profile_5 {
      get { return (string)this["Profile_5"]; }
      set { this["Profile_5"] = value; }
    }

    #endregion


  }
}