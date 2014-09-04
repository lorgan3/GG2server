using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GG2server.objects {
    public interface IEntity {
        void Serialize();
        void Deserialize();
    }
}
