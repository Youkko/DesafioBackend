# Desafio Backend

Projeto criado usando .Net 6 (C#) com arquitetura de microserviços utilizando RabbitMQ como serviço de mensageria, banco de dados Postgres com Entity Framework como ORM, estruturado com Docker Compose e Dockerfiles.
A solução é composta por 3 aplicações rodando como microserviços e 1 Web API com Swagger.

## Requisitos

Para executar esse projeto, você precisará ter os seguintes softwares instalados em sua máquina:

- [Visual Studio 2022 Community](https://visualstudio.microsoft.com/pt-br/thank-you-downloading-visual-studio/?sku=Enterprise&channel=Release&version=VS2022&source=VSLandingPage&cid=2030&passive=false)
- [Docker](https://www.docker.com/products/docker-desktop/)

Opcionalmente, você pode instalar também:
- [pgAdmin 4](https://www.pgadmin.org/download/)

### Executando

- No Visual Studio
  - Selecione "docker-compose" como Startup Item e inicie.

- Command Prompt
  - Navegue até o diretório do projeto e execute o comando: `docker compose up --build`

Na primeira execução, o Docker deverá baixar e configurar todas as imagens necessárias e virtualizar o ambiente.
Se executado no Visual Studio, uma janela do navegador padrão do sistema deve surgir com a API do Swagger após a execução.

### Servidores e credenciais

Os seguintes serviços estarão acessíveis após a execução do docker compose:

- API Swagger
  - URL: [http://localhost/swagger/index.html](http://localhost/swagger/index.html)
  - Credenciais predefinidas via migrations
    - email: sysadmin@desafiobackend.com
    - senha: password@1

- RabbitMQ Management
  - URL: [http://localhost:15672/](http://localhost:15672/)
  - Credenciais predefinidas no docker compose
    - Username: rentalapp
    - Password: m07t0-R3n7@L!

- PostgreSQL
  - Acesso via pgAdmin 4
    - Hostname: localhost
    - Port: 5432
    - Username: rentalapp
    - Password: R3n7-@-m07t0!
   
## API Swagger

Ao executar a API, o primeiro endpoint que você deve executar é o de autenticação para obter um Bearer Token válido.
Esse token tem validade de 1 hora, e é necessário para acessar os endpoints protegidos, que são:

- Rentals
  - [GET] /Rentals - Listar todos os aluguéis do usuário logado
  - [GET] /Rentals/ListPlans - Listar todos os planos de aluguel disponíveis
  - [POST] /Rentals/Hire - Alugar um veículo
  - [POST] /Rentals/Return - Calcular o valor total do aluguel baseado na data de devolução

- Users
  - [POST] /Users/uploadchn - Enviar foto da CNH para registro

- Vehicles
  - [GET] /Vehicles - Listar todos os veículos cadastrados. O parâmetro opcional VIN pode ser adicionado para filtrar os resultados pela placa, filtrando todos os registros que possuam parte ou todo o valor especificado. Ex: /Vehicles?VIN=abc retorna todos os veículos que possuam "abc" na placa.
  - [POST] /Vehicles - Cadastrar um novo veículo
  - [PATCH] /Vehicles - Alterar a placa de um veículo cadastrado
  - [DELETE] /Vehicles - Excluir um veículo, desde que ele não possua registros de aluguel

Os únicos endpoints acessíveis sem necessidade de autenticação são:

- Auth
  - [POST] /Auth/login - Autenticar no sistema e obter token

- Users
  - [POST] /Users - Criar um novo usuário no sistema


O token deverá ser copiado e colado na área de autorização. Para abrir esse diálogo, clique no botão com um cadeado com rótulo **Authorize**, localizado na área superior direita da tela do Swagger.
