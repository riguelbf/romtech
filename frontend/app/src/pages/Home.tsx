import {useEffect, useRef, useState} from 'react';
import type {ColumnDef} from "@tanstack/react-table";
import {getProducts, type PaginatedProductResponse, type Product} from '../lib/api';
import {Button} from "../components/ui/button.tsx";
import {ProductModal} from "../components/ProductModal.tsx";
import { DataTable } from '../components/DataTable.tsx';

export function Home() {
    const [products, setProducts] = useState<Product[]>([]);
    const [selected, setSelected] = useState<Product | null>(null);
    const [open, setOpen] = useState(false);
    const [page, setPage] = useState(1);
    const [loading, setLoading] = useState(false);
    const [hasMore, setHasMore] = useState(true);
    const loader = useRef<HTMLDivElement | null>(null);

    useEffect(() => {
        const observer = new IntersectionObserver((entries: IntersectionObserverEntry[]) => {
            const entry: IntersectionObserverEntry = entries[0];
            if (entry.isIntersecting && !loading && hasMore) {
                setPage(p => p + 1);
            }
        });
        if (loader.current) {
            observer.observe(loader.current);
        }
        return () => observer.disconnect();
    }, [loading]);

    useEffect(() => {
        setLoading(true);
        getProducts(page).then((response: PaginatedProductResponse) => {
            setProducts((prev: Product[]): Product[] => [...prev, ...response.items]);
            setHasMore(products.length + response.items.length < response.totalCount);
            setLoading(false);
        });
    }, [page]);

    const columns: ColumnDef<Product>[] = [
        {
            accessorKey: 'name',
            header: 'Name',
        },
        {
            accessorKey: 'description',
            header: 'Description',
        },
        {
            accessorKey: 'stock',
            header: 'Stock',
        },
        {
            accessorKey: 'price',
            header: 'Price',
            cell: ({ row }) => `$${row.original.price.toFixed(2)}`,
        },
        {
            id: 'actions',
            cell: ({ row }) => (
                <Button
                    size="sm"
                    variant="outline"
                    onClick={() => {
                        setSelected(row.original);
                        setOpen(true);
                    }}>
                    Edit
                </Button>
            ),
        },
    ];

    return (
        <div className="p-4">
            <h1 className="text-xl font-semibold mb-4">Inventory</h1>
            <DataTable columns={columns} data={products} />
            <ProductModal product={selected} open={open} onClose={() => setOpen(false)} />
            <div ref={loader} className="text-center py-4 text-sm text-muted-foreground">
                {hasMore ? (loading ? 'Loading more products...' : 'Scroll down to load more') : 'No more products to load'}
            </div>
        </div>
    );
}