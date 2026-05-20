import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { GameProvider } from './context/GameContext';
import StartPage from './pages/StartPage';
import SelectRolePage from './pages/SelectRolePage';
import GuestGamePage from './pages/GuestGamePage';
import DirectorGamePage from './pages/DirectorGamePage';

function App() {
  return (
    <GameProvider>
      <Router>
        <Routes>
          <Route path="/" element={<StartPage />} />
          <Route path="/select-role" element={<SelectRolePage />} />
          <Route path="/guest-game" element={<GuestGamePage />} />
          <Route path="/director-game" element={<DirectorGamePage />} />
        </Routes>
      </Router>
    </GameProvider>
  );
}

export default App;
