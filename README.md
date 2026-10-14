# SGA AutoElétrica — Sistema de Gestão para Autoelétricas

Sistema desktop desenvolvido em C# e WPF para auxiliar no gerenciamento das operações de uma autoelétrica. A aplicação reúne funcionalidades de cadastro, controle de estoque, gerenciamento de serviços e ordens de serviço em um único ambiente.

O projeto faz parte do meu portfólio de desenvolvimento de software e foi desenvolvido considerando necessidades de uso real, organização arquitetural e manutenção da aplicação.

## Funcionalidades

### Gestão de cadastros

- Cadastro e consulta de clientes e veículos.
- Gerenciamento de fornecedores.
- Cadastro de peças, marcas e categorias.
- Cadastro e gerenciamento de serviços.
- Pesquisa e consulta de registros.

### Estoque

- Gerenciamento de peças.
- Registro de movimentações e ajustes de estoque.
- Organização de peças por marcas e categorias.

### Ordens de serviço

- Criação e gerenciamento de ordens de serviço.
- Associação de clientes e veículos aos atendimentos.
- Inclusão de peças e serviços nas ordens.
- Cálculo de valores e aplicação de descontos.
- Atualização de status e observações.
- Registro e acompanhamento de quilometragem.
- Consulta detalhada das ordens de serviço.

### Dashboard e acompanhamento operacional

- Visualização de indicadores financeiros e operacionais.
- Resumos por período mensal e anual.
- Consulta de informações relacionadas ao faturamento.

### Administração e configurações

- Criação do primeiro usuário administrador.
- Autenticação de usuários.
- Gerenciamento de usuários ativos e inativos.
- Alteração de dados do perfil e senha.
- Configuração dos dados da empresa.
- Configuração de impressão.
- Registro e consulta de notas fiscais de entrada.
- Recursos de backup manual e solicitação de backup automático no encerramento da aplicação.

## Tecnologias utilizadas

| Tecnologia | Aplicação |
|---|---|
| C# | Linguagem principal |
| .NET 10 | Plataforma de desenvolvimento |
| WPF | Construção da interface desktop |
| SQLite | Banco de dados local |
| Entity Framework Core | Mapeamento objeto-relacional e migrações |
| MediatR | Mediação de comandos e consultas |
| FluentValidation | Validação de dados |
| AutoMapper | Mapeamento de objetos e DTOs |
| Microsoft.Extensions.DependencyInjection | Injeção de dependências |
| WiX Toolset 7 | Configuração do instalador Windows |

## Arquitetura do projeto

A solução é dividida em projetos com responsabilidades específicas, buscando separar as regras de negócio, os casos de uso, a persistência e a interface gráfica.

```text
SgaAutoEletrica/
├── SgaAutoEletrica.slnx
└── src/
    ├── SgaAutoEletrica.UI/
    ├── SgaAutoEletrica.Application/
    ├── SgaAutoEletrica.Domain/
    ├── SgaAutoEletrica.Infrastructure/
    ├── SgaAutoEletrica.Migrator/
    ├── SgaAutoEletrica.Installer/
    └── publish/          # gerado por dotnet publish (não versionado)
```

### Responsabilidades das camadas

**Domain**

Contém as entidades, os enums e os value objects que representam conceitos do domínio da aplicação.

**Application**

Organiza os casos de uso, os comandos, as consultas, os DTOs, os contratos e as regras de aplicação.

**Infrastructure**

Implementa o acesso aos dados, os repositórios, os serviços de infraestrutura e o registro de dependências dos componentes dessa camada.

**UI**

Contém a interface gráfica desenvolvida em WPF, as telas, os ViewModels, os conversores e a inicialização da aplicação.

**Migrator**

Projeto destinado às operações de migração e configuração inicial do banco de dados.

**Installer**

Contém a configuração do instalador Windows utilizando WiX Toolset.

### Padrões e mecanismos utilizados

- Separação de responsabilidades por camadas.
- MediatR para mediação de solicitações e handlers.
- Abstrações de repositórios e unidade de trabalho.
- Injeção de dependências para composição dos serviços.
- DTOs para transporte de dados entre componentes.
- Entity Framework Core para persistência e gerenciamento de migrações.

## Persistência e inicialização do banco de dados

O sistema utiliza SQLite para armazenar os dados localmente.

Durante a inicialização, a aplicação:

1. Configura o ambiente regional para português do Brasil (`pt-BR`).
2. Registra os serviços e as dependências necessárias.
3. Cria o diretório de dados, caso ainda não exista.
4. Configura o acesso ao banco SQLite.
5. Aplica as migrações pendentes do Entity Framework Core.
6. Verifica se existem usuários cadastrados para direcionar o fluxo de primeiro acesso ou login.

### Estrutura de diretórios em tempo de execução

Após a instalação, a aplicação utiliza os seguintes diretórios em `%ProgramData%`:

```text
%ProgramData%\SgaAutoEletrica\
├── Data\
│   ├── sga.db              # banco de dados SQLite
│   └── Backups\            # cópias de segurança do banco
├── Config\                 # arquivos de configuração da aplicação
└── Logs\                   # arquivos de log
```

### Banco de dados

O caminho padrão configurado para o banco de dados é:

```text
%ProgramData%\SgaAutoEletrica\Data\sga.db
```

A estrutura do banco é definida pelo `AppDbContext` e pelas configurações de entidades na camada `Infrastructure`.

As migrações do Entity Framework Core ficam em:

