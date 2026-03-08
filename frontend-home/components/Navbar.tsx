"use client";

import Cookies from 'js-cookie';
import Link from "next/link";
import { useEffect, useState } from "react";
import axios from "axios";
import { ShoppingCartIcon } from "@heroicons/react/24/outline"; 

export default function Navbar() {
  const [itemCount, setItemCount] = useState(0);
  const [mounted, setMounted] = useState(false);

  
  const getUserFromToken = (token: string | undefined) => {
    if (!token) return null;
    try {
      const base64Url = token.split('.')[1];
      const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
      const jsonPayload = decodeURIComponent(window.atob(base64).split('').map(function(c) {
          return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
      }).join(''));
      
      const payload = JSON.parse(jsonPayload);

      
      return (
        payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"] || 
        payload.email || 
        payload.sub || 
        payload.unique_name
      );
    } catch (e) {
      console.error("Navbar: Token decode hatası:", e);
      return null;
    }
  };

  const getBasketCount = async () => {
    const token = Cookies.get("access_token");
    const userName = getUserFromToken(token);

    if (!token || !userName) {
        setItemCount(0);
        return;
    }

    try {
      const response = await axios.get(`http://localhost:5153/api/Basket/${userName}`, {
        headers: {
            'Authorization': `Bearer ${token}`
        }
      });
      
      const items = response.data.items || response.data.Items || [];
      
      const total = items.reduce((sum: number, item: any) => 
        sum + (item.quantity || item.Quantity || 0), 0
      );
      
      setItemCount(total);
    } catch (error: any) {
        if (error.response && error.response.status === 404) {
            setItemCount(0);
        } else {
            console.error("Navbar sepet çekme hatası:", error);
        }
    }
  };

  useEffect(() => {
    setMounted(true);
    getBasketCount();
    
    
    window.addEventListener("basketUpdated", getBasketCount);
    return () => window.removeEventListener("basketUpdated", getBasketCount);
  }, []);

  if (!mounted) {
    return <nav className="h-16 bg-white shadow-md border-b border-gray-100"></nav>; 
  }

  return (
    <nav className="bg-white shadow-md border-b border-gray-100 sticky top-0 z-50">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex justify-between h-16 items-center">
          
          {/* Logo */}
          <Link href="/" className="flex items-center space-x-2">
            <span className="text-2xl font-black bg-gradient-to-r from-indigo-600 to-purple-600 bg-clip-text text-transparent italic">
              HAKAN-STORE
            </span>
          </Link>

          {/* Menü & Sepet */}
          <div className="flex items-center space-x-8">
            <Link href="/" className="text-gray-600 hover:text-indigo-600 font-bold transition-colors">
              Anasayfa
            </Link>
            
            {/* Sepet İkonu - Linki 3001 portuna (Cart Microfrontend) yönlendiriyoruz */}
            <a href="/basket" className="relative group p-2">
              <ShoppingCartIcon className="h-7 w-7 text-gray-700 group-hover:text-indigo-600 transition-colors" />
              {itemCount > 0 && (
                <span className="absolute -top-1 -right-1 bg-red-600 text-white text-[10px] font-black h-5 w-5 flex items-center justify-center rounded-full shadow-lg shadow-red-200 animate-bounce">
                  {itemCount}
                </span>
              )}
            </a>
          </div>
        </div>
      </div>
    </nav>
  );
}