using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FantasyBaseball.Models
{
    public class Game {
        private List<Batter> _homeBatters;
        private List<Batter> _awayBatters;
        private List<Pitcher> _homePitchers;
        private List<Pitcher> _awayPitchers;

        private string _name;
        private Inning _inning;
        private int _outs;
        private int _nextHomeBatter;
        private int _nextAwayBatter;
        private string _identifier;
        private AtBat _currentAtBat;

        public Game( string identifier, string name, string homeTeam, string awayTeam ) {
            _homeBatters = new List<Batter>( );
            _awayBatters = new List<Batter>( );
            _homePitchers = new List<Pitcher>();
            _awayPitchers = new List<Pitcher>();
            _name = name;
            _identifier = identifier;
            _nextHomeBatter = 0;
            _nextAwayBatter = 1;
            _currentAtBat = new AtBat( _awayBatters[0], _homePitchers[0] );

            _homeBatters = GetHomeBatters( homeTeam );
            _homePitchers = GetHomePitchers( homeTeam );
            _awayBatters = GetAwayBatters( awayTeam );
            _awayPitchers = GetAwayPitchers( awayTeam );
        }

        public string Name {
            get { return _name; }
        }

        public string Inning {
            get { return _inning.ToString( ); }
        }

        public int Outs {
            get { return _outs; }
        }

        public string Identifier {
            get { return _identifier; }
        }

        public AtBat CurrentAtBat {
            get { return _currentAtBat; }
            set { _currentAtBat = value; }
        }

        public void AdvanceInning( ) {
            _inning.Advance();
            if ( _inning.HalfInning == HalfInning.Top )
                CurrentAtBat.Pitcher = _homePitchers[0];
            else {
                CurrentAtBat.Pitcher = _awayPitchers[0];
            }

            AdvanceBatter( );
            _outs = 0;
        }

        public void Hit( string type, string location ) {
            CurrentAtBat.Hit(type, location);
            AdvanceBatter( );
        }

        public void Out( string type, string location ) {
            CurrentAtBat.Out( type, location );
            AdvanceBatter( );
        }

        public void Strike( ) {
            CurrentAtBat.Strikes++;
            if(CurrentAtBat.Strikes == 3)
                if(++_outs == 3)
                    AdvanceInning();
                else {
                    AdvanceBatter( );
                }
        }

        public void Ball( ) {
            CurrentAtBat.Balls++;
            if ( CurrentAtBat.Balls == 4 )
                AdvanceBatter( );
        }

        private void AdvanceBatter( ) {
            if ( _inning.HalfInning == HalfInning.Top ) {
                var batter = _nextAwayBatter >= _awayBatters.Count ? 0 : _nextAwayBatter;
                CurrentAtBat.Batter = _awayBatters[batter];
                _nextAwayBatter++;
            } else {
                var batter = _nextHomeBatter >= _homeBatters.Count ? 0 : _nextHomeBatter;
                CurrentAtBat.Batter = _homeBatters[batter];
                _nextHomeBatter++;
            }
        }

        private List<Batter> GetHomeBatters( string teamName ) {
            return new List<Batter> {
                new Batter{Name = "Home Batter 1", Number = 1, Position = "P", Team = teamName, Bats = "R"},
                new Batter{Name = "Home Batter 2", Number = 2, Position = "C", Team = teamName, Bats = "L"},
                new Batter{Name = "Home Batter 3", Number = 3, Position = "1B", Team = teamName, Bats = "R"},
                new Batter{Name = "Home Batter 4", Number = 4, Position = "2B", Team = teamName, Bats = "R"},
                new Batter{Name = "Home Batter 5", Number = 5, Position = "3B", Team = teamName, Bats = "R"},
                new Batter{Name = "Home Batter 6", Number = 6, Position = "SS", Team = teamName, Bats = "L"},
                new Batter{Name = "Home Batter 7", Number = 7, Position = "LF", Team = teamName, Bats = "L"},
                new Batter{Name = "Home Batter 8", Number = 8, Position = "CF", Team = teamName, Bats = "R"},
                new Batter{Name = "Home Batter 9", Number = 9, Position = "RF", Team = teamName, Bats = "R"},
            };
        }

        private List<Batter> GetAwayBatters( string teamName ) {
            return new List<Batter> {
                new Batter{Name = "Away Batter 1", Number = 1, Position = "P", Team = teamName, Bats = "R"},
                new Batter{Name = "Away Batter 2", Number = 2, Position = "C", Team = teamName, Bats = "L"},
                new Batter{Name = "Away Batter 3", Number = 3, Position = "1B", Team = teamName, Bats = "R"},
                new Batter{Name = "Away Batter 4", Number = 4, Position = "2B", Team = teamName, Bats = "R"},
                new Batter{Name = "Away Batter 5", Number = 5, Position = "3B", Team = teamName, Bats = "R"},
                new Batter{Name = "Away Batter 6", Number = 6, Position = "SS", Team = teamName, Bats = "L"},
                new Batter{Name = "Away Batter 7", Number = 7, Position = "LF", Team = teamName, Bats = "L"},
                new Batter{Name = "Away Batter 8", Number = 8, Position = "CF", Team = teamName, Bats = "R"},
                new Batter{Name = "Away Batter 9", Number = 9, Position = "RF", Team = teamName, Bats = "R"},
            };
        }

        private List<Pitcher> GetHomePitchers( string teamName ) {
            return new List<Pitcher> {
                new Pitcher{Name = "Home Pitcher 1", Number = 25, Position = "P", Team = teamName, Throws = "L"}
            };
        }

        private List<Pitcher> GetAwayPitchers( string teamName ) {
            return new List<Pitcher> {
                new Pitcher{Name = "Away Pitcher 1", Number = 17, Position = "P", Team = teamName, Throws = "R"}
            };
        } 
    }
}