"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import axios from "axios";
import { ShoppingCartIcon } from "@heroicons/react/24/outline"; // İkon için (npm install @heroicons/react)

export default function Navbar() {
  const [itemCount, setItemCount] = useState(0);

  useEffect(() => {
    const getBasketCount = async () => {
  try {
    const response = await axios.get("http://localhost:5153/api/Basket/Hakan-123");
    
    // Backend'den gelen veriyi konsola basalım ki gözümüzle görelim
    console.log("Redis'ten Gelen Sepet:", response.data);

    // Hem 'items' hem 'Items' (Büyük/Küçük harf) kontrolü yapıyoruz
    const items = response.data.items || response.data.Items || [];
    
    const total = items.reduce((sum: number, item: any) => sum + item.quantity, 0);
    console.log("Hesaplanan Toplam Sayı:", total);
    
    setItemCount(total);
  } catch (error) {
    console.error("Navbar sepet çekme hatası:", error);
  }
};

    getBasketCount();
    
    // Sayfada sepete ekleme yapıldığında Navbar'ın haberi olsun diye basit bir event dinleyici (Opsiyonel)
    window.addEventListener("basketUpdated", getBasketCount);
    return () => window.removeEventListener("basketUpdated", getBasketCount);
  }, []);

  return (
    <nav className="bg-white shadow-md border-b border-gray-100 sticky top-0 z-50">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex justify-between h-16 items-center">
          {/* Logo */}
          <Link href="/" className="flex items-center space-x-2">
            <span className="text-2xl font-black bg-gradient-to-r from-indigo-600 to-purple-600 bg-clip-text text-transparent">
              HAKAN-STORE
            </span>
          </Link>

          {/* Menü & Sepet */}
          <div className="flex items-center space-x-8">
            <Link href="/" className="text-gray-600 hover:text-indigo-600 font-medium transition-colors">
              Anasayfa
            </Link>
            
            {/* Sepet İkonu */}
            <Link href="/basket" className="relative group p-2">
              <ShoppingCartIcon className="h-7 w-7 text-gray-700 group-hover:text-indigo-600 transition-colors" />
              {itemCount > 0 && (
                <span className="absolute -top-1 -right-1 bg-red-500 text-white text-[10px] font-bold h-5 w-5 flex items-center justify-center rounded-full shadow-lg animate-bounce">
                  {itemCount}
                </span>
              )}
            </Link>
          </div>
        </div>
      </div>
    </nav>
  );
}