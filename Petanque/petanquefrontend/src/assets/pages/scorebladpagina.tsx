import { useEffect, useState } from "react";
import Calendar from "react-calendar";
const apiUrl = import.meta.env.VITE_API_URL;
import 'react-calendar/dist/Calendar.css';
// Interfaces
interface Speler {
    spelerId: number;
    voornaam: string;
    naam: string;
}

interface SpelResponseContract {
    spelId: number;
    speeldagId: number;
    terrein: string;
    spelerScores: any[]; // Wordt niet gebruikt
}

interface PlayerResponseContract extends Speler { }

interface SpelverdelingResponseContract {
    spelverdelingsId: number;
    spelId: number;
    team: string;
    spelerPositie: string;
    spelerVolgnr: number;
    speler: PlayerResponseContract;
    spel: SpelResponseContract;
}

interface Speeldag {
    speeldagId: number;
    datum: string;
}

interface Team {
    players: string[];
    points: number;
}

interface Game {
    spelId: number;
    terrein: string;
    teamA: Team;
    teamB: Team;
}

// Helpers
const createEmptyTeam = (): Team => ({
    players: [],
    points: 0,
});

const createEmptyGame = (): Game => ({
    spelId: 0,
    terrein: "",
    teamA: createEmptyTeam(),
    teamB: createEmptyTeam(),
});

