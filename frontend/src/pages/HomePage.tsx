import React, { useState } from 'react';
import styles from './HomePage.module.css';
import ProductList from '../components/ProductList';
import ProductForm from '../components/ProductForm';

const HomePage: React.FC = () => {
    const [editingProduct, setEditingProduct] = useState<any | null>(null); // Gerencia o estado do produto em edição

    const handleEditProduct = (product: any) => {
        // Define o produto que será editado
        setEditingProduct(product);
    };

    const handleClearEditing = () => {
        // Limpa o estado de edição
        setEditingProduct(null);
    };

    return (
        <div className={styles.homePage}>
            <h1>Gestão de Produtos</h1>
            {/* Passa o estado e manipuladores para os componentes */}
            <ProductForm editingProduct={editingProduct} onClearEditing={handleClearEditing} />
            <ProductList onEditProduct={handleEditProduct} />
        </div>
    );
};

export default HomePage;
