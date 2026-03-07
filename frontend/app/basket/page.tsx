"use client";

import { useEffect, useState } from "react";
import axios from "axios";
import { toast, ToastContainer } from "react-toastify";
import { TrashIcon, PlusIcon, MinusIcon } from "@heroicons/react/24/outline";
import Link from "next/link";
import 'react-toastify/dist/ReactToastify.css';

export default function BasketPage() {
  const [basket, setBasket] = useState<any>(null);
  const [loading, setLoading] = useState(true);

  const fetchBasket = async () => {
    try {
      const response = await axios.get("http://localhost:5153/api/Basket/Hakan-123");
      // Büyük/Küçük harf sigortası
      const normalized = {
        UserName: response.data.userName || response.data.UserName,
        Items: response.data.items || response.data.Items || []
      };
      setBasket(normalized);
    } catch (error) {
      console.error("Sepet hatası:", error);
    } finally {
      setLoading(false);
    }
  };

  const updateBasket = async (updatedItems: any[]) => {
    try {
      const payload = { UserName: "Hakan-123", Items: updatedItems };
      await axios.post("http://localhost:5153/api/Basket", payload);
      setBasket(payload);
      window.dispatchEvent(new Event("basketUpdated"));
    } catch (error) {
      toast.error("Güncellenemedi.");
    }
  };

  const changeQuantity = (productId: string, delta: number) => {
    const newItems = [...basket.Items];
    const item = newItems.find((i: any) => (i.productId || i.ProductId) === productId);
    if (item) {
      const currentQty = item.quantity !== undefined ? item.quantity : item.Quantity;
      item.Quantity = currentQty + delta;
      item.quantity = currentQty + delta; // İki tarafı da güncelle

      if (item.Quantity <= 0) {
        removeItem(productId);
      } else {
        updateBasket(newItems);
      }
    }
  };

  const removeItem = (productId: string) => {
    const newItems = basket.Items.filter((i: any) => (i.productId || i.ProductId) !== productId);
    updateBasket(newItems);
    toast.info("Ürün çıkarıldı.");
  };

  useEffect(() => { fetchBasket(); }, []);

  if (loading) return <div className="p-20 text-center font-bold">Yükleniyor...</div>;

  const items = basket?.Items || [];
  const totalPrice = items.reduce((sum: number, i: any) => sum + (i.price || i.Price) * (i.quantity || i.Quantity), 0);

  return (
    <div className="min-h-screen bg-gray-50 py-12 px-4">
      <ToastContainer />
      <div className="max-w-5xl mx-auto">
        <h1 className="text-3xl font-black text-gray-900 mb-8">🛒 Sepetim</h1>
        {items.length === 0 ? (
          <div className="bg-white p-12 rounded-3xl shadow-sm text-center border-2 border-dashed">
            <p className="text-gray-500 mb-6">Sepetin boş kral!</p>
            <Link href="/" className="bg-indigo-600 text-white px-8 py-3 rounded-xl font-bold">Vitrine Dön</Link>
          </div>
        ) : (
          <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
            <div className="lg:col-span-2 space-y-4">
              {items.map((item: any) => (
                <div key={item.productId || item.ProductId} className="bg-white p-5 rounded-3xl shadow-sm flex items-center space-x-6 border">
                  <img src={item.pictureUrl || item.PictureUrl} className="h-20 w-20 object-cover rounded-2xl" />
                  <div className="flex-1">
                    <h3 className="font-bold text-gray-800">{item.productName || item.ProductName}</h3>
                    <p className="text-indigo-600 font-bold">{item.price || item.Price} ₺</p>
                  </div>
                  <div className="flex items-center bg-gray-100 rounded-xl p-1">
                    <button onClick={() => changeQuantity(item.productId || item.ProductId, -1)} className="p-2 hover:text-red-500"><MinusIcon className="h-5 w-5" /></button>
                    <span className="font-bold w-8 text-center">{item.quantity || item.Quantity}</span>
                    <button onClick={() => changeQuantity(item.productId || item.ProductId, 1)} className="p-2 hover:text-green-500"><PlusIcon className="h-5 w-5" /></button>
                  </div>
                  <button onClick={() => removeItem(item.productId || item.ProductId)} className="text-gray-300 hover:text-red-500 p-2"><TrashIcon className="h-6 w-6" /></button>
                </div>
              ))}
            </div>
            <div className="bg-white p-8 rounded-3xl shadow-xl h-fit sticky top-24">
              <h2 className="text-xl font-bold mb-6">Özet</h2>
              <div className="text-3xl font-black text-indigo-600 mb-8">{totalPrice} ₺</div>
              <button className="w-full bg-indigo-600 text-white font-bold py-4 rounded-2xl shadow-lg">ÖDEMEYE GEÇ</button>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}