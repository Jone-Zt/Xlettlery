using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Serializable]
    public class MySlefGeneratePicker<game, match> where game : class,new()where match : class,new()
    {
        public MySlefGeneratePicker() { BallGames = new Dictionary<int,List<game>>(); }
        public match Match { get; set; }
        public Dictionary<int,List<game>> BallGames { get; set; }
    }
}
