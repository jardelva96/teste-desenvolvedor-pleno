import React, { useState, useEffect } from 'react';
import styles from './ProductList.module.css';
import api from '../services/api';

interface ProductListProps {
    onEditProduct: (product: any) => void;
}

const ProductList: React.FC<ProductListProps> = ({ onEditProduct }) => {
    const [products, setProducts] = useState<any[]>([]);
    const [categories, setCategories] = useState<any[]>([]);
    const [error, setError] = useState<string | null>(null);
    const [loading, setLoading] = useState<boolean>(false);

    // Função para carregar categorias
    const fetchCategories = async () => {
        try {
            const response = await api.get('/Categories');
            setCategories(response.data);
        } catch (err) {
            console.error('Erro ao carregar categorias:', err);
            setError('Erro ao carregar categorias.');
        }
    };

    // Função para carregar produtos
    const fetchProducts = async () => {
        setLoading(true);
        try {
            const response = await api.get('/Products');
            setProducts(response.data);
            setError(null);  // Limpar qualquer erro anterior
        } catch (err) {
            console.error('Erro ao carregar produtos:', err);
            setError('Erro ao carregar a lista de produtos.');
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchCategories();
        fetchProducts();
    }, []);

    // Função para excluir um produto
    const handleDeleteProduct = async (id: number) => {
        try {
            await api.delete(`/Products/${id}`);
            // Atualiza a lista de produtos removendo o item deletado
            setProducts((prevProducts) => prevProducts.filter((product) => product.id !== id));
            setError(null); // Limpar erro
        } catch (err) {
            console.error('Erro ao excluir produto:', err);
            setError('Erro ao excluir produto.');
        }
    };

    return (
        <div className={styles.productListContainer}>
            <h2>Lista de Produtos</h2>
            {error && <p className={styles.errorMessage}>{error}</p>}
            {loading && <p className={styles.loadingMessage}>Carregando...</p>}

            {products.length > 0 ? (
                <table className={styles.productTable}>
                    <thead>
                        <tr>
                            <th>ID</th>
                            <th>Nome</th>
                            <th>Preço</th>
                            <th>Categoria</th>
                            <th>Ações</th>
                        </tr>
                    </thead>
                    <tbody>
                        {products.map((product) => (
                            <tr key={product.id}>
                                <td>{product.id}</td>
                                <td>{product.name}</td>
                                <td>{product.price.toFixed(2)}</td>
                                <td>{categories.find(category => category.id === product.categoryId)?.name || 'Sem categoria'}</td>
                                <td>
                                    <button onClick={() => onEditProduct(product)}>Editar</button>
                                    <button onClick={() => handleDeleteProduct(product.id)}>Excluir</button>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            ) : (
                !loading && <p className={styles.emptyMessage}>Nenhum produto encontrado.</p>
            )}
        </div>
    );
};

export default ProductList;
