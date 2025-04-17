import React, { useState, useEffect } from "react";

const createEmptyTeam = () => ({
    players: ["", ""],
    points: 0,
});

const createEmptyGame = () => ({
    teamA: createEmptyTeam(),
    teamB: createEmptyTeam(),
});

const MatchScoreCard = () => {
    const [games, setGames] = useState([createEmptyGame(), createEmptyGame(), createEmptyGame()]);
    const [terrein, setTerrein] = useState(1);
    const [speeldagen, setSpeeldagen] = useState([]);
    const [selectedSpeeldag, setSelectedSpeeldag] = useState("");

    useEffect(() => {
        // Fetch speeldagen from backend
        fetch("https://localhost:7241/api/speeldagen")
            .then((res) => res.json())
            .then((data) => {
                setSpeeldagen(data);
                if (data.length > 0) {
                    setSelectedSpeeldag(data[0].datum);
                }
            })
            .catch((err) => console.error("Fout bij laden van speeldagen:", err));
    }, []);

    const handleNameChange = (gameIndex, teamKey, playerIndex, value) => {
        const updatedGames = [...games];
        updatedGames[gameIndex][teamKey].players[playerIndex] = value;
        setGames(updatedGames);
    };

    const handlePointsChange = (gameIndex, teamKey, value) => {
        const updatedGames = [...games];
        const parsedValue = parseInt(value);
        updatedGames[gameIndex][teamKey].points = isNaN(parsedValue) ? 0 : Math.max(0, parsedValue);
        setGames(updatedGames);
    };

    return (
        <div className="p-6 max-w-6xl mx-auto text-sm">
            <div className="mb-4">
                <h1 className="text-xl font-bold text-center">VL@S - Wervik/Geluwe</h1>
                <div className="flex justify-between mt-2 text-sm items-center">
                    <span className="flex items-center gap-2">
                        <span className="bg-orange-200 px-2 py-1 rounded">TERREIN</span>
                        <input
                            type="number"
                            value={terrein}
                            min={1}
                            onChange={(e) => setTerrein(Math.max(1, parseInt(e.target.value) || 1))}
                            placeholder="Nr"
                            className="border rounded px-2 py-1 w-16"
                        />
                    </span>
                    <span className="flex items-center gap-2">
                        <label className="text-sm font-medium">Speeldag:</label>
                        <select
                            value={selectedSpeeldag}
                            onChange={(e) => setSelectedSpeeldag(e.target.value)}
                            className="border rounded px-2 py-1"
                        >
                            {speeldagen.map((dag) => (
                                <option key={dag.speeldagId} value={dag.datum}>
                                    {new Date(dag.datum).toLocaleDateString()}
                                </option>
                            ))}
                        </select>
                    </span>
                </div>
            </div>

            {games.map((game, gameIndex) => (
                <div key={gameIndex} className="border rounded-xl p-4 mb-6 bg-white shadow space-y-4">
                    <h2 className="text-lg font-semibold bg-yellow-200 inline-block px-3 py-1 rounded">SPEL {gameIndex + 1}</h2>
                    <div className="grid grid-cols-2 gap-4">
                        {["teamA", "teamB"].map((teamKey, idx) => {
                            const team = game[teamKey];
                            const score = team.points;
                            const otherScore = game[teamKey === "teamA" ? "teamB" : "teamA"].points;
                            const difference = score - otherScore;

                            return (
                                <div key={teamKey}>
                                    <div className="space-y-1">
                                        {team.players.map((name, i) => (
                                            <input
                                                key={i}
                                                type="text"
                                                value={name}
                                                onChange={(e) => handleNameChange(gameIndex, teamKey, i, e.target.value)}
                                                placeholder={`Speler ${i + 1}`}
                                                className="border-b w-full px-2 py-1 mb-1"
                                            />
                                        ))}
                                    </div>

                                    <div className="mt-2">
                                        <label className="block text-sm mb-1">Aantal punten:</label>
                                        <input
                                            type="number"
                                            value={team.points}
                                            min={0}
                                            onChange={(e) => handlePointsChange(gameIndex, teamKey, e.target.value)}
                                            className="border px-2 py-1 w-24"
                                        />
                                    </div>

                                    <div className="mt-2 text-sm font-medium">
                                        Aantal (X): <span className="font-bold">{score}</span>
                                        <span className="ml-4">{difference >= 0 ? "+" : ""}{difference}</span>
                                    </div>
                                </div>
                            );
                        })}
                    </div>
                </div>
            ))}
        </div>
    );
};

export default MatchScoreCard;
