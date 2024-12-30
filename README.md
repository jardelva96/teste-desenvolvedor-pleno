# Teste para Desenvolvedor Full Stack Pleno (C# / React)

Bem-vindo(a) ao teste para a vaga de Desenvolvedor Full Stack Pleno! O objetivo deste teste é avaliar suas habilidades no desenvolvimento de aplicações integradas, tanto no backend quanto no frontend, utilizando **C#** e **React**.

## Desafio

O objetivo do desafio é criar uma aplicação para **gestão de produtos**. A solução deverá incluir:

- Uma **API RESTful** em **C#** para gerenciar o cadastro e operações de produtos.
- Uma **interface web** em **React** que permita interações com a API.
- A solução deve refletir boas práticas de desenvolvimento e design.

## Funcionalidades

### Backend

- **CRUD de Produtos**:
  - Implementação das operações de criação (POST), leitura (GET - individual e lista), atualização (PUT) e exclusão (DELETE) para produtos.
  - O **DELETE** deve ser lógico: produtos excluídos devem ser marcados como inativos (campo `isDeleted`), sem removê-los fisicamente do banco de dados.
  
- **Relacionamento entre Tabelas**:
  - **Categoria do Produto**: Uma tabela de categorias deve conter os campos `id`, `nome` e `descricao`. Cada produto deve estar associado a uma categoria.
  - **Fornecedor**: Uma tabela de fornecedores deve conter campos como `id`, `nome`, `cnpj`, `telefone` e `endereco`. Cada produto pode estar associado a um ou mais fornecedores.
  - **População inicial**: A tabela de categorias deve ser previamente populada com dados como "Eletrônicos", "Móveis", "Alimentos", e a tabela de fornecedores deve ser preenchida com dados fictícios.

- **Autenticação com JWT**:
  - Implementação de um mecanismo de autenticação utilizando **JWT** (JSON Web Token) para proteger os endpoints da API.
  - As credenciais são enviadas ao fazer login, e um token é gerado para autenticar as requisições subsequentes.

- **Banco de Dados**:
  - Utilização de um banco de dados **relacional** como **MySQL**, **SQL Server** ou **Informix**.

### Frontend

- **Listagem de Produtos**:
  - Exibição de uma tabela responsiva contendo os produtos cadastrados, com informações sobre categoria e fornecedores.

- **Cadastro e Edição de Produtos**:
  - Formulário para a criação e edição de produtos, com a possibilidade de selecionar categoria e fornecedores.

- **Validações**:
  - Implementação de validação básica dos campos no frontend.

- **Exclusão de Produtos**:
  - Funcionalidade para excluir produtos da listagem, marcando-os como inativos.

## Requisitos Não Obrigatórios (Extras)

- **Documentação**:
  - Documentação da API utilizando **Swagger** ou ferramenta similar.
  
- **Testes Automatizados**:
  - Backend: testes unitários e/ou de integração utilizando **xUnit**.
  - Frontend: testes unitários ou de interface com **Jest**.
  
- **Design**:
  - Melhorias na interface com boas práticas de **UX/UI**.
  
- **Desempenho**:
  - Implementação de **cache** para melhorar a performance de requisições no backend.

## Tecnologias Utilizadas

### Backend

- **C#**
- **.NET Core
- **Entity Framework Core**
- **JWT (JSON Web Token)** - para autenticação
- **SQL Server**

### Frontend

- **React** com **TypeScript**
- **React Router** (para navegação)
- **Axios** (para chamadas API)
- **React-Scripts** (para execução e build do projeto)
- **npm** (gerenciador de pacotes)

### Ferramentas de Desenvolvimento

- **Docker** (para contêineres e orquestração)
- **Swagger** (para documentação da API)
- **Postman** (para testar a API)

## Como Rodar o Projeto

### 1. Clonando o Repositório
Clone o repositório para sua máquina local:
```bash
git clone https://github.com/jardelva96/teste-desenvolvedor-pleno.git
cd teste-desenvolvedor-pleno
git checkout teste/jardel

```
2. Instalando Dependências
## Para o Frontend
Entre na pasta frontend e instale as dependências:
```bash
cd frontend
npm install --legacy-peer-deps
npm start
```
## Para o Backend
Certifique-se de ter o .NET Core SDK instalado. Navegue até a pasta backend e restaure as dependências:
```bash
cd backend
dotnet restore
dotnet run
```
3. Rodando o Projeto com Docker
- Para rodar o projeto com Docker, use o comando abaixo:
```bash
docker-compose up --build
```

- Isso criará os contêineres necessários e iniciará o projeto.

4. Acessando a Aplicação
Frontend: Acesse a aplicação através do navegador em http://localhost:3000.
Backend (API): A API estará disponível em http://localhost:5235 (products, login..)
5. Swagger (Documentação da API)
Para acessar a documentação da API, basta acessar http://localhost:5235/swagger no navegador.
6. Testando a API com Postman
Você pode testar os endpoints da API usando o Postman. A coleção de testes já está disponível no arquivo postman_api_test_collection.json. Para importar a coleção:
Abra o Postman.
Clique em Importar e selecione o arquivo postman_api_test_collection.json.
Use os endpoints da API conforme a documentação do Swagger.
