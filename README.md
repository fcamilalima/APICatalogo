# Projeto APICatalogo

## Descrição
O **APICatalogo** é uma API desenvolvida em ASP.NET Core para gerenciar um catálogo de produtos e categorias. O projeto segue as melhores práticas de desenvolvimento, incluindo padrões de projetos, segurança e testes unitários.

## Funcionalidades
- **Log e Tratamento de Erros**:
  - Implementação de logs para monitoramento e depuração.
  - Filtros globais para tratamento de exceções e respostas consistentes.

- **Padrões de Projetos**:
  - **Repository Pattern**: Abstração para acesso ao banco de dados.
  - **Unit of Work**: Gerenciamento de transações e repositórios.
  - **DTOs e Mapeamento**: Uso de Data Transfer Objects (DTOs) e mapeamento entre entidades e DTOs com métodos de extensão.

- **Recursos da API**:
  - Suporte ao verbo HTTP `PATCH` para atualizações parciais.
  - Paginação, filtros e ordenação para endpoints de listagem.
  - Programação assíncrona visando a melhoria de desempenho.

- **Segurança**:
  - Autenticação e autorização com tokens JWT.

- **Configurações Avançadas**:
  - Suporte a CORS para controle de origens permitidas.
  - Limitação de taxa de requisições (Rate Limiting).
  - Versionamento da API via Query String e URI.

- **Testes e Documentação**:
  - Testes de unidade com xUnit.
  - Documentação interativa com Swagger.

## Tecnologias Utilizadas
- **.NET 8**
- **Entity Framework Core** (MySQL)
- **xUnit** (Testes unitários)
- **FluentAssertions** (Validação em testes)
- **Swagger** (Documentação da API)
- **JWT** (Autenticação)
- **Rate Limiting** (Controle de requisições)

## Requisitos
- **SDK .NET 8** ou superior
- **MySQL** (para o banco de dados)
- **Visual Studio 2022** ou outro editor compatível

## Configuração do Projeto
1. Clone o repositório:
```
git clone git@github.com:fcamilalima/APICatalogo.git
```

## Contato
- **Autor**: Camila Lima
- **Email**: fcamilalima@gmail.com
