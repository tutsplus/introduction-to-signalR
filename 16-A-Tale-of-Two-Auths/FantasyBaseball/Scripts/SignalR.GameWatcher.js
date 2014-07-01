/// <reference path="jquery-2.1.0.js" />
/// <reference path="jquery.signalR-2.0.3.js" />

/*!
    ASP.NET SignalR Game Watcher
*/

// Crockford's supplant method (poor man's templating)
if (!String.prototype.supplant) {
    String.prototype.supplant = function (o) {
        return this.replace(/{([^{}]*)}/g,
            function (a, b) {
                var r = o[b];
                return typeof r === 'string' || typeof r === 'number' ? r : a;
            }
        );
    };
}

$(function () {
    var gamesWatcher = $.connection.games,
        $gamesSection = $('#games'),
	    $gameTemplate = '<div id="{Identifier}"><div class="row"><div class="col-lg-12"><h2 id="game-name">Game: </h2></div></div><div class="row"><div class="col-md-4"><h3>Batter</h3><p>Name: <span id="batter-name"></span> <br />Number: <span id="batter-number"></span><br />Position: <span id="batter-position"></span><br />Bats: <span id="batter-bats"></span></p></div><div class="col-md-4"><h3>AtBat</h3><p>Balls: <span id="atbat-balls"></span><br />Strikes: <span id="atbat-strikes"></span><br />Current Event: <span id="atbat-event"></span><br /><br /><span id="inning"></span></p></div><div class="col-md-4"><h3>Pitcher</h3><p>Name: <span id="pitcher-name"></span><br />Number: <span id="pitcher-number"></span><br />Throws: <span id="pitcher-throws"></span></p></div></div></div>';

    function init() {
        return gamesWatcher.server.getAllGames().done(function (games) {
            $gamesSection.empty();
            $.each(games, function () {
                $gamesSection.append($gameTemplate.supplant(this));
            });
        });
    }

    // Add client-side hub methods that the server will call
    $.extend(gamesWatcher.client, {
        updateGames: function (games) {
            $.each(games, updateGame(this));
        },
        updateGame: function (game) {
            $("#" + game.Identifier + " #game-name").text(game.Name);
            $("#" + game.Identifier + " #inning").text(game.Inning + " : " + game.Outs + " out(s)");

            $("#" + game.Identifier + " #atbat-balls").text(game.CurrentAtBat.Balls);
            $("#" + game.Identifier + " #atbat-strikes").text(game.CurrentAtBat.Strikes);
            $("#" + game.Identifier + " #atbat-event").text(game.CurrentAtBat.CurrentEvent);

            $("#" + game.Identifier + " #batter-name").text(game.CurrentAtBat.Batter.Name);
            $("#" + game.Identifier + " #batter-number").text(game.CurrentAtBat.Batter.Number);
            $("#" + game.Identifier + " #batter-position").text(game.CurrentAtBat.Batter.Position);
            $("#" + game.Identifier + " #batter-bats").text(game.CurrentAtBat.Batter.Bats);

            $("#" + game.Identifier + " #pitcher-name").text(game.CurrentAtBat.Pitcher.Name);
            $("#" + game.Identifier + " #pitcher-number").text(game.CurrentAtBat.Pitcher.Number);
            $("#" + game.Identifier + " #pitcher-throws").text(game.CurrentAtBat.Pitcher.Throws);
        },
        gamesRunning: function () {
            $("#run").prop("disabled", true);
            $("#pause").prop("disabled", false);
            $("#reset").prop("disabled", true);
        },
        gamesPaused: function () {
            $("#run").prop("disabled", false);
            $("#pause").prop("disabled", true);
            $("#reset").prop("disabled", false);
        },
        gamesReset: function () {
            init();
        }
    });

    // Start the connection
    $.connection.hub.start()
        .then(init)
        .then(function () {
            return gamesWatcher.server.getGamesState();
        })
        .done(function (state) {
            if (state === 'Running') {
                gamesWatcher.client.gamesRunning();
            } else {
                gamesWatcher.client.gamesPaused();
            }

            gamesWatcher.server.runGames();

            // Wire up the buttons
            $("#run").click(function () {
                gamesWatcher.server.runGames();
            });

            $("#pause").click(function () {
                gamesWatcher.server.pauseGames();
            });

            $("#reset").click(function () {
                gamesWatcher.server.reset();
            });
        });
});