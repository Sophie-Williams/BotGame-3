using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Botcraft
{
    class MobAction
    {
        public MobCmd cmd;
        public string directionString;
        public int repeat;


        public MobAction() : this(MobCmd.Idle){ }
        public MobAction(MobCmd _cmd) : this(_cmd, "") { }
        public MobAction(MobCmd _cmd, string _dirStr) : this(_cmd, _dirStr, 1) { }
        public MobAction(MobCmd _cmd, string _dirStr, int _repeat)
        {            
            cmd = _cmd;
            directionString = _dirStr;
            repeat = _repeat;
        }
    }
}
