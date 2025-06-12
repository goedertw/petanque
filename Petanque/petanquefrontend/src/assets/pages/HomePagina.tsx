import Kalender from "../Components/Kalender.tsx";
import { useState, useEffect } from "react";

const apiUrl = import.meta.env.VITE_API_URL;

interface Speeldag {
    speeldagId: number;
    datum: string;
}

interface Seizoen {
    seizoensId: number;
    startdatum: string;
    einddatum: string;
}

function HomePagina() {
    const [speeldagen, setSpeeldagen] = useState<Speeldag[]>([]);
    const [selectedSpeeldag, setSelectedSpeeldag] = useState<Speeldag | null>(null);
    const [seizoenen, setSeizoenen] = useState<Seizoen[]>([]);

    const [showCalendar, setShowCalendar] = useState(false);

    const [showCreateSpeeldagDialog, setShowCreateSpeeldagDialog] = useState(false);
    const [pendingDate, setPendingDate] = useState<Date | null>(null);

    const [showCreateSeizoenForm, setShowCreateSeizoenForm] = useState(false);
    const [seizoenStart, setSeizoenStart] = useState<string>("");
    const [seizoenEind, setSeizoenEind] = useState<string>("");

    useEffect(() => {
        const fetchSpeeldagen = async () => {
            try {
                const response = await fetch(`${apiUrl}/speeldagen`);
                const data = await response.json();
                setSpeeldagen(data);
            } catch (error) {
                console.error("Fout bij ophalen van speeldagen:", error);
            }
        };

        const fetchSeizoenen = async () => {
            try {
                const response = await fetch(`${apiUrl}/seizoenen`);
                const data = await response.json();
                setSeizoenen(data);
            } catch (error) {
                console.error("Fout bij ophalen van seizoenen:", error);
            }
        };

        fetchSpeeldagen();
        fetchSeizoenen();
    }, []);

    const handleSelectSpeeldag = (speeldag: Speeldag) => {
        setSelectedSpeeldag(speeldag);
    };

    const handleToggleCalendar = () => {
        setShowCalendar(!showCalendar);
    };

    const handleClickOnNewDate = (date: Date) => {
        setPendingDate(date);
        setShowCreateSpeeldagDialog(true);
    };

    const handleCreateSpeeldag = async () => {
        if (!pendingDate) return;

        try {
            const passendeSeizoen = seizoenen.find((s) => {
                const start = new Date(s.startdatum);
                const eind = new Date(s.einddatum);
                return pendingDate >= start && pendingDate <= eind;
            });

            if (!passendeSeizoen) {
                alert("Geen passend seizoen gevonden voor deze datum. Maak eerst een seizoen aan.");
                return;
            }

            const year = pendingDate.getFullYear();
            const month = (pendingDate.getMonth() + 1).toString().padStart(2, '0');
            const day = pendingDate.getDate().toString().padStart(2, '0');

            const request = {
                datum: `${year}-${month}-${day}`,
                seizoensId: passendeSeizoen.seizoensId
            };

            const response = await fetch(`${apiUrl}/speeldagen`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(request)
            });

            if (!response.ok) throw new Error("Fout bij aanmaken van speeldag");

            const createdSpeeldag = await response.json();

            setSpeeldagen((prev) => [...prev, createdSpeeldag]);
            setSelectedSpeeldag(createdSpeeldag);

            setShowCreateSpeeldagDialog(false);
            setPendingDate(null);
        } catch (error) {
            console.error("Fout bij aanmaken van speeldag:", error);
            alert("Kon speeldag niet aanmaken.");
        }
    };

    const handleCancelCreateSpeeldag = () => {
        setShowCreateSpeeldagDialog(false);
        setPendingDate(null);
    };

    const formatDateDutch = (date: Date) => {
        return date.toLocaleDateString("nl-NL", {
            weekday: "long",
            year: "numeric",
            month: "long",
            day: "numeric"
        });
    };

    const handleCreateSeizoen = async () => {
        if (!seizoenStart || !seizoenEind) {
            alert("Gelieve beide datums in te vullen.");
            return;
        }

        try {
            const request = {
                startdatum: seizoenStart,
                einddatum: seizoenEind
            };

            const response = await fetch(`${apiUrl}/seizoenen`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(request)
            });

            if (!response.ok) throw new Error("Fout bij aanmaken van seizoen");

            const createdSeizoen = await response.json();

            setSeizoenen((prev) => [...prev, createdSeizoen]);
            setShowCreateSeizoenForm(false);
            setSeizoenStart("");
            setSeizoenEind("");

            alert("Seizoen succesvol aangemaakt!");
        } catch (error) {
            console.error("Fout bij aanmaken van seizoen:", error);
            alert("Kon seizoen niet aanmaken.");
        }
    };

    return (
        <div className="min-h-screen bg-gray-100 flex flex-col items-center pt-16 text-gray-800 px-4">
            <div className="max-w-xl w-full text-center bg-white border border-[#fbd46d] shadow-lg rounded-2xl p-10">
                <h1 className="text-4xl font-bold text-[#3c444c] mb-6">
                    Welkom bij de <span className="text-[#fbd46d]">VL@S Petanque</span> App
                </h1>
                <p className="text-lg leading-relaxed">
                    Gebruik het menu hierboven om spellen te starten, scores in te voeren, klassementen te bekijken, aanwezigheden op te nemen en meer.
                    <br/>Hieronder kan je een nieuwe seizoen en speeldagen aanmaken.
                </p>
            </div>
            <h2 className="text-center text-2xl font-bold text-[#3c444c] mt-8 mb-2">
                Seizoen toevoegen:
            </h2>
            <button
                onClick={() => setShowCreateSeizoenForm(!showCreateSeizoenForm)}
                className="mt-6 mb-4 bg-[#ccac4c] bg-[#ccac4c] hover:bg-[#b8953d] text-white font-bold px-6 py-3 rounded-xl transition cursor-pointer"
            >
                {showCreateSeizoenForm ? "Verberg Seizoen Toevoegen" : "Voeg Seizoen Toe"}
            </button>

            {showCreateSeizoenForm && (
                <div className="bg-white p-6 rounded shadow-md max-w-sm w-full mb-6">
                    <h2 className="text-xl font-semibold mb-4">Nieuw Seizoen</h2>

                    <div className="mb-4">
                        <label className="block text-left mb-2">Startdatum:</label>
                        <div className="relative">
                            <input
                                type="date"
                                value={seizoenStart}
                                onChange={(e) => setSeizoenStart(e.target.value)}
                                className="border p-2 rounded w-full appearance-none pr-10"
                                id="startdate-input"
                            />
                            <button
                                type="button"
                                onClick={() => {
                                    const input = document.getElementById('startdate-input') as HTMLInputElement;
                                    input?.showPicker?.();
                                    input?.focus();
                                }}
                                className="absolute inset-y-0 right-0 flex items-center pr-3 text-gray-400 cursor-pointer"
                            >
                                📅
                            </button>
                        </div>
                    </div>

                    <div className="mb-4">
                        <label className="block text-left mb-2">Einddatum:</label>
                        <div className="relative">
                            <input
                                type="date"
                                value={seizoenEind}
                                onChange={(e) => setSeizoenEind(e.target.value)}
                                className="border p-2 rounded w-full appearance-none pr-10"
                                id="enddate-input"
                            />
                            <button
                                type="button"
                                onClick={() => {
                                    const input = document.getElementById('enddate-input') as HTMLInputElement;
                                    input?.showPicker?.();
                                    input?.focus();
                                }}
                                className="absolute inset-y-0 right-0 flex items-center pr-3 text-gray-400 cursor-pointer"
                            >
                                📅
                            </button>
                        </div>
                    </div>

                    <button
                        onClick={handleCreateSeizoen}
                        className="bg-[#ccac4c] hover:bg-[#b8953d] text-white font-bold py-2 px-4 rounded w-full"
                    >
                        Seizoen toevoegen
                    </button>
                </div>
            )}

            <h2 className="text-center text-2xl font-bold text-[#3c444c] mt-8 mb-2">
                Speeldagen toevoegen / selecteren:
            </h2>

            <Kalender
                speeldagen={speeldagen}
                selectedSpeeldag={selectedSpeeldag}
                onSelectSpeeldag={handleSelectSpeeldag}
                showCalendar={showCalendar}
                onToggleCalendar={handleToggleCalendar}
                onClickOnNewDate={handleClickOnNewDate}
            />

            {showCreateSpeeldagDialog && pendingDate && (
                <div className="fixed inset-0 flex items-center justify-center bg-black bg-opacity-50 z-50">
                    <div className="bg-white p-6 rounded shadow-md max-w-sm w-full text-center">
                        <h2 className="text-xl font-semibold mb-4">Speeldag toevoegen</h2>
                        <p className="mb-4">
                            Wil je <span className="font-bold">{formatDateDutch(pendingDate)}</span> als speeldag instellen?
                        </p>
                        <div className="flex justify-center gap-4">
                            <button
                                onClick={handleCreateSpeeldag}
                                className="bg-[#ccac4c] hover:bg-[#b8953d] text-white font-bold py-2 px-4 rounded"
                            >
                                Ja
                            </button>
                            <button
                                onClick={handleCancelCreateSpeeldag}
                                className="bg-gray-300 hover:bg-gray-400 text-gray-800 font-bold py-2 px-4 rounded"
                            >
                                Nee
                            </button>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
}

export default HomePagina;
