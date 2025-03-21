# API de Conversão e Análise de Arquivos

Esta é uma API desenvolvida em .NET 8 para conversão e análise de arquivos. Ela permite converter arquivos nos formatos `.md`, `.pdf` e `.docx`, armazená-los no S3 e também realizar análises utilizando a IA Gemini.

## Endpoints

### 1. Converter Arquivo

**Rota:** `POST /api/drive-management/v1/file/convert`

**Descrição:**
Recebe um arquivo no formato `.md`, `.pdf` ou `.docx`, converte-o para os outros dois formatos e retorna um arquivo ZIP contendo as conversões. Além disso, o arquivo original e os convertidos são armazenados no Amazon S3.

**Parâmetros:**
- `file` (multipart/form-data): O arquivo a ser convertido.

**Resposta:**
- `200 OK`: Retorna um arquivo ZIP contendo as versões convertidas.
- `400 Bad Request`: Caso o formato do arquivo não seja suportado.

### 2. Analisar Arquivo

**Rota:** `POST /api/drive-management/v1/file/analyze`

**Descrição:**
Recebe um nome de arquivo já armazenado e uma pergunta sobre seu conteúdo. Utiliza a IA Gemini para analisar e fornecer uma resposta baseada no conteúdo do documento.

**Parâmetros:**
- `fileName` (string, body JSON): O nome do arquivo a ser analisado.
- `question` (string, body JSON): A pergunta sobre o conteúdo do arquivo.

**Resposta:**
- `200 OK`: Retorna a resposta gerada pela IA com base no conteúdo do arquivo.
- `404 Not Found`: Caso o arquivo não seja localizado no S3.

## Tecnologias Utilizadas
- **.NET 8**: Framework principal para desenvolvimento da API.
- **Amazon S3**: Armazenamento dos arquivos originais e convertidos.
- **Gemini AI**: Análise de arquivos baseada em IA.

## Como Rodar o Projeto

1. Clone este repositório:
   ```sh
   git clone https://github.com/leolrccv/Drive.Management.System.git
   ```
2. Configure as credenciais do Amazon S3 e Gemini AI no `appsettings.json`.
3. Execute a API:
   ```sh
   dotnet run
   ```
---
