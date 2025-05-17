import type { PaginatedProductResponse, Product } from "../types";

export async function getProducts(pageNumber: number, pageSize = 2): Promise<PaginatedProductResponse> {
    try {
        const res = await fetch(`http://localhost:5179/api/v1/products?pageNumber=${pageNumber}&pageSize=${pageSize}`);
        
        if (!res.ok) { 
            throw new Error(`Error fetching products: ${res.status} ${res.statusText}`);
        }
        
        return res.json();
    } catch (err) {
        console.error('Error fetching products:', err);
        throw err;
    }
}

export async function updateProduct(product: Product): Promise<void> {
    try {
        const res = await fetch(`http://localhost:5179/api/v1/products/${product.id}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(product),
        });

        if (!res.ok) {
            throw new Error(`Error updating product: ${res.status} ${res.statusText}`);
        }
    } catch (err) {
        console.error('Error updating product:', err);
        throw err;
    }
}
