using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Serializable]
    public class MySlefGeneratePicker<game, match> where game : class, new() where match : class, new()
    {
        public MySlefGeneratePicker() { BallGames = new Dictionary<int, DataTable>(); }
        public match Match { get; set; }
        public Dictionary<int, DataTable> BallGames { get; set; }
    }
}
