import { useCallback, useEffect, useRef, useState } from 'react';
import { ProductModal } from "../features/products/components/ProductModal.tsx";
import { ProductDataTable } from '../features/products/components/ProductDataTable.tsx';
import { getProducts, updateProduct } from '../features/api/products.api.ts';
import type { PaginatedProductResponse, Product } from '../features/types.ts';
import { Loader } from '../features/products/components/Loader.tsx';

export function Home() {
    const [products, setProducts] = useState<Product[]>([]);
    const [selected, setSelected] = useState<Product | null>(null);
    const [open, setOpen] = useState(false);
    const [page, setPage] = useState(1);
    const [loading, setLoading] = useState(false);
    const [hasMore, setHasMore] = useState(true);
    const loader = useRef<HTMLDivElement | null>(null);

    const loadNextPage = useCallback(() => {
        setLoading(true);
        getProducts(page).then((response: PaginatedProductResponse) => {
            setProducts(prev => {
                const newProducts = [...prev, ...response.items];

                const unique = new Map(newProducts.map(p => [p.id, p]));
                const deduped = Array.from(unique.values());

                setHasMore(deduped.length < response.totalCount);
                return deduped;
            });
            setLoading(false);
        });
    }, [page]);


    const refreshProduct = useCallback(async (updated: Product) => {
        await updateProduct(updated);
        setProducts(prev => prev.map(p => (p.id === updated.id ? updated : p)));        
    }, []);

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
    }, [hasMore, loading]);

    useEffect(() => {
        if (hasMore && !loading) {
            loadNextPage();
        }
    }, [page, loadNextPage]);



    function handleEditProduct(row: Product) {
        setSelected(row);
        setOpen(true);
    }

    return (
        <div className="p-4">
            <h1 className="text-xl font-semibold mb-4">Inventory</h1>
            <ProductDataTable
                onEditProduct={handleEditProduct}
                data={products}
            />
            <ProductModal
                product={selected}
                open={open}
                onClose={() => setOpen(false)}
                onSubmit={refreshProduct}
            />
            <Loader
                hasMore={hasMore}
                loading={loading}
                loader={loader}
            />
        </div>
    );
}