```text
src/SgaAutoEletrica.Infrastructure/Persistence/Migrations/
```

### Gerando uma migração

Para criar uma nova migração durante o desenvolvimento:

```bash
dotnet ef migrations add NomeDaMigracao --project src/SgaAutoEletrica.Infrastructure/SgaAutoEletrica.Infrastructure.csproj --startup-project src/SgaAutoEletrica.UI/SgaAutoEletrica.UI.csproj
```

O comando pressupõe que a ferramenta `dotnet-ef` e as dependências necessárias estejam instaladas e configuradas no ambiente.

## Como executar

### Pré-requisitos

- Windows 10 ou 11.
- .NET 10 SDK.
- Git.
- Visual Studio com suporte ao desenvolvimento desktop com .NET e WPF, ou ambiente compatível.

> **Nota:** o projeto utiliza .NET 10. Certifique-se de ter o SDK correspondente instalado antes de compilar.

### Clonando o repositório

```bash
git clone https://github.com/BrugithM/AutoEletrica.git
cd AutoEletrica
```

### Restaurando as dependências

```bash
dotnet restore SgaAutoEletrica.slnx
```

### Compilando a solução

```bash
dotnet build SgaAutoEletrica.slnx
```

### Executando a aplicação

```bash
dotnet run --project src/SgaAutoEletrica.UI/SgaAutoEletrica.UI.csproj
```

Alternativamente, abra `SgaAutoEletrica.slnx` no Visual Studio e configure `SgaAutoEletrica.UI` como projeto de inicialização.

A execução depende da configuração correta do ambiente e das dependências do projeto. O banco é configurado e as migrações são aplicadas pela própria aplicação durante a inicialização.

## Instalador Windows

O repositório contém um projeto de instalação baseado no WiX Toolset 7, configurado para Windows x64.

A configuração inclui:

- Publicação **single-file** e **self-contained** (não requer .NET instalado na máquina de destino).
- Diretório de instalação em `Program Files`.
- Atalhos no Menu Iniciar e na Área de Trabalho.
- Ícone personalizado do aplicativo.
- Termos de uso apresentados durante a instalação.
- Criação dos diretórios de dados, configurações e logs em `%ProgramData%`.
- Permissões de escrita para os usuários nas pastas de dados.
- Interface de instalação com seleção do diretório.
- Atualização automática de versões anteriores.

### Gerando o instalador

```bash
# 1. Publicar o app (single-file, self-contained)
cd src/SgaAutoEletrica.UI
dotnet publish -c Release -o ../publish
cd ../..

# 2. (Opcional, mas recomendado) Limpar o cache do instalador
rm -rf src/SgaAutoEletrica.Installer/obj

# 3. Compilar o instalador
cd src/SgaAutoEletrica.Installer
dotnet build -c Release
cd ../..
```

O instalador gerado estará em:

```text
src/SgaAutoEletrica.Installer/bin/Release/SgaAutoEletrica_Setup.msi
```

> **Importante:** o projeto WiX reutiliza arquivos do cache de build. Ao republicar o
> aplicativo, recomenda-se limpar a pasta `obj` do instalador antes de compilar, para
> garantir que o `.msi` incorpore a versão mais recente do executável.

### Requisitos para execução do instalador

- Windows 10 ou 11 (64 bits).
- Privilégios de administrador durante a instalação (`Scope="perMachine"`).

> **Observação:** como o instalador não é assinado digitalmente, o Windows pode exibir
> um aviso do SmartScreen na primeira execução. Basta clicar em **"Mais informações"**
> e em **"Executar mesmo assim"** para prosseguir.

## Backup e recuperação de dados

O sistema possui funcionalidades para proteger os dados armazenados no banco SQLite:

- **Backup manual:** permite criar uma cópia do banco de dados em um diretório escolhido ou no diretório padrão de backups.
- **Backup automático:** cria uma cópia no diretório padrão e remove os backups excedentes, mantendo até 10 arquivos conforme a ordenação utilizada pelo sistema.
- **Listagem de backups:** disponibiliza os arquivos de backup existentes no diretório padrão.
- **Restauração:** permite substituir o banco de dados atual por um arquivo de backup selecionado.

Por padrão, os backups são armazenados em `Backups`, dentro do diretório de dados do sistema:

```text
%ProgramData%\SgaAutoEletrica\Data\Backups
```

Os arquivos seguem o padrão de nomenclatura `backup_YYYYMMDD_HHMMSS.db`.

> **Importante:** recomenda-se manter cópias adicionais em outro local para reduzir o risco de perda de dados.

## Status do projeto

O SGA AutoElétrica está em desenvolvimento e contempla funcionalidades voltadas à gestão operacional de uma autoelétrica.

A aplicação é utilizada inicialmente em um contexto real de operação e poderá receber correções, melhorias e novas funcionalidades conforme as necessidades identificadas.

## Direitos autorais e utilização do código

Copyright © 2026 BrugithM. Todos os direitos reservados, observada a titularidade dos direitos aplicáveis.

Este repositório é disponibilizado publicamente para apresentação do projeto e avaliação técnica.

A publicação do código não concede, por si só, autorização geral para copiar, modificar, redistribuir ou explorar comercialmente o software. Bibliotecas e componentes de terceiros permanecem sujeitos às respectivas licenças.

---

Desenvolvido por BrugithM.

[Repositório no GitHub](https://github.com/BrugithM/AutoEletrica)