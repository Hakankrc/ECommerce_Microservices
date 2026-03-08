"use client";

import { useEffect, useState } from "react";
import axios from "axios";
import { Product, CustomerBasket } from "@/types"; 
import { toast, ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';

export default function Home() {
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(true);

  // Fetch products through the Gateway
  const fetchProducts = async () => {
    try {
      const response = await axios.get("http://localhost:5153/api/product");
      setProducts(response.data);
    } catch (err) {
      console.error("Ürünler çekilemedi:", err);
    } finally {
      setLoading(false);
    }
  };

  // Smart Add to Basket (Fetches current basket and adds on top)
  const addToBasket = async (product: Product) => {
    try {
      
      const currentRes = await axios.get("http://localhost:5153/api/Basket/Hakan-123");
      let currentItems = currentRes.data.items || currentRes.data.Items || [];

      
      const existingItem = currentItems.find((i: any) => (i.productId || i.ProductId) === product.id.toString());

      if (existingItem) {
        
        if (existingItem.quantity !== undefined) existingItem.quantity += 1;
        if (existingItem.Quantity !== undefined) existingItem.Quantity += 1;
      } else {
        
        currentItems.push({
          ProductId: product.id.toString(),
          ProductName: product.name,
          Price: product.price,
          Quantity: 1,
          PictureUrl: product.pictureUrl
        });
      }

      // Send the updated list
      await axios.post("http://localhost:5153/api/Basket", {
        UserName: "Hakan-123",
        Items: currentItems
      });

      toast.success(`${product.name} sepete eklendi! 🛒`);
      // Dispatch basket updated event
      window.dispatchEvent(new Event("basketUpdated"));
      
    } catch (err) {
      console.error("Ekleme hatası:", err);
      toast.error("Sepete eklenirken hata oluştu.");
    }
  };

  useEffect(() => {
    fetchProducts();
  }, []);

  return (
    <main className="min-h-screen bg-gray-50 p-10">
      <ToastContainer /> 
      <div className="max-w-7xl mx-auto">
        <h1 className="text-4xl font-black mb-12 text-center text-indigo-900 tracking-tighter">
          🔥 MICROSERVICES SHOWCASE
        </h1>

        {loading ? (
          <div className="text-center py-20 animate-pulse text-indigo-600 font-bold">Yükleniyor...</div>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-10">
            {products.map((product) => (
              <div key={product.id} className="bg-white rounded-3xl shadow-sm hover:shadow-xl transition-all border border-gray-100 overflow-hidden group">
                <div className="h-64 overflow-hidden bg-gray-100">
                   <img src={product.pictureUrl} alt={product.name} className="h-full w-full object-cover group-hover:scale-110 transition-transform duration-500" />
                </div>
                <div className="p-8">
                  <h2 className="text-2xl font-bold mb-3 text-gray-800">{product.name}</h2>
                  <p className="text-gray-500 mb-8 text-sm leading-relaxed">{product.description}</p>
                  <div className="flex justify-between items-center">
                    <span className="text-3xl font-black text-indigo-600">{product.price} ₺</span>
                    <button onClick={() => addToBasket(product)} className="bg-indigo-600 text-white font-bold py-3 px-8 rounded-2xl hover:bg-indigo-700 transition-colors shadow-lg shadow-indigo-100">
                      Ekle
                    </button>
                  </div>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </main>
  );
}