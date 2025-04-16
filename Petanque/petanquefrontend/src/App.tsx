import { Routes, Route, useNavigate } from 'react-router-dom'
import SecondPage from './assets/pages/aanwezigheidspagina.tsx'
import ThirdPage from './assets/pages/spelerpagina.tsx'

function App() {
    const navigate = useNavigate()

    return (
        <>
            <Routes>
                <Route path="/second" element={<SecondPage />} />
            </Routes>
            <Routes>
                <Route path="/third" element={<ThirdPage />} />
            </Routes>

            <button onClick={() => navigate('/second')}>Aanwezigheden Opnemen</button>
            <button onClick={() => navigate('/third')}>Speler Pagina</button>
        </>
    )
}

export default App
