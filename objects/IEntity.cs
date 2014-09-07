using GG2server.logic.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GG2server.objects {
    public interface IEntity {
        void Serialize(UpdateType type, List<byte> buffer);
    }
}
