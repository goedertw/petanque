import { useEffect, useState } from 'react';
import './App.css';

// TypeScript-interface voor speler
interface Speler {
    spelerId: number;
    voornaam: string;
    naam: string;
}

function App() {
    const [spelers, setSpelers] = useState<Speler[]>([]);

    useEffect(() => {
        fetch('https://localhost:7241/api/players')
            .then(response => response.json())
            .then(data => setSpelers(data))
            .catch(error => console.error('Fout bij ophalen:', error));
    }, []);

    return (
        <div className="App">
            <h1>Spelerslijst</h1>
            <ul>
                {spelers.map(speler => (
                    <li key={speler.spelerId}>
                        {speler.voornaam} {speler.naam}
                    </li>
                ))}
            </ul>
        </div>
    );
}

export default App;
