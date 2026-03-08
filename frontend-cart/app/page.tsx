"use client";

import Cookies from 'js-cookie';
import { useEffect, useState } from "react";
import axios from "axios";
import { TrashIcon, ArrowLeftIcon } from "@heroicons/react/24/outline";
import { toast, ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';

export default function CartPage() {
  const [basketItems, setBasketItems] = useState<any[]>([]);
  const [totalPrice, setTotalPrice] = useState(0);
  const [loading, setLoading] = useState(true);
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
      
      // .NET Identity Claim Map 
      return (
        payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"] || 
        payload.email || 
        payload.sub || 
        payload.unique_name
      );
    } catch (e) {
      console.error("CartPage: Token decode hatası:", e);
      return null;
    }
  };

  const fetchBasket = async () => {
    
    const token = Cookies.get("access_token"); 
    const userName = getUserFromToken(token);

    if (!token || !userName) {
        setLoading(false);
        return;
    }

    try {
      const response = await axios.get(`http://localhost:5153/api/Basket/${userName}`, {
        headers: { Authorization: `Bearer ${token}` }
      });

      const items = response.data.items || response.data.Items || [];
      setBasketItems(items);
      
      const total = items.reduce((sum: number, item: any) => sum + (item.price * item.quantity), 0);
      setTotalPrice(total);
    } catch (error: any) {
      console.error("Sepet çekilemedi:", error);
      if (error.response?.status === 401) toast.error("Oturumunuz geçersiz! Lütfen tekrar giriş yapın.");
    } finally {
      setLoading(false);
    }
  };

  const removeItem = async (productId: string) => {
    const token = Cookies.get("access_token");
    const userName = getUserFromToken(token);
    
    if (!token || !userName) return;

    // Create a new list that only includes items that are not being removed
    const updatedItems = basketItems.filter(item => (item.productId || item.ProductId) !== productId);

    try {
      
      await axios.post("http://localhost:5153/api/Basket", {
        UserName: userName,
        Items: updatedItems
      }, {
        headers: { Authorization: `Bearer ${token}` }
      });

      setBasketItems(updatedItems);
      const total = updatedItems.reduce((sum: number, item: any) => sum + (item.price * item.quantity), 0);
      setTotalPrice(total);
      
      toast.info("Ürün sepetten çıkarıldı.");
      
      
      window.dispatchEvent(new Event("basketUpdated"));

    } catch (error) {
      console.error("Silme hatası:", error);
      toast.error("Ürün silinemedi.");
    }
  };

  useEffect(() => {
    setMounted(true);
    fetchBasket();
  }, []);

  
  if (!mounted) return null;

  if (loading) return (
    <div className="min-h-screen flex items-center justify-center bg-slate-50">
      <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-indigo-600"></div>
      <span className="ml-4 font-bold text-indigo-600">Sepetiniz yükleniyor...</span>
    </div>
  );

  return (
    <div className="min-h-screen bg-slate-50 p-6 md:p-12">
      <ToastContainer position="bottom-right" />
      <div className="max-w-4xl mx-auto bg-white shadow-2xl rounded-3xl p-8">
        
        
        <div className="flex justify-between items-center mb-10 border-b pb-6">
            <h1 className="text-4xl font-black text-slate-800 tracking-tight italic">
              SEPETİM <span className="text-indigo-600">🛒</span>
            </h1>
            <a href="http://localhost:3000" className="flex items-center text-indigo-600 font-bold hover:text-indigo-800 transition-colors bg-indigo-50 px-4 py-2 rounded-xl">
               <ArrowLeftIcon className="h-5 w-5 mr-2" /> Vitrine Dön
            </a>
        </div>

        {basketItems.length === 0 ? (
          <div className="text-center py-20 border-4 border-dotted border-slate-100 rounded-3xl">
            <p className="text-slate-400 text-2xl font-medium mb-6">Sepetin bomboş kral... 💨</p>
            {!getUserFromToken(Cookies.get("access_token")) && (
              <p className="text-amber-600 bg-amber-50 inline-block px-4 py-2 rounded-lg font-bold mb-6">
                ⚠️ Not: Giriş yapmamış görünüyorsunuz!
              </p>
            )}
            <br />
            <a href="http://localhost:3000" className="bg-indigo-600 text-white px-8 py-3 rounded-2xl font-bold shadow-lg hover:bg-indigo-700 transition">
               Ürünlere Bak
            </a>
          </div>
        ) : (
          <div className="space-y-6">
            {basketItems.map((item) => (
              <div key={item.productId || item.ProductId} className="flex flex-col sm:flex-row justify-between items-center p-6 bg-slate-50 rounded-3xl border border-slate-100 hover:shadow-md transition-all">
                <div className="flex items-center space-x-6 w-full sm:w-auto">
                  <div className="bg-white p-2 rounded-2xl shadow-sm">
                    <img src={item.pictureUrl || "https://via.placeholder.com/150"} alt="" className="w-24 h-24 object-cover rounded-xl" />
                  </div>
                  <div>
                    <h3 className="font-extrabold text-2xl text-slate-800">{item.productName || item.ProductName}</h3>
                    <p className="text-slate-500 font-bold">{item.price} ₺ <span className="text-xs text-slate-400">x</span> {item.quantity}</p>
                  </div>
                </div>
                
                <div className="flex items-center justify-between w-full sm:w-auto mt-6 sm:mt-0 space-x-8 border-t sm:border-t-0 pt-4 sm:pt-0">
                  <span className="text-3xl font-black text-indigo-600">
                    {(item.price * item.quantity).toFixed(2)} ₺
                  </span>
                  <button onClick={() => removeItem(item.productId || item.ProductId)} className="p-4 text-red-500 hover:bg-red-100 rounded-2xl transition-all shadow-sm bg-white">
                    <TrashIcon className="h-7 w-7" />
                  </button>
                </div>
              </div>
            ))}

            
            <div className="mt-12 pt-8 border-t-4 border-slate-50 flex flex-col sm:flex-row justify-between items-center">
                <div className="mb-8 sm:mb-0 text-center sm:text-left">
                    <p className="text-slate-400 font-bold uppercase tracking-widest text-sm">Toplam Ödenecek</p>
                    <p className="text-6xl font-black text-slate-900 leading-tight">
                      {totalPrice.toFixed(2)} <span className="text-2xl text-indigo-600">₺</span>
                    </p>
                </div>
                <button className="w-full sm:w-auto bg-slate-900 text-white px-16 py-6 rounded-3xl font-black text-2xl hover:bg-green-600 transition-all shadow-2xl hover:shadow-green-200 transform hover:-translate-y-1">
                    ÖDEMEYE GEÇ 💳
                </button>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}