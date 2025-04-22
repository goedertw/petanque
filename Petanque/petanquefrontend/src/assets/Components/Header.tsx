import { useState } from "react";
import { Routes, Route, useNavigate } from 'react-router-dom';
import SecondPage from '../pages/aanwezigheidspagina.tsx';
import ThirdPage from '../pages/spelerpagina.tsx';
import HomePage from '../pages/HomePagina.tsx';
import ScorePagina from '../pages/scorebladpagina.tsx';
import vlasLogo from '../images/vlas_logo.png'; // Zorg ervoor dat het pad klopt!
import DagKlassementpagina from '../pages/Dagklassementpagina.tsx';

function Header() {
    const navigate = useNavigate();
    const [isMenuOpen, setIsMenuOpen] = useState(false);

    // Toggle the mobile menu
    const toggleMenu = () => setIsMenuOpen(!isMenuOpen);

    // Handle navigation and close the menu when a link is clicked
    const handleNavigation = (path: string) => {
        navigate(path);
        setIsMenuOpen(false); // Close the menu after navigation
    };

    return (
        <div className="min-h-screen bg-gray-100 text-gray-900">
            <header className="bg-white p-4 shadow-md">
                <div className="container mx-auto flex justify-between items-center">
                    {/* Logo Section */}
                    <div className="flex flex-col items-center">
                        <img
                            src={vlasLogo}
                            alt="VL@S Petanque Logo"
                            className="h-15"
                        />
                        <h2 className="text-[#3c444c] text-xl font-bold mt-2">Petanque</h2>
                    </div>

                    {/* Hamburger menu button */}
                    <button
                        onClick={toggleMenu}
                        className="block lg:hidden p-2 rounded-md text-[#3c444c] focus:outline-none"
                    >
                        <span className="block w-6 h-1 bg-[#3c444c] mb-1"></span>
                        <span className="block w-6 h-1 bg-[#3c444c] mb-1"></span>
                        <span className="block w-6 h-1 bg-[#3c444c]"></span>
                    </button>

                    {/* Navigation Links */}
                    <nav className={`lg:flex space-x-4 ${isMenuOpen ? "block" : "hidden"} lg:block`}>
                        <button
                            onClick={() => handleNavigation('/')}
                            className="w-36 bg-[#fbd46d] text-[#3c444c] font-bold py-2 px-4 rounded hover:bg-[#f7c84c] transition cursor-pointer"
                        >
                            Home
                        </button>
                        <button
                            onClick={() => handleNavigation('/aanwezigheden')}
                            className="w-36 bg-[#fbd46d] text-[#3c444c] font-bold py-2 px-4 rounded hover:bg-[#f7c84c] transition cursor-pointer"
                        >
                            Aanwezigheden
                        </button>
                        <button
                            onClick={() => handleNavigation('/spelers')}
                            className="w-36 bg-[#fbd46d] text-[#3c444c] font-bold py-2 px-4 rounded hover:bg-[#f7c84c] transition cursor-pointer"
                        >
                            Spelers
                        </button>
                        <button
                            onClick={() => handleNavigation('/scorebladeren')}
                            className="w-36 bg-[#fbd46d] text-[#3c444c] font-bold py-2 px-4 rounded hover:bg-[#f7c84c] transition cursor-pointer"
                        >
                            Scores
                        </button>
                        <button
                            onClick={() => handleNavigation('/dagklassement')}
                            className="w-36 bg-[#fbd46d] text-[#3c444c] font-bold py-2 px-4 rounded hover:bg-[#f7c84c] transition cursor-pointer"
                        >
                            Dagklassementen
                        </button>
                    </nav>

                    {/* Mobile Menu (Sliding) */}
                    <div
                        className={`fixed inset-0 bg-[#3c444c] bg-opacity-75 lg:hidden transition-all duration-300 transform ${
                            isMenuOpen ? "translate-x-0" : "translate-x-full"
                        } lg:translate-x-0 flex justify-center items-center space-x-4`}
                    >
                        {/* Adjust layout for mobile dropdown */}
                        <div className="flex flex-col items-center space-y-4">
                            <button
                                onClick={() => handleNavigation('/')}
                                className="bg-[#fbd46d] text-[#3c444c] font-bold py-2 px-4 rounded hover:bg-[#f7c84c] transition cursor-pointer"
                            >
                                Home
                            </button>
                            <button
                                onClick={() => handleNavigation('/aanwezigheden')}
                                className="bg-[#fbd46d] text-[#3c444c] font-bold py-2 px-4 rounded hover:bg-[#f7c84c] transition cursor-pointer"
                            >
                                Aanwezigheden
                            </button>
                            <button
                                onClick={() => handleNavigation('/spelers')}
                                className="bg-[#fbd46d] text-[#3c444c] font-bold py-2 px-4 rounded hover:bg-[#f7c84c] transition cursor-pointer"
                            >
                                Spelers
                            </button>
                            <button
                                onClick={() => handleNavigation('/scorebladeren')}
                                className="bg-[#fbd46d] text-[#3c444c] font-bold py-2 px-4 rounded hover:bg-[#f7c84c] transition cursor-pointer"
                            >
                                Scores
                            </button>
                            <button
                                onClick={() => handleNavigation('/dagklassement')}
                                className="bg-[#fbd46d] text-[#3c444c] font-bold py-2 px-4 rounded hover:bg-[#f7c84c] transition cursor-pointer"
                            >
                                Dagklassementen
                            </button>
                        </div>
                    </div>
                </div>
            </header>

            <main className="container mx-auto p-8">
                <Routes>
                    <Route path="/" element={<HomePage />} />
                    <Route path="/aanwezigheden" element={<SecondPage />} />
                    <Route path="/spelers" element={<ThirdPage />} />
                    <Route path="/scorebladeren" element={<ScorePagina />} />
                    <Route path="/dagklassement" element={<DagKlassementpagina />} />
                </Routes>
            </main>
        </div>
    );
}

export default Header;
