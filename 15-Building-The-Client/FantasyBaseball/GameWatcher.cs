using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Threading;
using System.Collections.Concurrent;
using FantasyBaseball.Models;

namespace FantasyBaseball
{
    public class GameWatcher
    {
        private readonly static Lazy<GameWatcher> _instance = new Lazy<GameWatcher>(() => new GameWatcher(GlobalHost.ConnectionManager.GetHubContext<GameHub>().Clients));

        private readonly object _gameStateLock = new object();
        private readonly object _updateGamesLock = new object();

        private readonly TimeSpan _updateInterval = TimeSpan.FromMilliseconds(250);
        private readonly Random _updateOrNotRandom = new Random();

        private readonly ConcurrentDictionary<string, Game> _games = new ConcurrentDictionary<string, Game>();

        private Timer _timer;
        private volatile bool _updatingGames;
        private volatile GamesState _gamesState;

        private string[] _positions = new string[] { "pitcher", "catcher", "first base", "second base", "thrid base", "shortstop", "left field", "center field", "right field" };
        private string[] _hitTypes = new string[] { "single", "double", "triple", "homerun" };
        private string[] _outTypes = new string[] { "ground", "pop", "fly" };

        private GameWatcher(IHubConnectionContext clients)
        {
            Clients = clients;
            LoadDefaultGames();
            StartWatcher();
        }

        public static GameWatcher Instance
        {
            get { return _instance.Value; }
        }

        private IHubConnectionContext Clients { get; set; }

        public GamesState GamesState
        {
            get { return _gamesState; }
            private set { _gamesState = value; }
        }

        public IEnumerable<Game> GetAllGames()
        {
            return _games.Values;
        }

        public void StartWatcher()
        {
            lock (_gameStateLock)
            {
                if (GamesState != GamesState.Running)
                {
                    _timer = new Timer(UpdateAtBats, null, _updateInterval, _updateInterval);
                    GamesState = GamesState.Running;
                    BroadcastGamesStateChange(GamesState.Running);
                }
            }
        }

        public void PauseWatcher()
        {
            lock (_gameStateLock)
            {
                if (GamesState == GamesState.Running)
                {
                    if (_timer != null)
                        _timer.Dispose();

                    GamesState = GamesState.Paused;
                    BroadcastGamesStateChange(GamesState.Paused);
                }
            }
        }

        public void Reset()
        {
            lock (_gameStateLock)
            {
                if (GamesState != GamesState.Paused)
                {
                    throw new InvalidOperationException("Games must be paused before running a reset.");
                }

                LoadDefaultGames();
                BroadcastGamesReset();
            }
        }

        public void LoadDefaultGames()
        {
            _games.Clear();

            var games = new List<Game>
            {
                new Game("cubsvspirates", "Cubs vs Pirates", "Chicago Cubs", "Atlanta Braves"),
                new Game("bravesvsyankees", "Mets vs Yankees", "New York Mets", "New York Yankees")
            };

            games.ForEach(game => _games.TryAdd(game.Name, game));
        }

        private void UpdateAtBats(object state)
        {
            lock (_updateGamesLock)
            {
                if (!_updatingGames)
                {
                    _updatingGames = true;

                    foreach (var game in _games.Values)
                    {
                        if (TryUpdateGame(game))
                        {
                            BroadcastGame(game);
                        }
                    }

                    _updatingGames = false;
                }
            }
        }

        private bool TryUpdateGame(Game game)
        {
            var r = _updateOrNotRandom.NextDouble();
            if (r > 0.1)
                return false;

            if (game.CurrentAtBat.IsComplete)
            {
                game.CurrentAtBat.Reset();
                return true;
            }

            var random = new Random();
            var eventValue = random.NextDouble();

            if ( eventValue > 0.66 )
                game.Ball( );
            else if (eventValue > 0.33)
                game.Strike();
            else
                PerformAction(game);

            return true;
        }

        private void BroadcastGamesStateChange(GamesState state)
        {
            switch (state)
            {
                case GamesState.Paused:
                    Clients.All.gamesPaused();
                    break;
                case GamesState.Running:
                    Clients.All.gamesRunning();
                    break;
                default:
                    break;
            }
        }

        private void BroadcastGame(Game game)
        {
            Clients.All.updateGame(game);
        }

        private void BroadcastGamesReset()
        {
            Clients.All.gamesReset();
        }

        private void PerformAction(Game game)
        {
            var random = new Random();
            var isHit = random.NextDouble() > 0.51;

            if (isHit)
            {
                var typePos = random.Next(0, _hitTypes.Length);
                var startPoint = 0;
                if (typePos == 3)
                    startPoint = 6;
                var locationPos = random.Next(startPoint, _positions.Length);

                game.Hit(_hitTypes[typePos], _positions[locationPos]);
            }
            else
            {
                var typePos = random.Next(0, _outTypes.Length);
                var startPos = 0;
                var endPos = _positions.Length;
                if (typePos == 0 || typePos == 1)
                {
                    endPos = 6;
                }
                else
                {
                    startPos = 6;
                }
                var locationPos = random.Next(startPos, endPos);

                game.Out(_outTypes[typePos], _positions[locationPos]);
            }
        }
    }

    public enum GamesState
    {
        Running,
        Paused
    }
}