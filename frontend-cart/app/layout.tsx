import type { Metadata } from "next";

import "./globals.css";
import Navbar from "@/components/Navbar";



export const metadata: Metadata = {
  title: "Microservices E-Commerce",
  description: "Hakan'ın Güçlü Mikroservis Projesi",
};

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="tr">
      <body className="antialiased font-sans selection:bg-indigo-100">
        <Navbar /> 
        {children}
      </body>
    </html>
  );
}