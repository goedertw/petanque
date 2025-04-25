import { useState } from "react";
import { Routes, Route, useNavigate } from 'react-router-dom';
import SecondPage from '../pages/aanwezigheidspagina.tsx';
import ThirdPage from '../pages/spelerpagina.tsx';
import HomePage from '../pages/HomePagina.tsx';
import ScorePagina from '../pages/scorebladpagina.tsx';
import vlasLogo from '../images/vlas_logo.png';
// import DagKlassementpagina from '../pages/Dagklassementpagina.tsx';
import SpeeldagenDropdown from '../pages/spelverdeling.tsx';
import KlassementenPagina from "../pages/KlassementenPagina.tsx";

function Header() {
    const navigate = useNavigate();
    const [isMenuOpen, setIsMenuOpen] = useState(false);

    const toggleMenu = () => setIsMenuOpen(!isMenuOpen);
    const handleNavigation = (path: string) => {
        navigate(path);
        setIsMenuOpen(false);
    };

    const navButtonClass =
        "bg-[#fbd46d] text-[#3c444c] font-bold py-1.5 px-3 rounded-2xl shadow hover:bg-[#f7c84c] transition duration-200";

    return (
        <div className="min-h-screen bg-gray-100 text-gray-900">
            <header className="bg-white py-4 px-6 shadow-md sticky top-0 z-50">
                <div className="mx-auto max-w-screen-xl flex justify-between items-center flex-wrap gap-y-4">
                    {/* Logo */}
                    <div className="flex flex-col items-center">
                        <img
                            src={vlasLogo}
                            alt="VL@S Petanque Logo"
                            className="h-16"
                        />
                        <h2 className="text-[#3c444c] text-xl font-bold mt-2">Petanque</h2>
                    </div>

                    {/* Hamburger Menu */}
                    <button
                        onClick={toggleMenu}
                        className="lg:hidden p-2 rounded focus:outline-none transition-transform"
                    >
                        <div className="space-y-1">
                            <span className="block w-6 h-0.5 bg-[#3c444c]"></span>
                            <span className="block w-6 h-0.5 bg-[#3c444c]"></span>
                            <span className="block w-6 h-0.5 bg-[#3c444c]"></span>
                        </div>
                    </button>

                    {/* Desktop Navigation */}
                    <nav className="hidden lg:flex flex-wrap gap-3 justify-center">
                        <button onClick={() => handleNavigation('/')} className={navButtonClass}>Home</button>
                        <button onClick={() => handleNavigation('/aanwezigheden')} className={navButtonClass}>Aanwezigheden</button>
                        <button onClick={() => handleNavigation('/spelers')} className={navButtonClass}>Spelers</button>
                        <button onClick={() => handleNavigation('/scorebladeren')} className={navButtonClass}>Scores</button>
                        <button onClick={() => handleNavigation('/klassement')} className={navButtonClass}>Klassementen</button>
                        <button onClick={() => handleNavigation('/spelverdeling')} className={navButtonClass}>Spelverdelingen</button>
                    </nav>
                </div>

                {/* Mobile Navigation */}
                {isMenuOpen && (
                    <div className="lg:hidden mt-4 bg-white rounded-xl p-4 shadow space-y-2 flex flex-col items-center animate-fade-in">
                        <button onClick={() => handleNavigation('/')} className={navButtonClass}>Home</button>
                        <button onClick={() => handleNavigation('/aanwezigheden')} className={navButtonClass}>Aanwezigheden</button>
                        <button onClick={() => handleNavigation('/spelers')} className={navButtonClass}>Spelers</button>
                        <button onClick={() => handleNavigation('/scorebladeren')} className={navButtonClass}>Scores</button>
                        <button onClick={() => handleNavigation('/dagklassement')} className={navButtonClass}>Dagklassementen</button>
                        <button onClick={() => handleNavigation('/spelverdeling')} className={navButtonClass}>Spelverdelingen</button>
                    </div>
                )}
            </header>

            <main className="max-w-screen-xl mx-auto p-8">
                <Routes>
                    <Route path="/" element={<HomePage />} />
                    <Route path="/aanwezigheden" element={<SecondPage />} />
                    <Route path="/spelers" element={<ThirdPage />} />
                    <Route path="/scorebladeren" element={<ScorePagina />} />
                    <Route path="/klassement" element={<KlassementenPagina />} />
                    <Route path="/spelverdeling" element={<SpeeldagenDropdown />} />
                </Routes>
            </main>
        </div>
    );
}

export default Header;
