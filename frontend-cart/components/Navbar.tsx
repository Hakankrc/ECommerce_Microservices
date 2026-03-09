"use client";

import Cookies from 'js-cookie';
import Link from "next/link";
import { useEffect, useState } from "react";
import axios from "axios";
import { ShoppingCartIcon, UserIcon, ArrowRightOnRectangleIcon } from "@heroicons/react/24/outline"; 

export default function Navbar() {
  const [itemCount, setItemCount] = useState(0);
  const [mounted, setMounted] = useState(false);
  const [isLoggedIn, setIsLoggedIn] = useState(false);

  const getUserFromToken = (token: string | undefined) => {
    if (!token) return null;
    try {
      const base64Url = token.split('.')[1];
      const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
      const jsonPayload = decodeURIComponent(window.atob(base64).split('').map(function(c) {
          return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
      }).join(''));
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
      const total = items.reduce((sum: number, item: any) => sum + (item.quantity || item.Quantity || 0), 0);
      setItemCount(total);
    } catch (error) { setItemCount(0); }
  };

  const handleLogout = () => {
    Cookies.remove("access_token");
    Cookies.remove("refresh_token");
    window.location.href = "http://localhost:3000";
  };

  return (
    <nav className="bg-white shadow-md border-b sticky top-0 z-50">
      <div className="max-w-7xl mx-auto px-4 h-16 flex justify-between items-center">
        <a href="http://localhost:3000" className="text-2xl font-black bg-linear-to-r from-indigo-600 to-purple-600 bg-clip-text text-transparent italic">
          HAKAN-STORE
        </a>

        <div className="flex items-center space-x-6">
          <a href="http://localhost:3000" className="text-gray-600 font-bold">Anasayfa</a>

          <Link href="/" className="relative p-2 group">
            <ShoppingCartIcon className="h-7 w-7 text-indigo-600" />
            {itemCount > 0 && (
              <span className="absolute -top-1 -right-1 bg-red-500 text-white text-[10px] font-bold h-5 w-5 flex items-center justify-center rounded-full">
                {itemCount}
              </span>
            )}
          </Link>

          <div className="flex items-center border-l pl-6 space-x-4">
            {isLoggedIn ? (
              <button onClick={handleLogout} className="text-red-500 font-bold">Logout</button>
            ) : (
              <a href="http://localhost:3000/login" className="text-gray-600 font-bold">Login</a>
            )}
          </div>
        </div>
      </div>
    </nav>
  );
}