// MatchScoreCard Component
function MatchScoreCard() {
    const [games, setGames] = useState<Game[]>([createEmptyGame(), createEmptyGame(), createEmptyGame()]);
    const [terrein, setTerrein] = useState<number>(() => {
        const opgeslagenTerrein = localStorage.getItem('selectedTerrein');
        return opgeslagenTerrein ? parseInt(opgeslagenTerrein) : 1;
    });
    const [speeldagen, setSpeeldagen] = useState<Speeldag[]>([]);
    const [selectedSpeeldag, setSelectedSpeeldag] = useState<number | null>(null);

    // Fetch speeldagen
    useEffect(() => {
        fetch(`${apiUrl}/speeldagen`)
            .then((res) => res.json())
            .then((data: Speeldag[]) => {
                setSpeeldagen(data);

                if (data.length > 0) {
                    const opgeslagenSpeeldagId = localStorage.getItem("selectedSpeeldag");
                    let gekozenSpeeldagId = opgeslagenSpeeldagId ? parseInt(opgeslagenSpeeldagId) : data[0].speeldagId;

                    const speeldagBestaat = data.some((dag) => dag.speeldagId === gekozenSpeeldagId);

                    if (!speeldagBestaat) {
                        gekozenSpeeldagId = data[0].speeldagId;
                    }

                    setSelectedSpeeldag(gekozenSpeeldagId);
                }
            })
            .catch((err) => console.error("Fout bij laden van speeldagen:", err));
    }, []);

    // Fetch spelverdelingen voor het geselecteerde terrein
    useEffect(() => {
        if (selectedSpeeldag === null) return;

        fetch(`${apiUrl}/spelverdelingen/${selectedSpeeldag}`)
            .then((res) => res.json())
            .then((data: SpelverdelingResponseContract[]) => {
                const spelMap: Record<number, Game> = {};
                data.forEach((entry) => {
                    const terreinLabel = entry.spel.terrein;
                    const spelId = entry.spel.spelId;
                    const teamKey = entry.team === "Team A" ? "teamA" : "teamB";
                    const spelerNaam = `${entry.speler.voornaam} ${entry.speler.naam}`;

                    if (terreinLabel.trim().toLowerCase() === `terrein ${terrein}`.toLowerCase()) {
                        if (!spelMap[spelId]) {
                            spelMap[spelId] = {
                                spelId,
                                terrein: terreinLabel,
                                teamA: createEmptyTeam(),
                                teamB: createEmptyTeam(),
                            };
                        }

                        spelMap[spelId][teamKey].players.push(spelerNaam);
                    }
                });

                const mappedGames = Object.values(spelMap);
                setGames(mappedGames);
            })
            .catch((err) => console.error("Fout bij laden van spelverdeling:", err));
    }, [selectedSpeeldag, terrein]);

    // Handlers
    const handleNameChange = (gameIndex: number, teamKey: "teamA" | "teamB", playerIndex: number, value: string) => {
        const updatedGames = [...games];
        updatedGames[gameIndex][teamKey].players[playerIndex] = value;
        setGames(updatedGames);
    };

    const handlePointsChange = (gameIndex: number, teamKey: "teamA" | "teamB", value: string) => {
        const updatedGames = [...games];
        const parsedValue = parseInt(value);
        updatedGames[gameIndex][teamKey].points = isNaN(parsedValue) ? 0 : Math.max(0, parsedValue);
        setGames(updatedGames);
    };

    const handleSave = async () => {
        if (!selectedSpeeldag) return;

        // Validate that one of the teams has a score of 13
        const invalidGames = games.filter(
            (game) => game.teamA.points !== 13 && game.teamB.points !== 13
        );

        if (invalidGames.length > 0) {
            alert("Fout: Beide teams hebben geen score van 13. Scores kunnen niet worden opgeslagen.");
            return;
        }

        try {
            await Promise.all(
                games.map((game) =>
                    fetch(`${apiUrl}/scores/${game.spelId}`, {
                        method: "PUT",
                        headers: { "Content-Type": "application/json" },
                        body: JSON.stringify({
                            speeldagId: selectedSpeeldag,
                            terrein: game.terrein,
                            scoreA: game.teamA.points,
                            scoreB: game.teamB.points,
                        }),
                    }).then((res) => {
                        if (!res.ok) throw new Error("Fout bij opslaan van score");
                    })
                )
            );
            alert("Scores succesvol ge pdatet!");
        } catch (error) {
            console.error("Fout bij opslaan:", error);
            alert("Er ging iets mis bij het opslaan.");
        }
    };

    return (
        <div className="p-6 max-w-6xl mx-auto text-sm">
            <div className="mb-4">
                <h1 className="text-xl font-bold text-center text-[#f7f7f7] bg-[#3c444c] p-4 rounded-2xl shadow-lg">
                    VL@S - Scores
                </h1>
                <div className="flex justify-between mt-2 text-sm items-center">
                    <span className="flex items-center gap-2">
                        <span className="bg-[#fbd46d] px-2 py-1 rounded">TERREIN</span>
                        <input
                            type="number"
                            value={terrein}
                            min={1}
                            onChange={(e) => {
                                const value = Math.max(1, parseInt(e.target.value) || 1);
                                setTerrein(value);
                                localStorage.setItem('selectedTerrein', value.toString());
                            }}
                            placeholder="Nr"
                            className="border border-[#74747c] p-2 rounded w-16"
                        />
                    </span>
                    <span className="flex items-center gap-2">
                        <label className="text-sm font-medium text-[#44444c]">Selecteer een speeldag:</label>
                        <Calendar
                            onClickDay={(value) => {
                                const clickedDate = new Date(value);
                                const matchingSpeeldag = speeldagen.find((dag) => {
                                    const dagDate = new Date(dag.datum);
                                    return (
                                        dagDate.getFullYear() === clickedDate.getFullYear() &&
                                        dagDate.getMonth() === clickedDate.getMonth() &&
                                        dagDate.getDate() === clickedDate.getDate()
                                    );
                                });

                                if (matchingSpeeldag) {
                                    setSelectedSpeeldag(matchingSpeeldag.speeldagId);
                                }
                            }}
                            value={
                                (() => {
                                    const speeldag = speeldagen.find((dag) => dag.speeldagId === selectedSpeeldag);
                                    return speeldag ? new Date(speeldag.datum) : null;
                                })()
                            }
                            tileContent={({ date, view }) => {
                                if (view === 'month') {
                                    const match = speeldagen.find((dag) => {
                                        const dagDate = new Date(dag.datum);
                                        return (
                                            dagDate.getFullYear() === date.getFullYear() &&
                                            dagDate.getMonth() === date.getMonth() &&
                                            dagDate.getDate() === date.getDate()
                                        );
                                    });

                                    return match ? (
                                        <div className="flex justify-center items-center mt-1">
                                            <div className="h-2 w-2 rounded-full bg-[#ccac4c]"></div>
                                        </div>
                                    ) : null;
                                }
                            }}
                            className="p-4 bg-white rounded-2xl shadow-md text-[#44444c]"
                            // calendarType="ISO"
                            tileClassName={({ date, view }) => {
                                if (view === 'month') {
                                    const speeldag = speeldagen.find((dag) => {
                                        const dagDate = new Date(dag.datum);
                                        return (
                                            dagDate.getFullYear() === date.getFullYear() &&
                                            dagDate.getMonth() === date.getMonth() &&
                                            dagDate.getDate() === date.getDate()
                                        );
                                    });

                                    if (speeldag && speeldag.speeldagId === selectedSpeeldag) {
                                        return 'bg-[#ccac4c] text-white rounded-full'; // ✨ Highlight
                                    }
                                }
                                return null;
                            }}
                        />
                    </span>
                </div>
            </div>

            {games.map((game, gameIndex) => (
                <div key={gameIndex} className="border rounded-xl p-6 mb-6 bg-white shadow-lg space-y-4">
                    <h2 className="text-lg font-semibold bg-[#ccac4c] text-white inline-block px-3 py-1 rounded-full">
                        SPEL {gameIndex + 1}
                    </h2>
                    <div className="flex justify-between items-center space-x-6">
                        <div className="w-full flex flex-col items-start">
                            <h3 className="font-medium text-lg text-left text-[#3c444c]">Team A</h3>
                            <div className="space-y-2 w-full">
                                {game.teamA.players.map((name, i) => (
                                    <p key={i} className="border-b w-full px-2 py-1 mb-2 rounded-lg border-[#74747c]">
                                        {name}
                                    </p>
                                ))}
                            </div>
                            <div className="mt-4">
                                <label className="block text-sm mb-1 text-[#44444c]">Aantal punten:</label>
                                <input
                                    type="number"
                                    value={game.teamA.points}
                                    min={0}
                                    max={13}
                                    onChange={(e) => handlePointsChange(gameIndex, "teamA", e.target.value)}
                                    className="border px-2 py-1 w-24 rounded-lg border-[#74747c]"
                                />
                                <div className="mt-2 text-sm font-medium text-[#44444c]">
                                    Aantal: <span className="font-bold">{game.teamA.points}</span>
                                    <span className="ml-4">
                                        {game.teamA.points - game.teamB.points >= 0 ? "+" : ""}
                                        {game.teamA.points - game.teamB.points}
                                    </span>
                                </div>
                            </div>
                        </div>

                        <div className="w-1 bg-gray-300 h-48 my-auto"></div>

                        <div className="w-full flex flex-col items-end">
                            <h3 className="font-medium text-lg text-right text-[#3c444c]">Team B</h3>
                            <div className="space-y-2 w-full">
                                {game.teamB.players.map((name, i) => (
                                    <p key={i} className="border-b w-full px-2 py-1 mb-2 rounded-lg border-[#74747c]">
                                        {name}
                                    </p>
                                ))}
                            </div>

                            <div className="mt-4">
                                <label className="block text-sm mb-1 text-[#44444c]">Aantal punten:</label>
                                <input
                                    type="number"
                                    value={game.teamB.points}
                                    min={0}
                                    max={13}
                                    onChange={(e) => handlePointsChange(gameIndex, "teamB", e.target.value)}
                                    className="border px-2 py-1 w-24 rounded-lg border-[#74747c]"
                                />
                                <div className="mt-2 text-sm font-medium text-[#44444c]">
                                    Aantal: <span className="font-bold">{game.teamB.points}</span>
                                    <span className="ml-4">
                                        {game.teamB.points - game.teamA.points >= 0 ? "+" : ""}
                                        {game.teamB.points - game.teamA.points}
                                    </span>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            ))}

            <button
                onClick={handleSave}
                className="bg-[#3c444c] text-white px-4 py-2 rounded hover:bg-[#2f373f] transition cursor-pointer mt-6"
            >
                Opslaan
            </button>
        </div>
    );
}

export default MatchScoreCard;
