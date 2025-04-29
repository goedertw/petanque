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
    const [searchNaam, setSearchNaam] = useState<string>('');  // State for search by name
    const [newVoornaam, setNewVoornaam] = useState('');
    const [newNaam, setNewNaam] = useState('');

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

    const filteredSpelers = spelers.filter((speler) => {
        const searchTerm = searchNaam.toLowerCase();
        return (
            speler.voornaam.toLowerCase().includes(searchTerm) ||
            speler.naam.toLowerCase().includes(searchTerm)
        );
    });

    return (
        <div className="min-h-screen flex justify-center items-start bg-[#f7f7f7]">
            <div className="max-w-3xl w-full p-6">
                <div className="max-w-3xl mx-auto p-6">
                    <h1 className="text-3xl font-bold text-white bg-[#3c444c] p-4 rounded-2xl shadow mb-6 text-center">
                        Leden
                    </h1>

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

                    {/* Spelers lijst (scrollable) */}
                    <ul className="space-y-4 max-h-96 overflow-y-auto">
                        {filteredSpelers.length === 0 ? (
                            <p className="text-center text-gray-500">Geen spelers gevonden</p>
                        ) : (
                            filteredSpelers.map(speler => (
                                <li key={speler.spelerId} className="p-4 bg-[#fbd46d] rounded-lg shadow">
                                    <p><span className="font-semibold text-[#44444c]">{speler.voornaam} {speler.naam}</span></p>
                                </li>
                            ))
                        )}
                    </ul>
                </div>
            </div>
        </div>
    );
}

export default SpelerPagina;
