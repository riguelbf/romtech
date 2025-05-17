import { useCallback, useEffect, useRef, useState } from 'react';
import { ProductModal } from "../features/products/components/ProductModal.tsx";
import { ProductDataTable } from '../features/products/components/ProductDataTable.tsx';
import { getProducts } from '../features/api/products.api.ts';
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

    const refreshTable = useCallback(() => {
        getProducts(page).then((response: PaginatedProductResponse) => {
            setProducts((prev: Product[]): Product[] => [...prev, ...response.items]);
            setHasMore(products.length + response.items.length < response.totalCount);
            setLoading(false);
        });
    }, [page, products, setProducts, setHasMore, setLoading]);

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
        console.log('useEffect triggered');
        setLoading(true);
        refreshTable();
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [page]);


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
                onCallback={() => refreshTable()}
            />
            <Loader
                hasMore={hasMore}
                loading={loading}
                loader={loader}
            />
        </div>
    );
}

