
export interface Product {
    id: number;     
    name: string;
    price: number;
    pictureUrl: string;
    description: string;
    stock: number; 
}


export interface BasketItem {
    productId: string;
    productName: string;
    price: number;
    quantity: number;
    pictureUrl: string;
}


export interface CustomerBasket {
    userName: string;
    items: BasketItem[];
}