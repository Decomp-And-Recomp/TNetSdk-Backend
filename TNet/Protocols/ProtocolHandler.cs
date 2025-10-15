using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TNet.Protocols;

internal abstract class ProtocolHandler
{
    public abstract void Handle(Client client, UnPacker unPacker);
}
