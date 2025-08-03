import { useEffect, useState } from "react";
import Kalender from "../Components/Kalender.tsx"; // ← importeer jouw eigen component!
const apiUrl = import.meta.env.VITE_API_URL;

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
    spelerScores: any[];
}

interface PlayerResponseContract extends Speler {}

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



const formatDateToDutch = (dateString: string): string => {
    const date = new Date(dateString);
    return ` speeldag: ${date.getDate()} ${date.toLocaleDateString("nl-NL", { month: "long" })}`;
};

// Component
function Scorebladpagina() {
    const [gamesPerTerrein, setGamesPerTerrein] = useState<Record<string, Game[]>>({});
    const [speeldagen, setSpeeldagen] = useState<Speeldag[]>([]);
    const [selectedSpeeldag, setSelectedSpeeldag] = useState<Speeldag | null>(null);
    const [showCalendar, setShowCalendar] = useState(false);

    // Fetch speeldagen
    useEffect(() => {
        fetch(`${apiUrl}/speeldagen`)
            .then((res) => res.json())
            .then((data: Speeldag[]) => {
                setSpeeldagen(data);

                if (data.length > 0) {
                    const opgeslagenSpeeldagId = localStorage.getItem("speeldagId");
                    let gekozenSpeeldagId = opgeslagenSpeeldagId ? parseInt(opgeslagenSpeeldagId) : data[0].speeldagId;

                    const speeldagBestaat = data.some((dag) => dag.speeldagId === gekozenSpeeldagId);

                    if (!speeldagBestaat) {
                        gekozenSpeeldagId = data[0].speeldagId;
                    }

                    const gekozenSpeeldag = data.find((dag) => dag.speeldagId === gekozenSpeeldagId) || data[0];
                    setSelectedSpeeldag(gekozenSpeeldag);
                }
            })
            .catch((err) => console.error("Fout bij laden van speeldagen:", err));
    }, []);

    // Fetch spelverdelingen per speeldag
    useEffect(() => {
        if (!selectedSpeeldag) return;

        fetch(`${apiUrl}/spelverdelingen/${selectedSpeeldag.speeldagId}`)
            .then((res) => res.json())
            .then((data: SpelverdelingResponseContract[]) => {
                const terreinMap: Record<string, Record<number, Game>> = {};

                data.forEach((entry) => {
                    const terreinLabel = entry.spel.terrein.trim();
                    const spelId = entry.spel.spelId;
                    const teamKey = entry.team === "Team A" ? "teamA" : "teamB";
                    const spelerNaam = `${entry.spelerVolgnr}. ${entry.speler.voornaam} ${entry.speler.naam}`;

                    if (!terreinMap[terreinLabel]) {
                        terreinMap[terreinLabel] = {};
                    }

                    if (!terreinMap[terreinLabel][spelId]) {
                        terreinMap[terreinLabel][spelId] = {
                            spelId,
                            terrein: terreinLabel,
                            teamA: createEmptyTeam(),
                            teamB: createEmptyTeam(),
                        };
                    }

                    terreinMap[terreinLabel][spelId][teamKey].players.push(spelerNaam);
                });

                const restoredGames: Record<string, Game[]> = {};

                for (const terrein in terreinMap) {
                    const key = `punten_${selectedSpeeldag.speeldagId}_terrein_${terrein}`;
                    const opgeslagen = localStorage.getItem(key);
                    const games = Object.values(terreinMap[terrein]);

                    if (opgeslagen) {
                        const parsed = JSON.parse(opgeslagen) as Game[];
                        restoredGames[terrein] = games.map((game) => {
                            const match = parsed.find((g) => g.spelId === game.spelId);
                            return match
                                ? {
                                    ...game,
                                    teamA: { ...game.teamA, points: match.teamA.points },
                                    teamB: { ...game.teamB, points: match.teamB.points },
                                }
                                : game;
                        });
                    } else {
                        restoredGames[terrein] = games;
                    }
                }

                setGamesPerTerrein(restoredGames);
            })
            .catch((err) => console.error("Fout bij laden van spelverdeling:", err));
    }, [selectedSpeeldag]);

    const handlePointsChange = (terrein: string, gameIndex: number, teamKey: "teamA" | "teamB", value: string) => {
        const updated = { ...gamesPerTerrein };
        const parsedValue = parseInt(value);
        updated[terrein][gameIndex][teamKey].points = isNaN(parsedValue) ? 0 : Math.max(0, parsedValue);
        setGamesPerTerrein(updated);

        if (selectedSpeeldag) {
            const key = `punten_${selectedSpeeldag.speeldagId}_terrein_${terrein}`;
            localStorage.setItem(key, JSON.stringify(updated[terrein]));
        }
    };

    const handleSave = async () => {
        if (!selectedSpeeldag) return;

        const allGames = Object.entries(gamesPerTerrein).flatMap(([_, games]) => games);
        const invalidGames = allGames.filter(
            (game) => game.teamA.points !== 13 && game.teamB.points !== 13
        );

        if (invalidGames.length > 0) {
            alert("Fout: Bij elk spel moet één team exact 13 punten hebben.");
            return;
        }

        try {
            await Promise.all(
                allGames.map((game) =>
                    fetch(`${apiUrl}/scores/${game.spelId}`, {
                        method: "PUT",
                        headers: { "Content-Type": "application/json" },
                        body: JSON.stringify({
                            speeldagId: selectedSpeeldag.speeldagId,
                            terrein: game.terrein,
                            scoreA: game.teamA.points,
                            scoreB: game.teamB.points,
                        }),
                    }).then((res) => {
                        if (!res.ok) throw new Error("Fout bij opslaan van score");
                    })
                )
            );

            // 🔁 Na scores succesvol opgeslagen → klassement genereren
            const klassementResp = await fetch(`${apiUrl}/dagklassementen/${selectedSpeeldag.speeldagId}`, {
                method: "POST",
            });

            if (!klassementResp.ok) {
                throw new Error("Fout bij genereren dagklassement");
            }

            alert("Scores én dagklassement succesvol opgeslagen!");
        } catch (error) {
            console.error("Fout bij opslaan of klassement:", error);
            alert("Er ging iets mis bij het opslaan of genereren van het klassement.");
        }
    };


    return (
        <div className="max-w-6xl">
            <div className="mb-0">
                <h2 className="text-3xl font-bold text-white bg-[#3c444c] p-2 rounded-2xl shadow mb-6 text-center">
                    Scores
                </h2>

                <Kalender
                    speeldagen={speeldagen}
                    selectedSpeeldag={selectedSpeeldag}
                    onSelectSpeeldag={(speeldag) => {
                        setSelectedSpeeldag(speeldag);
                        localStorage.setItem("speeldagId", speeldag.speeldagId.toString());
                        setShowCalendar(false);
                    }}
                    showCalendar={showCalendar}
                    onToggleCalendar={() => setShowCalendar(!showCalendar)}
                />
            </div>

            {Object.entries(gamesPerTerrein).map(([terrein, games]) => (
                <div key={terrein} className="mb-0">
                    {games.map((game, gameIndex) => (
                        <div key={gameIndex} className="border rounded-xl p-3 mb-3 bg-white">
                            <h3 className="text-md font-semibold bg-[#fbd46d] text-[#3c444c] inline-block px-3 py-1 rounded-full">
                                {terrein.toUpperCase()}&nbsp;&nbsp;-&nbsp;&nbsp;SPEL {gameIndex + 1}
                            </h3>
                            <div className="flex justify-between items-center space-x-6">
                                <div className="w-full flex flex-col items-start">
                                    <h4 className="font-medium text-lg text-left text-[#3c444c] mt-2"><b><u>Team A</u></b></h4>
                                    <div className="space-y-0 w-full">
                                        {game.teamA.players.map((name, i) => (
                                            <p key={i} className="w-full m-0">
                                                {name}
                                            </p>
                                        ))}
                                    </div>
                                    <div className="mt-3">
                                        <label className="text-sm mb-0 text-[#44444c]">Aantal punten: </label>
                                        <input
                                            type="number"
                                            value={game.teamA.points}
                                            min={0}
                                            max={13}
                                            onChange={(e) => handlePointsChange(terrein, gameIndex, "teamA", e.target.value)}
                                            className="border px-2 py-1 w-24 rounded-lg border-[#74747c]"
                                        />
                                        <span className="ml-4 text-sm text-[#44444c]">(
                                            {game.teamA.points - game.teamB.points >= 0 ? "+" : ""}
                                            {game.teamA.points - game.teamB.points})
                                        </span>
                                    </div>
                                </div>

                                <div className="w-1 bg-gray-500 h-30 my-auto"></div>

                                <div className="w-full flex flex-col items-start">
                                    <h4 className="font-medium text-lg text-left text-[#3c444c]"><b><u>Team B</u></b></h4>
                                    <div className="space-y-0 w-full">
                                        {game.teamB.players.map((name, i) => (
                                            <p key={i} className="w-full m-0">
                                                {name}
                                            </p>
                                        ))}
                                    </div>
                                    <div className="mt-4">
                                        <label className="text-sm mb-1 text-[#44444c]">Aantal punten: </label>
                                        <input
                                            type="number"
                                            value={game.teamB.points}
                                            min={0}
                                            max={13}
                                            onChange={(e) => handlePointsChange(terrein, gameIndex, "teamB", e.target.value)}
                                            className="border px-2 py-1 w-24 rounded-lg border-[#74747c]"
                                        />
                                        <span className="ml-4 text-sm text-[#44444c]">(
                                            {game.teamB.points - game.teamA.points >= 0 ? "+" : ""}
                                            {game.teamB.points - game.teamA.points})
                                        </span>
                                    </div>
                                </div>
                            </div>
                        </div>
                    ))}
                </div>
            ))}

            <button
                onClick={handleSave}
                className="bg-[#3c444c] text-white px-4 py-2 rounded hover:bg-[#2f373f] transition cursor-pointer mt-0"
            >
                Opslaan
            </button>
        </div>
    );
}

export default Scorebladpagina;
