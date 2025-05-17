import type { ColumnDef } from "@tanstack/react-table";
import { DataTable } from "../../../components/DataTable";
import type { Product } from "../../types";
import { Button } from "../../../components/ui/button";

export function ProductDataTable({ data, onEditProduct }: { data: Product[], onEditProduct: (row: Product) => void }) {

    const columns: ColumnDef<Product>[] = [
        {
            accessorKey: 'id',
            header: 'Code',
        },
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
                    onClick={() => onEditProduct(row.original)}>
                    Edit
                </Button>
            ),
        },
    ];

    return <DataTable columns={columns} data={data} />;
}