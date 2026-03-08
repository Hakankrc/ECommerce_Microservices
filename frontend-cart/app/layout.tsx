import type { Metadata } from "next";
import { Inter } from "next/font/google";
import "./globals.css";
import Navbar from "@/components/Navbar";

const inter = Inter({ subsets: ["latin"] });

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
      <body className={inter.className}>
        <Navbar /> {/* Navbar her zaman en tepede! */}
        {children}
      </body>
    </html>
  );
}