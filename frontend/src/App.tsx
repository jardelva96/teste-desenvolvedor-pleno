import { BrowserRouter as Router, Route, Routes } from 'react-router-dom'; // Use Routes em vez de Switch
import Login from './pages/Login';
import HomePage from './pages/HomePage';

const App = () => {
  return (
    <Router>
      <Routes> {/* Substituímos Switch por Routes */}
        {/* Rota para Login */}
        <Route path="/login" element={<Login />} />
        
        {/* Rota para a página inicial */}
        <Route path="/home" element={<HomePage />} />
        
        {/* Rota padrão ou fallback */}
        <Route path="/" element={<Login />} /> {/* Rota padrão */}
      </Routes>
    </Router>
  );
}

export default App;
