using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperSocket.Common
{
    public delegate void VEventHandler<TEventArgs>(TEventArgs e);
    public delegate void VEventHandler();
    public delegate void BEventHandler<TEventArgs>(TEventArgs e);
    public delegate void BEventHandler();
}
