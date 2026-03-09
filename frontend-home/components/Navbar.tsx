"use client";

import Cookies from 'js-cookie';
import Link from "next/link";
import { useEffect, useState } from "react";
import axios from "axios";
import { ShoppingCartIcon, ArrowRightOnRectangleIcon } from "@heroicons/react/24/outline"; 

export default function Navbar() {
  const [itemCount, setItemCount] = useState(0);
  const [mounted, setMounted] = useState(false);
  const [isLoggedIn, setIsLoggedIn] = useState(false);

  
  const getUserFromToken = (token: string | undefined) => {
    if (!token) return null;
    try {
      const base64Url = token.split('.')[1];
      const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
      const jsonPayload = decodeURIComponent(window.atob(base64).split('').map(c => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2)).join(''));
      const payload = JSON.parse(jsonPayload);
      return payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"] || payload.email;
    } catch (e) { return null; }
  };

  const getBasketCount = async () => {
    const token = Cookies.get("access_token");
    const userName = getUserFromToken(token);
    if (!token || !userName) { setItemCount(0); return; }

    try {
      const response = await axios.get(`http://localhost:5153/api/Basket/${userName}`, {
        headers: { 'Authorization': `Bearer ${token}` }
      });
      const items = response.data.items || response.data.Items || [];
      setItemCount(items.reduce((sum: number, i: any) => sum + (i.quantity || i.Quantity || 0), 0));
    } catch (e) { setItemCount(0); }
  };

  const handleLogout = () => {
    Cookies.remove("access_token");
    Cookies.remove("refresh_token");
    window.location.href = "http://localhost:3000"; 
  };

  useEffect(() => {
    setMounted(true);
    getBasketCount();
    setIsLoggedIn(!!Cookies.get("access_token"));
    window.addEventListener("basketUpdated", getBasketCount);
    return () => window.removeEventListener("basketUpdated", getBasketCount);
  }, []);

  if (!mounted) return <nav className="h-16 bg-white shadow-md border-b"></nav>;

  return (
    <nav className="bg-white shadow-md border-b sticky top-0 z-50">
      <div className="max-w-7xl mx-auto px-4 h-16 flex justify-between items-center">
        
        <Link href="/" className="text-2xl font-black bg-gradient-to-r from-indigo-600 to-purple-600 bg-clip-text text-transparent italic">
          HAKAN-STORE
        </Link>

        <div className="flex items-center space-x-6">
          <Link href="/" className="text-gray-600 font-bold">Anasayfa</Link>
          
          
          <a href="http://localhost:3001/basket" className="relative p-2 group">
            <ShoppingCartIcon className="h-7 w-7 text-gray-700 group-hover:text-indigo-600 transition-colors" />
            {itemCount > 0 && (
              <span className="absolute -top-1 -right-1 bg-red-600 text-white text-[10px] font-black h-5 w-5 flex items-center justify-center rounded-full animate-bounce">
                {itemCount}
              </span>
            )}
          </a>

          <div className="flex items-center border-l pl-6 space-x-4">
            {isLoggedIn ? (
              <button onClick={handleLogout} className="flex items-center text-red-500 font-bold">
                <ArrowRightOnRectangleIcon className="h-6 w-6 mr-1" /> Çıkış
              </button>
            ) : (
              <>
                <Link href="/login" className="text-gray-600 font-bold">Giriş</Link>
                <Link href="/register" className="bg-indigo-600 text-white px-4 py-2 rounded-lg font-bold">Kayıt Ol</Link>
              </>
            )}
          </div>
        </div>
      </div>
    </nav>
  );
}