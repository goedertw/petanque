import { useEffect, useState } from 'react';

const apiUrl = import.meta.env.VITE_API_URL;

interface Speler {
    spelerId: number;
    voornaam: string;
    naam: string;
}

function SpelerPagina() {
    const [spelers, setSpelers] = useState<Speler[]>([]);
    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);
    const [searchNaam, setSearchNaam] = useState<string>('');
    const [newVoornaam, setNewVoornaam] = useState('');
    const [newNaam, setNewNaam] = useState('');

    const [showConfirm, setShowConfirm] = useState(false);
    const [spelerToDelete, setSpelerToDelete] = useState<Speler | null>(null);
    const [aanwezigheidsCheck, setAanwezigheidsCheck] = useState<{ aanwezig: boolean; speeldagen: string[] }>({ aanwezig: false, speeldagen: [] });

    const fetchAllPlayers = () => {
        setLoading(true);
        fetch(`${apiUrl}/players`)
            .then(res => res.json())
            .then(data => {
                setSpelers(data);
                setLoading(false);
            })
            .catch(err => {
                setError('Fout bij ophalen van spelers: ' + err.message);
                setLoading(false);
            });
    };

    useEffect(() => {
        fetchAllPlayers();
    }, []);

    const handleCreatePlayer = () => {
        if (!newVoornaam || !newNaam) return;

        fetch(`${apiUrl}/players`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ voornaam: newVoornaam, naam: newNaam }),
        })
            .then(res => {
                if (!res.ok) throw new Error('Fout bij toevoegen van speler');
                return res.json();
            })
            .then(() => {
                fetchAllPlayers();
                setNewVoornaam('');
                setNewNaam('');
            })
            .catch(err => setError(err.message));
    };

    const confirmDeletePlayer = (speler: Speler) => {
        // Eerst aanwezigheid checken
        fetch(`${apiUrl}/aanwezigheden/check-speler-aanwezig?id=${speler.spelerId}`)
            .then(res => {
                if (!res.ok) throw new Error('Fout bij controleren van aanwezigheid');
                return res.json();
            })
            .then(data => {
                setAanwezigheidsCheck({ aanwezig: data.aanwezig, speeldagen: data.speeldagen });
                setShowConfirm(true);
                setSpelerToDelete(speler);
            })
            .catch(err => setError('Fout bij controleren van aanwezigheid: ' + err.message));
    };

    const handleDeleteConfirmed = () => {
        if (!spelerToDelete) return;

        fetch(`${apiUrl}/players/${spelerToDelete.spelerId}`, {
            method: 'DELETE',
        })
            .then(res => {
                if (!res.ok) throw new Error('Fout bij verwijderen van speler');
                fetchAllPlayers();
                setShowConfirm(false);
                setSpelerToDelete(null);
            })
            .catch(err => setError('Fout bij verwijderen: ' + err.message));
    };

    const handleCancelDelete = () => {
        setShowConfirm(false);
        setSpelerToDelete(null);
        setAanwezigheidsCheck({ aanwezig: false, speeldagen: [] });
    };

    const filteredSpelers = spelers.filter((speler) => {
        const searchTerm = searchNaam.toLowerCase();
        return (
            speler.voornaam.toLowerCase().includes(searchTerm) ||
            speler.naam.toLowerCase().includes(searchTerm)
        );
    });

    return (
        <div className="min-h-screen flex justify-center items-start bg-[#f7f7f7]">
            <div className="max-w-3xl w-full p-0">
                <div className="max-w-3xl mx-auto p-0">
                    <h2 className="text-3xl font-bold text-white bg-[#3c444c] p-2 rounded-2xl shadow mb-6 text-center">
                        Leden
                    </h2>

                    {/* Voeg speler toe */}
                    <div className="mb-8 bg-white p-4 rounded-xl shadow">
                        <h2 className="text-xl font-semibold text-[#44444c] mb-4">Nieuw Lid Toevoegen</h2>
                        <div className="flex flex-col sm:flex-row gap-2 mb-2">
                            <input
                                type="text"
                                placeholder="Voornaam"
                                value={newVoornaam}
                                onChange={(e) => setNewVoornaam(e.target.value)}
                                className="border border-[#74747c] p-2 rounded w-full"
                            />
                            <input
                                type="text"
                                placeholder="Naam"
                                value={newNaam}
                                onChange={(e) => setNewNaam(e.target.value)}
                                className="border border-[#74747c] p-2 rounded w-full"
                            />
                        </div>
                        <button
                            onClick={handleCreatePlayer}
                            className="bg-[#ccac4c] hover:bg-[#b8953d] text-white px-4 py-2 rounded shadow cursor-pointer"
                        >
                            Voeg toe
                        </button>
                    </div>

                    {/* Zoek op naam */}
                    <div className="mb-6 flex flex-col sm:flex-row gap-2 sm:items-center">
                        <input
                            type="text"
                            placeholder="Zoek lid op naam"
                            value={searchNaam}
                            onChange={(e) => setSearchNaam(e.target.value)}
                            className="border border-[#74747c] p-2 rounded w-full sm:w-auto"
                        />
                    </div>

                    {/* Loading & Error */}
                    {loading && <p className="text-[#ccac4c]">Bezig met laden...</p>}
                    {error && <p className="text-red-600 font-semibold mb-4">Error: {error}</p>}

                    {/* Spelers lijst */}
                    <ul className="space-y-4 max-h-96 overflow-y-auto">
                        {filteredSpelers.length === 0 ? (
                            <p className="text-center text-gray-500">Geen spelers gevonden</p>
                        ) : (
                            filteredSpelers.map(speler => (
                                <li
                                    key={speler.spelerId}
                                    className="p-4 bg-[#fbd46d] rounded-lg shadow flex justify-between items-center"
                                >
                                    <p className="font-semibold text-[#44444c]">
                                        {speler.voornaam} {speler.naam}
                                    </p>
                                    <button
                                        onClick={() => confirmDeletePlayer(speler)}
                                        className="text-red-600 hover:text-red-800 text-2xl cursor-pointer"
                                        title="Verwijder lid"
                                    >
                                        ❌
                                    </button>
                                </li>
                            ))
                        )}
                    </ul>

                    {/* Confirm Modal */}
                    {showConfirm && spelerToDelete && (
                        <div className="fixed inset-0 bg-black bg-opacity-50 flex justify-center items-center z-50">
                            <div className="bg-white p-6 rounded-xl shadow-lg max-w-sm w-full text-center">
                                {aanwezigheidsCheck.aanwezig ? (
                                    <>
                                        <h2 className="text-xl font-bold mb-4 text-[#44444c]">
                                            Kan <span className="text-red-600">{spelerToDelete.voornaam} {spelerToDelete.naam}</span> niet verwijderen.<br />
                                            De speler staat nog aanwezig op:
                                        </h2>
                                        <ul className="text-[#44444c] mb-4">
                                            {aanwezigheidsCheck.speeldagen.map((dag, index) => {
                                                const date = new Date(dag);
                                                const formattedDate = date.toLocaleDateString('nl-BE', {
                                                    day: 'numeric',
                                                    month: 'long',
                                                    year: 'numeric',
                                                });

                                                return <li key={index}>• {formattedDate}</li>;
                                            })}
                                        </ul>

                                        <button
                                            onClick={handleCancelDelete}
                                            className="bg-gray-400 hover:bg-gray-500 text-white px-4 py-2 rounded shadow cursor-pointer"
                                        >
                                            Sluiten
                                        </button>
                                    </>
                                ) : (
                                    <>
                                        <h2 className="text-xl font-bold mb-4 text-[#44444c]">
                                            Weet je zeker dat je <br />
                                            <span className="text-red-600">{spelerToDelete.voornaam} {spelerToDelete.naam}</span> wilt verwijderen?
                                        </h2>
                                        <div className="flex justify-center gap-4 mt-6">
                                            <button
                                                onClick={handleDeleteConfirmed}
                                                className="bg-red-600 hover:bg-red-700 text-white px-4 py-2 rounded shadow cursor-pointer"
                                            >
                                                Ja, verwijder
                                            </button>
                                            <button
                                                onClick={handleCancelDelete}
                                                className="bg-gray-400 hover:bg-gray-500 text-white px-4 py-2 rounded shadow cursor-pointer"
                                            >
                                                Nee
                                            </button>
                                        </div>
                                    </>
                                )}
                            </div>
                        </div>
                    )}
                </div>
            </div>
        </div>
    );
}

export default SpelerPagina;
