using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TNet.Server;

internal class Room
{
    public string name, comment;

    public Client owner;

    public List<Client> clients = [];
}
