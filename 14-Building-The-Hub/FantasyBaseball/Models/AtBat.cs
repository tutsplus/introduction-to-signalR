using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FantasyBaseball.Models
{
    public class AtBat {
        private bool _isComplete;
        private int _strikes;
        private int _balls;
        private string _currentEvent;
        private Batter _batter;
        private Pitcher _pitcher;

        public AtBat( Batter batter, Pitcher pitcher ) {
            _isComplete = false;
            _strikes = 0;
            _balls = 0;
            _currentEvent = "Batting";
            _batter = batter;
            _pitcher = pitcher;
        }

        public Batter Batter {
            get { return _batter; }
            set { _batter = value; }
        }

        public Pitcher Pitcher {
            get { return _pitcher; }
            set { _pitcher = value; }
        }

        public int Balls {
            get { return _balls; }
            set {
                _balls = value;
                if ( _balls == 4 ) {
                    _currentEvent = "Walk";
                    _isComplete = true;
                }
            }
        }

        public int Strikes {
            get { return _strikes; }
            set {
                _strikes = value;
                if ( _strikes == 3 ) {
                    _currentEvent = "Strike Out";
                    _isComplete = true;
                }
            }
        }

        public string CurrentEvent {
            get { return _currentEvent; }
        }

        public bool IsComplete {
            get { return _isComplete; }
        }

        public void Hit( string type, string location ) {
            _currentEvent = string.Format( "{0} hit a {1} to {2}", _batter.Name, type, location );
            _isComplete = true;
        }

        public void Out( string type, string location ) {
            _currentEvent = string.Format("{0} hit a {1} out to {2}", _batter.Name, type, location);
            _isComplete = true;
        }

        public void Reset( ) {
            _strikes = 0;
            _balls = 0;
            _currentEvent = "Batting";
            _isComplete = false;
        }
    }
}