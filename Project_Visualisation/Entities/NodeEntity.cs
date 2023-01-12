using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Visualisation.Entities
{
    public class NodeEntity : PowerEntity
    {
        public NodeEntity()
        {

        }

        public NodeEntity(long id, string name)
        {
            Id = id;
            Name = name;
        }

        public override string ToString()
        {
            return $"Node:\nID: {Id}\nName: {Name}\nConnections: {Connections}";
        }
    }
}
