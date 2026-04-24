import { BrowserRouter, Routes, Route } from 'react-router-dom';
import Landing from './pages/Landing/Landing';
import Totem from './pages/Totem/Totem';
import OrderTracking from './pages/Totem/OrderTracking';
import Login from './pages/Restaurant/Login';
import Kitchen from './pages/Restaurant/Kitchen';

export default function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Landing />} />
        <Route path="/totem" element={<Totem />} />
        <Route path="/totem/order/:id" element={<OrderTracking />} />
        <Route path="/restaurant/login" element={<Login />} />
        <Route path="/restaurant/kitchen" element={<Kitchen />} />
      </Routes>
    </BrowserRouter>
  );
}
