# Mobile_AR_Tutorial

Aplicação móvel de Realidade Aumentada (AR) desenvolvida no contexto da dissertação de mestrado:

**Explorando a Realidade Aumentada na Indústria 5.0: Impacto e Usabilidade de Ferramentas para Criação de Tutoriais**

O objetivo da aplicação é permitir que usuários sem conhecimentos em programação, modelagem 3D ou desenvolvimento de aplicações de Realidade Aumentada possam criar, editar e visualizar tutoriais interativos diretamente em dispositivos móveis Android.

---

# Visão Geral

A aplicação utiliza Realidade Aumentada baseada em marcadores (*Image Targets*) para posicionar objetos virtuais sobre equipamentos reais. Esses objetos são utilizados para orientar usuários durante a execução de procedimentos operacionais, manutenção, treinamento e transferência de conhecimento.

O sistema suporta dois perfis principais de utilização:

* **Criadores de tutoriais:** responsáveis pela criação e edição das instruções.
* **Usuários finais:** responsáveis pela visualização e execução dos procedimentos.

---

# Principais Funcionalidades

## Gerenciamento de Tutoriais

* Criar tutoriais
* Editar tutoriais
* Renomear tutoriais
* Excluir tutoriais
* Filtrar tutoriais

## Gerenciamento de Tarefas

* Criar tarefas
* Editar tarefas
* Excluir tarefas
* Reordenar tarefas
* Pré-visualizar tarefas

## Objetos Virtuais

A aplicação permite a utilização dos seguintes elementos:

* Seta Direcional
* Seta Horária
* Seta Anti-horária
* Texto 3D
* Indicador de Atenção
* Indicador de Correto
* Indicador de Incorreto
* Indicador de Proibição
* Ferramentas Virtuais

Cada objeto pode ter suas propriedades configuradas individualmente:

* Posição
* Rotação
* Escala
* Cor
* Texto associado

---

# Arquitetura

```text
Usuário
   │
   ▼
Aplicação Android
(Unity + Vuforia)
   │
   ├── Banco de Dados
   │      (MongoDB Atlas)
   │
   └── Repositório de Objetos
          (Asset Bundles)
```

O banco de dados é responsável por armazenar:

* Tutoriais
* Tarefas
* Objetos virtuais
* Configurações dos objetos

O repositório de objetos virtuais armazena os modelos e recursos utilizados pela aplicação.

---

# Tecnologias Utilizadas

| Tecnologia     | Finalidade                          |
| -------------- | ----------------------------------- |
| Unity          | Desenvolvimento da aplicação        |
| C#             | Programação                         |
| Vuforia Engine | Rastreamento em Realidade Aumentada |
| MongoDB Atlas  | Banco de dados                      |
| Android        | Plataforma móvel                    |
| Blender        | Modelagem 3D                        |
| Figma          | Prototipação da interface           |
| Git            | Controle de versão                  |

---

# Estrutura do Projeto

```text
Mobile_AR_Tutorial
│
├── Assets
├── Packages
├── ProjectSettings
├── UserSettings
└── ...
```

Os diretórios mais importantes são:

### Assets

Contém:

* Scripts C#
* Prefabs
* Imagens
* Objetos 3D
* Interfaces
* Recursos de RA

### Packages

Pacotes utilizados pelo Unity:

* Vuforia Engine
* Newtonsoft Json
* Dependências do projeto

### ProjectSettings

Configurações do projeto Unity.

---

# Configuração do Banco de Dados

O projeto utiliza MongoDB Atlas.

## Passo 1

Criar uma conta em:

https://www.mongodb.com/atlas

## Passo 2

Criar um Cluster.

## Passo 3

Criar um banco de dados.

Exemplo:

```text
ARTutorials
```

## Passo 4

Criar uma coleção:

```text
Tutorials
```

## Passo 5

Criar usuário com permissão de leitura e escrita.

## Passo 6

Configurar a lista de IPs autorizados.

## Passo 7

Obter a Connection String.

Exemplo:

```text
mongodb+srv://usuario:senha@cluster.mongodb.net/
```

## Passo 8

Inserir a Connection String nos scripts responsáveis pela conexão com o banco.

---

# Configuração do Repositório de Objetos Virtuais

A aplicação realiza o download dinâmico dos objetos utilizados nos tutoriais.

Os Asset Bundles podem ser armazenados em:

* Google Drive
* Azure Blob Storage
* AWS S3
* Servidor HTTP
* Rede corporativa interna
* NAS corporativo

Após a configuração do repositório, as URLs dos objetos devem ser associadas aos registros armazenados no banco de dados.

---

# Compilação

## Requisitos

* Unity 2022 LTS ou superior
* Android SDK
* OpenJDK
* Android Build Support

## Gerar APK

No Unity:

```text
File
→ Build Settings
→ Android
→ Switch Platform
→ Build
```

## Gerar App Bundle

```text
File
→ Build Settings
→ Android
→ Build App Bundle (Google Play)
```

---

# Implantação em Produção

Uma implantação típica pode utilizar:

### Banco de Dados

* MongoDB Atlas

### Repositório de Objetos

* Azure Blob Storage
* AWS S3
* Servidor corporativo

### Clientes

* Smartphones Android

Em ambientes industriais, tanto o banco de dados quanto os objetos virtuais podem ser hospedados em servidores internos da organização, mantendo todos os dados dentro da infraestrutura corporativa.

---

# Resultados Obtidos

A aplicação foi avaliada por especialistas de domínio e usuários finais.

Resultados médios observados:

| Grupo           | SUS   | TAM  |
| --------------- | ----- | ---- |
| Especialistas   | 85,60 | 4,47 |
| Usuários Finais | 92,57 | 4,68 |

Os resultados indicaram elevados níveis de usabilidade e aceitação da tecnologia para ambos os grupos avaliados.

---

# Publicação

Este projeto originou a publicação:

**Tutoriais AR Interativos: Uma Ferramenta Simplificada para Indústria 5.0**

SBAI 2025 – Simpósio Brasileiro de Automação Inteligente.

---

# Autor

Rodrigo José de Paiva

Programa de Pós-Graduação em Instrumentação, Controle e Automação de Processos de Mineração (PROFICAM)

Universidade Federal de São João del-Rei (UFSJ)

---

# Licença

Projeto disponibilizado para fins acadêmicos e científicos.
