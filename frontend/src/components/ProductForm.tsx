import React, { useState, useEffect } from 'react';
import styles from './ProductForm.module.css';
import api from '../services/api';

interface ProductFormProps {
    editingProduct: any | null;
    onClearEditing: () => void;
}

const ProductForm: React.FC<ProductFormProps> = ({ editingProduct, onClearEditing }) => {
    const [name, setName] = useState<string>('');
    const [price, setPrice] = useState<number | ''>('');
    const [categoryId, setCategoryId] = useState<number | ''>('');
    const [categories, setCategories] = useState<any[]>([]);
    const [suppliers, setSuppliers] = useState<any[]>([]);
    const [selectedSuppliers, setSelectedSuppliers] = useState<number[]>([]);
    const [error, setError] = useState<string | null>(null);
    const [loading, setLoading] = useState<boolean>(false);

    useEffect(() => {
        const fetchData = async () => {
            try {
                const [categoriesResponse, suppliersResponse] = await Promise.all([
                    api.get('/Categories'),
                    api.get('/Suppliers'),
                ]);
                setCategories(categoriesResponse.data);
                setSuppliers(suppliersResponse.data);
            } catch (err) {
                console.error('Erro ao carregar dados:', err);
                setError('Erro ao carregar categorias ou fornecedores.');
            }
        };

        fetchData();
    }, []);

    useEffect(() => {
        if (editingProduct) {
            setName(editingProduct.name || '');
            setPrice(editingProduct.price || '');
            setCategoryId(editingProduct.categoryId || '');
            setSelectedSuppliers(
                editingProduct.productSuppliers?.map((supplier: any) => supplier.suppliersId) || []
            );
        } else {
            resetForm();
        }
    }, [editingProduct]);

    const resetForm = () => {
        setName('');
        setPrice('');
        setCategoryId('');
        setSelectedSuppliers([]);
        setError(null);
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        if (!name || price === '' || categoryId === '' || selectedSuppliers.length === 0) {
            setError('Preencha todos os campos e selecione pelo menos um fornecedor.');
            return;
        }

        const product = {
            name,
            price: parseFloat(price.toString()),
            categoryId: parseInt(categoryId.toString(), 10),
            productSuppliers: selectedSuppliers.map((id) => ({ suppliersId: id })),
        };

        setLoading(true);

        try {
            if (editingProduct) {
                const response = await api.put(`/Products/${editingProduct.id}`, product);
                if (response.status === 200) {
                    alert('Produto atualizado com sucesso!');
                }
            } else {
                const response = await api.post('/Products', product);
                if (response.status === 201) {
                    alert('Produto adicionado com sucesso!');
                }
            }
            onClearEditing();
            resetForm();
        } catch (err) {
            console.error('Erro ao salvar produto:', err);
            setError('Erro ao salvar o produto.');
        } finally {
            setLoading(false);
        }
    };

    const handleSupplierSelection = (supplierId: number) => {
        if (selectedSuppliers.includes(supplierId)) {
            setSelectedSuppliers(selectedSuppliers.filter((id) => id !== supplierId));
        } else {
            setSelectedSuppliers([...selectedSuppliers, supplierId]);
        }
    };

    return (
        <form className={styles.productForm} onSubmit={handleSubmit}>
            <h2>{editingProduct ? 'Editar Produto' : 'Adicionar Produto'}</h2>
            {error && <p className={styles.errorMessage}>{error}</p>}
            {loading && <p className={styles.loadingMessage}>Carregando...</p>}

            <input
                type="text"
                placeholder="Nome"
                value={name}
                onChange={(e) => setName(e.target.value)}
                required
            />
            <input
                type="number"
                placeholder="Preço"
                value={price}
                onChange={(e) => setPrice(e.target.value ? parseFloat(e.target.value) : '')}
                required
            />
            <select
                value={categoryId}
                onChange={(e) => setCategoryId(parseInt(e.target.value, 10))}
                required
            >
                <option value="">Selecione uma categoria</option>
                {categories.map((category) => (
                    <option key={category.id} value={category.id}>
                        {category.name}
                    </option>
                ))}
            </select>

            <fieldset>
                <legend>Selecionar Fornecedores</legend>
                {suppliers.map((supplier) => (
                    <label key={supplier.id}>
                        <input
                            type="checkbox"
                            checked={selectedSuppliers.includes(supplier.id)}
                            onChange={() => handleSupplierSelection(supplier.id)}
                        />
                        {supplier.name}
                    </label>
                ))}
            </fieldset>

            <button type="submit" disabled={loading}>
                {editingProduct ? 'Atualizar' : 'Salvar'}
            </button>
            {editingProduct && (
                <button type="button" onClick={onClearEditing} disabled={loading}>
                    Cancelar
                </button>
            )}
        </form>
    );
};

export default ProductForm;
