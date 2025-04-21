import { Routes, Route, useNavigate } from 'react-router-dom'
import SecondPage from '../pages/aanwezigheidspagina.tsx'
import ThirdPage from '../pages/spelerpagina.tsx'
import HomePage from '../pages/HomePagina.tsx'
import ScorePagina from '../pages/scorebladpagina.tsx'
import vlasLogo from '../images/vlas_logo.png'; // Zorg ervoor dat het pad klopt!

function Header() {
    const navigate = useNavigate();

    return (
        <div className="min-h-screen bg-gray-100 text-gray-900">
            <header className="bg-white p-4 shadow-md">
                <div className="container mx-auto flex justify-between items-center">
                    <div className="flex flex-col items-center">
                        <img
                            src={vlasLogo}
                            alt="VL@S Petanque Logo"
                            className="h-15"
                        />
                        <h2 className="text-[#3c444c] text-xl font-bold mt-2">Petanque</h2>
                    </div>
                    <nav className="space-x-4">
                        <button
                            onClick={() => navigate('/')}
                            className="bg-[#fbd46d] text-[#3c444c] font-bold py-2 px-4 rounded hover:bg-[#f7c84c] transition cursor-pointer"
                        >
                            Home Pagina
                        </button>
                        <button
                            onClick={() => navigate('/aanwezigheden')}
                            className="bg-[#fbd46d] text-[#3c444c] font-bold py-2 px-4 rounded hover:bg-[#f7c84c] transition cursor-pointer"
                        >
                            Aanwezigheden Opnemen
                        </button>
                        <button
                            onClick={() => navigate('/spelers')}
                            className="bg-[#fbd46d] text-[#3c444c] font-bold py-2 px-4 rounded hover:bg-[#f7c84c] transition cursor-pointer"
                        >
                            Speler Pagina
                        </button>
                        <button
                            onClick={() => navigate('/scorebladeren')}
                            className="bg-[#fbd46d] text-[#3c444c] font-bold py-2 px-4 rounded hover:bg-[#f7c84c] transition cursor-pointer"
                        >
                            Scores
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
