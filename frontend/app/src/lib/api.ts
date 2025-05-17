export type Product = {
    id: number
    name: string
    description: string
    price: number
    stock: number
}

export type PaginatedProductResponse = {
    items: Product[]
    totalCount: number
    pageNumber: number
    pageSize: number
}

export async function getProducts(pageNumber: number, pageSize = 2): Promise<PaginatedProductResponse> {
    const res = await fetch(`http://localhost:5179/api/v1/products?pageNumber=${pageNumber}&pageSize=${pageSize}`);
    
    if (!res.ok) { 
        throw new Error(`Error fetching products: ${res.status} ${res.statusText}`); 
    }
    
    return res.json();
}

export async function updateProduct(product: Product): Promise<void> {
    const res = await fetch(`http://localhost:5179/api/v1/products/${product.id}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(product),
    })
    
    if (!res.ok) { 
        throw new Error(`Error updating product: ${res.status} ${res.statusText}`);
    }
    
    return res.json();
}