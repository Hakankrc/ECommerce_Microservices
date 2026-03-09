"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";

export default function RegisterPage() {
  const [name, setName] = useState("");
  const [surname, setSurname] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [loading, setLoading] = useState(false);
  const router = useRouter();

  const handleRegister = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");
    setSuccess("");

    if (password !== confirmPassword) {
      setError("Passwords do not match.");
      return;
    }

    setLoading(true);

    try {
      const res = await fetch("http://localhost:5153/api/Auth/register", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          name,
          surname,
          email,
          password,
        }),
      });

      if (res.ok) {
        setSuccess("Account created successfully. You can now sign in.");
        setTimeout(() => {
          router.push("/login");
        }, 1500);
      } else {
        const data = await res.json().catch(() => null);
        const message =
          (Array.isArray(data) && data[0]?.description) ||
          data?.message ||
          "Registration failed. Please check your details.";
        setError(message);
      }
    } catch {
      setError("Unable to reach the server. Please try again.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-100 p-4">
      <div className="max-w-md w-full bg-white rounded-3xl shadow-xl p-10">
        <h2 className="text-3xl font-black text-center text-indigo-900 mb-8">
          KAYIT OL
        </h2>

        {error && (
          <p className="text-red-500 text-sm mb-4 text-center font-bold">
            {error}
          </p>
        )}
        {success && (
          <p className="text-green-600 text-sm mb-4 text-center font-bold">
            {success}
          </p>
        )}

        <form onSubmit={handleRegister} className="space-y-4">
          <div className="flex gap-3">
            <input
              type="text"
              placeholder="Name"
              className="w-1/2 p-4 border border-gray-300 rounded-2xl bg-white text-black focus:ring-2 focus:ring-indigo-500 outline-none"
              value={name}
              onChange={(e) => setName(e.target.value)}
              required
            />
            <input
              type="text"
              placeholder="Surname"
              className="w-1/2 p-4 border border-gray-300 rounded-2xl bg-white text-black focus:ring-2 focus:ring-indigo-500 outline-none"
              value={surname}
              onChange={(e) => setSurname(e.target.value)}
              required
            />
          </div>

          <input
            type="email"
            placeholder="Email"
            className="w-full p-4 border border-gray-300 rounded-2xl bg-white text-black focus:ring-2 focus:ring-indigo-500 outline-none"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
          />

          <input
            type="password"
            placeholder="Password"
            className="w-full p-4 border border-gray-300 rounded-2xl bg-white text-black focus:ring-2 focus:ring-indigo-500 outline-none"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
          />

          <input
            type="password"
            placeholder="Confirm Password"
            className="w-full p-4 border border-gray-300 rounded-2xl bg-white text-black focus:ring-2 focus:ring-indigo-500 outline-none"
            value={confirmPassword}
            onChange={(e) => setConfirmPassword(e.target.value)}
            required
          />

          <button
            type="submit"
            disabled={loading}
            className="w-full bg-indigo-600 text-white font-bold py-4 rounded-2xl hover:bg-indigo-700 transition-all shadow-lg disabled:opacity-60 disabled:cursor-not-allowed"
          >
            {loading ? "Creating account..." : "Create Account"}
          </button>
        </form>
      </div>
    </div>
  );
}

