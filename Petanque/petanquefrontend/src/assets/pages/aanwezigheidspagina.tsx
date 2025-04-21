import { useEffect, useState } from "react";
import '../../index.css';
const apiUrl = import.meta.env.VITE_API_URL;

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

    const loadAanwezigheden = () => {
        setLoading(true);
        fetch(`${apiUrl}/aanwezigheden`)
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

    useEffect(() => {
        fetch(`${apiUrl}/players`)
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

    useEffect(() => {
        fetch(`${apiUrl}/speeldagen`)
            .then((res) => {
                if (!res.ok) throw new Error("Netwerkfout bij speeldagen");
                return res.json();
            })
            .then((data: Speeldag[]) => {
                setSpeeldagen(data);
                if (data.length > 0) {
                    const eersteSpeeldagId = data[0].speeldagId;
                    setGeselecteerdeSpeeldag(eersteSpeeldagId);
                    loadAanwezigheden();
                }
            })
            .catch((err) => setError(err.message));
    }, []);

    useEffect(() => {
        if (geselecteerdeSpeeldag) {
            loadAanwezigheden();
        }
    }, [geselecteerdeSpeeldag]);

    const bevestigAanwezigheid = (spelerId: number) => {
        if (!geselecteerdeSpeeldag) {
            setError("Geen speeldag gekozen");
            return;
        }

        const spelerAanwezig = aanwezigheden.find(
            (aanwezigheid) => aanwezigheid.spelerId === spelerId && aanwezigheid.speeldagId === geselecteerdeSpeeldag
        );

        if (spelerAanwezig) return;

        const spelerVolgnr =
            aanwezigheden.filter((a) => a.speeldagId === geselecteerdeSpeeldag).length + 1;

        const nieuweAanwezigheid: Omit<Aanwezigheid, "aanwezigheidId"> = {
            speeldagId: geselecteerdeSpeeldag,
            spelerId,
            spelerVolgnr,
        };

        fetch(`${apiUrl}/aanwezigheden`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify(nieuweAanwezigheid),
        })
            .then((res) => {
                if (!res.ok) throw new Error("Fout bij versturen van aanwezigheid");
                return res.json();
            })
            .then((data: Aanwezigheid) => {
                setAanwezigheden((prev) => [...prev, data]);
            })
            .catch((err) => {
                setError(err.message);
            });
    };

    const isSpeeldagInVerleden = (speeldagDatum: string) => {
        const speeldagDate = new Date(speeldagDatum);
        const today = new Date();
        return speeldagDate < today;
    };

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

    const filteredAanwezigheden = aanwezigheden.filter(
        (aanwezigheid) => aanwezigheid.speeldagId === geselecteerdeSpeeldag
    );

    return (
        <div className="p-6">
            <h1 className="text-3xl font-bold text-white bg-[#3c444c] p-4 rounded-2xl shadow mb-6 text-center">
                Overzicht Aanwezigheden
            </h1>

            <div className="mb-8 flex flex-col sm:flex-row items-center gap-4">
                <label className="text-lg font-medium text-[#44444c]">Kies een speeldag:</label>
                <select
                    className="px-4 py-2 rounded-xl border border-[#74747c] shadow text-[#44444c]"
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
                                <h2 className="text-xl font-semibold text-[#44444c] mb-1">
                                    {speler.voornaam} {speler.naam}
                                </h2>
                            </div>
                            <div className="mt-auto">
                                {speeldagIsVandaag ? (
                                    !spelerAanwezig ? (
                                        <button
                                            onClick={() => bevestigAanwezigheid(speler.spelerId)}
                                            className="bg-[#ccac4c] hover:bg-[#b8953d] text-white font-bold px-4 py-2 rounded-xl w-full transition cursor-pointer"
                                        >
                                            Bevestig aanwezigheid
                                        </button>
                                    ) : (
                                        <p className="bg-[#fbd46d] text-[#44444c] font-semibold px-2 py-1 rounded-full inline-block">
                                            Aanwezig
                                        </p>

                                    )
                                ) : speeldagInVerleden ? (
                                    spelerAanwezig ? (
                                        <p className="text-sm font-medium text-[#ccac4c]">Was aanwezig</p>
                                    ) : (
                                        <p className="text-sm font-medium text-[#74747c]">Was niet aanwezig</p>
                                    )
                                ) : (
                                    <button
                                        onClick={() => bevestigAanwezigheid(speler.spelerId)}
                                        className="bg-[#ccac4c] hover:bg-[#b8953d] text-white font-bold px-4 py-2 rounded-xl w-full transition cursor-pointer"
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
