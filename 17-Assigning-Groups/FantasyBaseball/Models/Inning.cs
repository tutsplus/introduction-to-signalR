using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FantasyBaseball.Models
{
    public enum HalfInning {
        Top,
        Bottom
    }

    public class Inning {
        private int _inning;
        private HalfInning _halfInning;

        public Inning( ) {
            _halfInning = HalfInning.Top;
            _inning = 1;
        }

        public HalfInning HalfInning {
            get { return _halfInning; }
        }

        public void Advance( ) {
            if(_halfInning == HalfInning.Top)
                _halfInning = HalfInning.Bottom;
            else {
                _halfInning = HalfInning.Top;
                _inning++;
            }
        }

        public override string ToString( ) {
            var inningString = string.Empty;
            var lastInningDigit = _inning%10;

            switch ( lastInningDigit ) {
                case 1:
                    if ( _inning == 11 )
                        inningString = _inning + "th";
                    else
                        inningString = _inning + "st";
                    break;
                case 2:
                    inningString = _inning + "nd";
                    break;
                case 3:
                    inningString = _inning + "rd";
                    break;
                default:
                    inningString = _inning + "th";
                    break;
            }

            return string.Format( "{0} of the {1}", _halfInning.ToString( ), inningString );
        }
    }
}