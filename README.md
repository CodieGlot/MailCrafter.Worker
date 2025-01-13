
# MailCrafter - Worker Service

This repository contains the **Worker Service** for the MailCrafter system, built using **.NET Worker Service**. The Worker Service handles background processing tasks such as email sending, template personalization, and data handling.

## Features
- **Background Task Processing**: Consumes tasks from the **RabbitMQ** queue and processes them asynchronously.
- **Email Sending**: Retrieves email templates, personalizes content, and sends emails to recipients.
- **Auto-Scaling**: Worker nodes are managed by **Kubernetes** for auto-scaling based on workload demands.

## Architecture
- Built with **.NET Worker Service** for background processing.
- Consumes tasks from the **RabbitMQ** queue, processing them efficiently.
- Communicates with the **Core Layer** to retrieve data and perform business logic.
- Works alongside the **Application Layer** to perform tasks asynchronously.
- Interfaces with **MongoDB** for retrieving and updating data.

## Setup and Installation
### Prerequisites:
- **.NET 8 or later**
- **RabbitMQ** instance
- **Kubernetes** for auto-scaling (optional)
- **MongoDB** for data storage

### Installation Steps:
1. Clone this repository.
   ```bash
   git clone https://github.com/CodieGlot/MailCrafter.Worker.git
   ```
2. Restore dependencies.
   ```bash
   dotnet restore
   ```
3. Set up RabbitMQ and configure MongoDB.
4. Deploy the service to **Kubernetes** (optional for auto-scaling).
5. Run the worker service.
   ```bash
   dotnet run
   ```

## Usage
- The worker nodes will automatically consume tasks from the RabbitMQ queue and process them as they arrive.
- Monitor the worker's performance via **Kubernetes** (if used) for scaling and logging.

## License
MIT License. See LICENSE file for details.
