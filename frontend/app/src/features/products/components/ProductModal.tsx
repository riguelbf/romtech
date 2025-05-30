
import React, { useEffect, useState } from 'react';
import { Dialog, DialogContent, DialogDescription, DialogTitle } from '../../../components/ui/dialog.tsx';
import { Input } from "../../../components/ui/input.tsx";
import { Button } from '../../../components/ui/button.tsx';

import type { Product } from '../../types.ts';
import { toast } from 'react-toastify';

export function ProductModal({ product, open, onClose, onSubmit }: {
    product: Product | null;
    open: boolean;
    onClose: () => void;
    onSubmit: (product: Product) => Promise<void>;
}) {
    const [form, setForm] = useState(product ?? { id: '', name: '', description: '', stock: 0, price: 0 });

    useEffect(() => {
        if (product) {
            setForm(product);
        }
    }, [product]);

    if (!product) return null;

    function handleChange(e: React.ChangeEvent<HTMLInputElement>) {
        const { name, value } = e.target;
        setForm({ ...form, [name]: name === 'stock' || name === 'price' ? Number(value) : value });
    }

    async function handleSubmit() {
        try {
            const productUpdated: Product = {
                id: Number(form.id),
                name: form.name,
                description: form.description,
                price: form.price,
                stock: form.stock
            }

            await onSubmit(productUpdated);

            toast.success('Product saved successfully!', {
                autoClose: 2000,
            });

            onClose();
        } catch (err) {
            console.error('Error updating product:', err);
            toast.error('Error saving product!');
        }
    }

    return (
        <Dialog open={open} onOpenChange={onClose}>
            <DialogContent className="max-w-md w-full" aria-describedby="Product details">
                <DialogDescription>Product details</DialogDescription>
                <DialogTitle>Edit Product</DialogTitle>
                <div className="space-y-4">
                    <Input name="name" value={form.name} onChange={handleChange} placeholder="Name" />
                    <Input name="description" value={form.description} onChange={handleChange} placeholder="Description" />
                    <Input name="stock" type="number" value={form.stock} onChange={handleChange} placeholder="Stock" />
                    <Input name="price" type="number" step="0.01" value={form.price} onChange={handleChange} placeholder="Price" />
                    <div className="flex justify-end gap-2 pt-4">
                        <Button variant="secondary" onClick={onClose}>Cancel</Button>
                        <Button onClick={handleSubmit}>Save changes</Button>
                    </div>
                </div>
            </DialogContent>
        </Dialog>
    );
}