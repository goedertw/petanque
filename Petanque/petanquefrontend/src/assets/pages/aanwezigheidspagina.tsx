import { useEffect, useState } from "react";
import '../../index.css';

interface Speler {
    spelerId: number;
    voornaam: string;
    naam: string;
}

interface Speeldag {
    speeldagId: number;
    datum: string;
}

interface Aanwezigheid {
    aanwezigheidId: number;
    speeldagId: number;
    spelerId: number;
    spelerVolgnr: number;
}

function Aanwezigheidspagina() {
    const [spelers, setSpelers] = useState<Speler[]>([]);
    const [speeldagen, setSpeeldagen] = useState<Speeldag[]>([]);
    const [geselecteerdeSpeeldag, setGeselecteerdeSpeeldag] = useState<number | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [aanwezigheden, setAanwezigheden] = useState<Aanwezigheid[]>([]);

    // Functie om de aanwezigheden op te halen voor de geselecteerde speeldag
    const loadAanwezigheden = () => {
        setLoading(true);
        fetch("https://localhost:7241/api/aanwezigheden")
            .then((res) => {
                if (!res.ok) throw new Error("Fout bij ophalen aanwezigheden");
                return res.json();
            })
            .then((data: Aanwezigheid[]) => {
                setAanwezigheden(data);
                setLoading(false);
            })
            .catch((err) => {
                setError(err.message);
                setLoading(false);
            });
    };

    // Fetch spelers
    useEffect(() => {
        fetch("https://localhost:7241/api/players")
            .then((res) => {
                if (!res.ok) throw new Error("Netwerkfout bij spelers");
                return res.json();
            })
            .then((data: Speler[]) => {
                setSpelers(data);
            })
            .catch((err) => {
                setError(err.message);
            });
    }, []);

    // Fetch speeldagen
    useEffect(() => {
        fetch("https://localhost:7241/api/speeldagen")
            .then((res) => {
                if (!res.ok) throw new Error("Netwerkfout bij speeldagen");
                return res.json();
            })
            .then((data: Speeldag[]) => {
                setSpeeldagen(data);
                if (data.length > 0) {
                    const eersteSpeeldagId = data[0].speeldagId;
                    setGeselecteerdeSpeeldag(eersteSpeeldagId);
                    // Laad de aanwezigheden voor de eerste speeldag
                    loadAanwezigheden();
                }
            })
            .catch((err) => setError(err.message));
    }, []);

    // Als de geselecteerde speeldag verandert, laad de aanwezigheden opnieuw
    useEffect(() => {
        if (geselecteerdeSpeeldag) {
            loadAanwezigheden();
        }
    }, [geselecteerdeSpeeldag]); // Trigger opnieuw wanneer de speeldag verandert

    const bevestigAanwezigheid = (spelerId: number) => {
        if (!geselecteerdeSpeeldag) {
            setError("Geen speeldag gekozen");
            return;
        }

        const spelerVolgnr = 1;

        fetch("https://localhost:7241/api/aanwezigheden", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                SpeeldagId: geselecteerdeSpeeldag,
                SpelerId: spelerId,
                SpelerVolgnr: spelerVolgnr,
            }),
        })
            .then((res) => {
                if (!res.ok) throw new Error("Fout bij bevestigen");
                return res.json();
            })
            .then(() => {
                setAanwezigheden((prev) => [
                    ...prev,
                    { spelerId, speeldagId: geselecteerdeSpeeldag, spelerVolgnr: 1 },
                ]);
            })
            .catch(() => {
                setError("Fout bij bevestigen");
            });
    };

    // Hulpfunctie om te checken of de geselecteerde speeldag in het verleden ligt
    const isSpeeldagInVerleden = (speeldagDatum: string) => {
        const speeldagDate = new Date(speeldagDatum);
        const today = new Date();
        return speeldagDate < today;
    };

    // Hulpfunctie om te checken of de geselecteerde speeldag vandaag is
    const isSpeeldagVandaag = (speeldagDatum: string) => {
        const speeldagDate = new Date(speeldagDatum);
        const today = new Date();
        return (
            speeldagDate.getFullYear() === today.getFullYear() &&
            speeldagDate.getMonth() === today.getMonth() &&
            speeldagDate.getDate() === today.getDate()
        );
    };

    if (loading) return <p className="text-center mt-10">Bezig met laden...</p>;
    if (error) return <p className="text-center text-red-600 mt-10">Fout: {error}</p>;

    // Filter de aanwezigheden op basis van de geselecteerde speeldag
    const filteredAanwezigheden = aanwezigheden.filter(
        (aanwezigheid) => aanwezigheid.speeldagId === geselecteerdeSpeeldag
    );

    return (
        <div className="p-6">
            <h1 className="text-3xl font-bold text-white bg-blue-600 p-4 rounded-xl text-center mb-8 shadow-lg">
                Overzicht Aanwezigheden
            </h1>

            {/* Speeldag Dropdown */}
            <div className="mb-8 flex flex-col sm:flex-row items-center gap-4">
                <label className="text-lg font-medium">Kies een speeldag:</label>
                <select
                    className="px-4 py-2 rounded-xl border border-gray-300 shadow"
                    value={geselecteerdeSpeeldag ?? ""}
                    onChange={(e) => setGeselecteerdeSpeeldag(parseInt(e.target.value))}
                >
                    {speeldagen.map((dag) => (
                        <option key={dag.speeldagId} value={dag.speeldagId}>
                            {new Date(dag.datum).toLocaleDateString()}
                        </option>
                    ))}
                </select>
            </div>

            {/* Spelers Grid */}
            <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-6 w-full">
                {spelers.map((speler) => {
                    const spelerAanwezig = filteredAanwezigheden.find(
                        (aanwezigheid) => aanwezigheid.spelerId === speler.spelerId
                    );
                    const speeldagDatum = speeldagen.find(
                        (dag) => dag.speeldagId === geselecteerdeSpeeldag
                    )?.datum;

                    const speeldagInVerleden = speeldagDatum && isSpeeldagInVerleden(speeldagDatum);
                    const speeldagIsVandaag = speeldagDatum && isSpeeldagVandaag(speeldagDatum);

                    return (
                        <div
                            key={speler.spelerId}
                            className="bg-white rounded-2xl shadow-md p-6 flex flex-col justify-between w-full"
                        >
                            <div>
                                <h2 className="text-xl font-semibold text-gray-800 mb-1">
                                    {speler.voornaam} {speler.naam}
                                </h2>
                            </div>
                            <div className="mt-auto">
                                {speeldagIsVandaag ? (
                                    <button
                                        onClick={() => bevestigAanwezigheid(speler.spelerId)}
                                        className="bg-green-500 hover:bg-green-600 text-white px-4 py-2 rounded-xl w-full transition"
                                    >
                                        Bevestig aanwezigheid
                                    </button>
                                ) : speeldagInVerleden ? (
                                    <p className="text-sm text-gray-500">
                                        {spelerAanwezig ? "Was aanwezig" : "Was niet aanwezig"}
                                    </p>
                                ) : (
                                    <button
                                        onClick={() => bevestigAanwezigheid(speler.spelerId)}
                                        className="bg-green-500 hover:bg-green-600 text-white px-4 py-2 rounded-xl w-full transition"
                                    >
                                        Bevestig aanwezigheid
                                    </button>
                                )}
                            </div>
                        </div>
                    );
                })}
            </div>
        </div>
    );
}

export default Aanwezigheidspagina;
