﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS20_HudBar.GUI
{
  internal class ClickedEventArgs : EventArgs
  {
    public GItem Item { get; } // readonly
    public ClickedEventArgs( GItem item ) { Item = item; }
  }

}