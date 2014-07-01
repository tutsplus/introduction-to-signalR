using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FantasyBaseball.Models
{
    public abstract class Player
    {
        public string Name { get; set; }
        public string Team { get; set; }
        public int Number { get; set; }
        public string Position { get; set; }
    }
}