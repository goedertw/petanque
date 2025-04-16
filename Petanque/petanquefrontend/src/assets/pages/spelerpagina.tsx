import { useEffect, useState } from 'react';

interface Speler {
    spelerId: number;
    voornaam: string;
    naam: string;
}

function SpelerPagina() {
    const [spelers, setSpelers] = useState<Speler[]>([]);
    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);
    const [searchId, setSearchId] = useState<string>('');
    const [newVoornaam, setNewVoornaam] = useState('');
    const [newNaam, setNewNaam] = useState('');

    const fetchAllPlayers = () => {
        setLoading(true);
        fetch('https://localhost:7241/api/players')
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

    const handleSearch = () => {
        if (!searchId) return;
        setLoading(true);
        fetch(`https://localhost:7241/api/players/${searchId}`)
            .then(res => {
                if (!res.ok) throw new Error('Speler niet gevonden');
                return res.json();
            })
            .then(data => {
                setSpelers([data]);
                setLoading(false);
            })
            .catch(err => {
                setError(err.message);
                setLoading(false);
            });
    };

    const handleCreatePlayer = () => {
        if (!newVoornaam || !newNaam) return;

        fetch('https://localhost:7241/api/players', {
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

    return (
        <div className="min-h-screen flex justify-center items-start bg-gray-50">
            <div className="max-w-3xl w-full p-6">
                <div className="max-w-3xl mx-auto p-6">
                    <h1 className="text-3xl font-bold text-white bg-blue-600 p-4 rounded-2xl shadow mb-6 text-center">
                        Spelers Pagina
                    </h1>

                    {/* Voeg speler toe */}
                    <div className="mb-8 bg-white p-4 rounded-xl shadow">
                        <h2 className="text-xl font-semibold mb-4">Nieuwe Speler Toevoegen</h2>
                        <div className="flex flex-col sm:flex-row gap-2 mb-2">
                            <input
                                type="text"
                                placeholder="Voornaam"
                                value={newVoornaam}
                                onChange={(e) => setNewVoornaam(e.target.value)}
                                className="border border-gray-300 p-2 rounded w-full"
                            />
                            <input
                                type="text"
                                placeholder="Naam"
                                value={newNaam}
                                onChange={(e) => setNewNaam(e.target.value)}
                                className="border border-gray-300 p-2 rounded w-full"
                            />
                        </div>
                        <button
                            onClick={handleCreatePlayer}
                            className="bg-green-600 hover:bg-green-700 text-white px-4 py-2 rounded shadow"
                        >
                            Voeg toe
                        </button>
                    </div>

                    {/* Zoek op ID */}
                    <div className="mb-6 flex flex-col sm:flex-row gap-2 sm:items-center">
                        <input
                            type="number"
                            placeholder="Zoek op speler ID"
                            value={searchId}
                            onChange={(e) => setSearchId(e.target.value)}
                            className="border border-gray-300 p-2 rounded w-full sm:w-auto"
                        />
                        <button
                            onClick={handleSearch}
                            className="bg-blue-500 hover:bg-blue-600 text-white px-4 py-2 rounded shadow"
                        >
                            Zoek
                        </button>
                        <button
                            onClick={fetchAllPlayers}
                            className="bg-gray-400 hover:bg-gray-500 text-white px-4 py-2 rounded shadow"
                        >
                            Toon alles
                        </button>
                    </div>

                    {/* Loading & Error */}
                    {loading && <p className="text-blue-500">Bezig met laden...</p>}
                    {error && <p className="text-red-600 font-semibold mb-4">Error: {error}</p>}

                    {/* Spelers lijst */}
                    <ul className="space-y-4">
                        {spelers.map(speler => (
                            <li key={speler.spelerId} className="p-4 bg-gray-100 rounded-lg shadow">
                                <p><span className="font-semibold">ID:</span> {speler.spelerId}</p>
                                <p><span className="font-semibold">Naam:</span> {speler.voornaam} {speler.naam}</p>
                            </li>
                        ))}
                    </ul>
                </div>
            </div>
        </div>
    );
}



export default SpelerPagina;
