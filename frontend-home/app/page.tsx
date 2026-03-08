"use client";

import Cookies from 'js-cookie';
import { useEffect, useState } from "react";
import axios from "axios";
import { Product } from "@/types"; 
import { toast, ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';

export default function Home() {
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(true);
  const [mounted, setMounted] = useState(false);
  const [isLoggedIn, setIsLoggedIn] = useState(false);

  // KRİTİK: Token Çözücü
  const getUserFromToken = (token: string) => {
    try {
      const base64Url = token.split('.')[1];
      const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
      const jsonPayload = decodeURIComponent(window.atob(base64).split('').map(function(c) {
          return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
      }).join(''));
      
      const payload = JSON.parse(jsonPayload);
      console.log("Hakan Kral - Payload Yakalandı:", payload);

      
      return (
        payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"] || 
        payload.email || 
        payload.sub || 
        payload.unique_name
      );
    } catch (e) {
      console.error("Token decode hatası:", e);
      return null;
    }
  };
  const addToBasket = async (product: Product) => {
    const token = Cookies.get('access_token');
    
    console.log("--- SEPETE EKLEME DENEMESİ ---");
    console.log("1. Cookies'ten okunan token:", token ? "VAR (Uzun metin)" : "YOK!");

    if (!token) {
      toast.info("Sepete eklemek için giriş yapmalısınız! 🔒");
      return;
    }

    const userName = getUserFromToken(token);
    console.log("2. Token'dan çözülen Kullanıcı Adı:", userName);

    if (!userName) {
      toast.error("Token var ama kullanıcı adın okunamadı! Konsola bak.");
      return;
    }

    const authHeader = { headers: { Authorization: `Bearer ${token}` } };

    try {
      console.log("3. Backend'e istek atılıyor...");
      let currentItems: any[] = [];
      try {
        const currentRes = await axios.get(`http://localhost:5153/api/Basket/${userName}`, authHeader);
        currentItems = currentRes.data.items || currentRes.data.Items || [];
      } catch (err) {
        console.log("Sepet boş, yeni oluşturulacak.");
      }

      const existingItem = currentItems.find((i: any) => (i.productId || i.ProductId) === product.id.toString());
      if (existingItem) {
        if (existingItem.quantity !== undefined) existingItem.quantity += 1;
        else if (existingItem.Quantity !== undefined) existingItem.Quantity += 1;
      } else {
        currentItems.push({
          ProductId: product.id.toString(),
          ProductName: product.name,
          Price: product.price,
          Quantity: 1,
          PictureUrl: product.pictureUrl
        });
      }

      await axios.post("http://localhost:5153/api/Basket", {
        UserName: userName,
        Items: currentItems
      }, authHeader);

      toast.success(`${product.name} sepete eklendi! 🛒`);
      window.dispatchEvent(new Event("basketUpdated"));
      
    } catch (err) {
      console.error("Backend Hatası:", err);
      toast.error("Sunucu sepeti kaydedemedi.");
    }
  };

  useEffect(() => {
    setMounted(true);
    const fetchProducts = async () => {
        try {
          const response = await axios.get("http://localhost:5153/api/product");
          setProducts(response.data);
        } catch (err) { console.error(err); }
        finally { setLoading(false); }
    };
    fetchProducts();
    setIsLoggedIn(!!Cookies.get('access_token'));
  }, []);

  if (!mounted) return null;

  return (
    <main className="min-h-screen bg-gray-50 p-10">
      <ToastContainer position="bottom-right" /> 
      <div className="max-w-7xl mx-auto">
        <div className="flex justify-between items-center mb-12">
            <h1 className="text-4xl font-black text-indigo-900 tracking-tighter">🔥 HAKAN STORE</h1>
            <div className={`px-4 py-2 rounded-full font-bold shadow-sm ${isLoggedIn ? 'bg-green-100 text-green-700' : 'bg-red-100 text-red-700'}`}>
               {isLoggedIn ? '🟢 Oturum Açık' : '🔴 Misafir'}
            </div>
        </div>
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-10">
            {products.map((product) => (
                <div key={product.id} className="bg-white rounded-3xl p-8 shadow-sm">
                    <img src={product.pictureUrl} className="h-40 w-full object-cover rounded-xl mb-4" />
                    <h2 className="text-xl font-bold">{product.name}</h2>
                    <button onClick={() => addToBasket(product)} className="mt-4 w-full bg-indigo-600 text-white py-3 rounded-xl font-bold">
                        Ekle
                    </button>
                </div>
            ))}
        </div>
      </div>
    </main>
  );
}