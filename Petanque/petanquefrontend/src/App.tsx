import { Routes, Route, useNavigate } from 'react-router-dom'
import SecondPage from './assets/pages/aanwezigheidspagina.tsx'
import ThirdPage from './assets/pages/spelerpagina.tsx'

function App() {
    const navigate = useNavigate();

    return (
        <div className="min-h-screen bg-gray-100 text-gray-900">
            {/* Header Section */}
            <header className="bg-green-600 p-4 shadow-md">
                <div className="container mx-auto flex justify-between items-center">
                    <h1 className="text-2xl text-white font-bold">Petanque Club</h1>
                    <nav className="space-x-4">
                        <button
                            onClick={() => navigate('/aanwezigheden')}
                            className="bg-blue-500 text-white py-2 px-4 rounded hover:bg-blue-600 transition"
                        >
                            Aanwezigheden Opnemen
                        </button>
                        <button
                            onClick={() => navigate('/spelers')}
                            className="bg-orange-500 text-white py-2 px-4 rounded hover:bg-orange-600 transition"
                        >
                            Speler Pagina
                        </button>
                    </nav>
                </div>
            </header>

            {/* Main Content Section */}
            <main className="container mx-auto p-8">
                <Routes>
                    <Route path="/aanwezigheden" element={<SecondPage />} />
                    <Route path="/spelers" element={<ThirdPage />} />
                </Routes>
            </main>

            {/* Footer Section */}
            <footer className="bg-gray-800 text-white py-4">
                <div className="container mx-auto text-center">
                    <p>© 2025 Petanque Club</p>
                </div>
            </footer>
        </div>
    );
}

export default App;
