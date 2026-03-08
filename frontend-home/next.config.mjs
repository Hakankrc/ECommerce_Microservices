/** @type {import('next').NextConfig} */
const nextConfig = {
  output: 'standalone',
  async rewrites() {
    return [
      {
        source: '/basket',
        destination: 'http://frontend-cart:3000/basket',
      },
      {
        source: '/basket/:path*',
        destination: 'http://frontend-cart:3000/basket/:path*',
      },
    ];
  },
};

export default nextConfig;