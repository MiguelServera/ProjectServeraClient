using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    [Serializable]
    public class GameSerializable
    {
        public string Id;
        public string Nickname;
        public string Score;
        public string Difficulty;
        public string DateStarted;
        public string DateFinished;
    }
}
