import { Routes, Route, useNavigate } from 'react-router-dom'
import SecondPage from '../pages/aanwezigheidspagina.tsx'
import ThirdPage from '../pages/spelerpagina.tsx'
import HomePage from '../pages/HomePagina.tsx'
import ScorePagina from '../pages/scorebladpagina.tsx'

function Header() {
    const navigate = useNavigate();

    return (
        <div className="min-h-screen bg-gray-100 text-gray-900">
            <header className="bg-green-600 p-4 shadow-md">
                <div className="container mx-auto flex justify-between items-center">
                    <h1 className="text-2xl text-white font-bold">VL@S Petanque</h1>
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
                        <button
                            onClick={() => navigate('/')}
                            className="bg-orange-500 text-white py-2 px-4 rounded hover:bg-orange-600 transition"
                        >
                            Home Pagina
                        </button>
                        <button
                            onClick={() => navigate('/scorebladeren')}
                            className="bg-orange-500 text-white py-2 px-4 rounded hover:bg-orange-600 transition"
                        >
                            scores
                        </button>
                    </nav>
                </div>
            </header>
            <main className="container mx-auto p-8">
                <Routes>
                    <Route path="/" element={<HomePage />} />
                    <Route path="/aanwezigheden" element={<SecondPage />} />
                    <Route path="/spelers" element={<ThirdPage />} />
                    <Route path="/scorebladeren" element={<ScorePagina />} />
                </Routes>
            </main>
        </div>
    );
}

export default Header;
