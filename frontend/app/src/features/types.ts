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