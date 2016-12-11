using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallongeCSharpDriver.Main
{
    public struct ParticipantID
    {
        public ParticipantID(int id, int gid)
        {
            ID = id;
            GroupID = gid;
        }
        public int ID { get; }
        public int GroupID { get; }
    }
}
