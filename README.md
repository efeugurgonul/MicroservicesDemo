# MicroservicesDemo

MicroservicesDemo, modern yazılım geliştirme yaklaşımlarını kullanan, .NET 8 tabanlı bir mikroservis mimarisi örneğidir. Bu proje, CQRS (Command Query Responsibility Segregation) pattern'ini kullanarak geliştirilmiş, Ocelot ile API Gateway oluşturulmuş ve JWT tabanlı kimlik doğrulama sistemi içermektedir.

## Özellikler

- **Mikroservis Mimarisi**: Her biri kendi veritabanına sahip, bağımsız çalışabilen servisler
- **CQRS Pattern**: Komut ve sorgu işlemlerinin ayrılması ile daha iyi ölçeklenebilirlik
- **API Gateway**: Ocelot ile merkezi yönlendirme ve API yönetimi
- **JWT Kimlik Doğrulama**: Güvenli ve token tabanlı kullanıcı kimlik doğrulama
- **İzin Sistemi**: Detaylı kaynak ve işlem bazlı kullanıcı izin yönetimi
- **PostgreSQL Veritabanı**: Code First yaklaşımıyla EF Core kullanımı
- **.NET 8**: Modern .NET platformunun avantajlarından yararlanma

## Proje Yapısı

MicroservicesDemo aşağıdaki projelerden oluşmaktadır:

- **Common.Core**: Tüm servisler tarafından kullanılan ortak bileşenler ve CQRS altyapısı
- **ApiGateway**: İstekleri karşılayan ve ilgili servislere yönlendiren API Gateway
- **ServerManagementService**: Kullanıcı, organizasyon ve izin yönetiminden sorumlu servis
- **ProductManagementService**: Ürün yönetiminden sorumlu servis

## Başlangıç

### Gereksinimler

- .NET 8 SDK
- PostgreSQL 15+
- Visual Studio 2022 veya JetBrains Rider

### Kurulum

1. Projeyi klonlayın:
   ```
   git clone https://github.com/your-username/MicroservicesDemo.git
   cd MicroservicesDemo
   ```

2. PostgreSQL'de veritabanlarını oluşturun:
   ```
   ServerManagementDb
   ProductManagementDb
   ```

3. `appsettings.json` dosyalarını kendi PostgreSQL bağlantı bilgilerinize göre güncelleyin.

4. Veritabanı migration'larını uygulayın:
   ```
   cd ServerManagementService
   dotnet ef database update
   
   cd ../ProductManagementService
   dotnet ef database update
   ```

5. Projeyi çalıştırın:
   ```
   cd ..
   dotnet run --project ApiGateway
   dotnet run --project ServerManagementService
   dotnet run --project ProductManagementService
   ```

### veya Visual Studio ile:

1. Solution'ı açın
2. Multiple Startup Projects'i seçin ve şu sırayla projeleri başlatacak şekilde ayarlayın:
   - ApiGateway
   - ServerManagementService
   - ProductManagementService
3. F5 ile çalıştırın

## Port Yapılandırması

- **ApiGateway**: https://localhost:5000, http://localhost:5001
- **ServerManagementService**: https://localhost:5002, http://localhost:5003
- **ProductManagementService**: https://localhost:5004, http://localhost:5005

## API Dokümantasyonu

Her servis için Swagger dokümantasyonu mevcuttur:

- ApiGateway: https://localhost:5000/swagger
- ServerManagementService: https://localhost:5002/swagger
- ProductManagementService: https://localhost:5004/swagger

## Kullanım Senaryoları

### Kullanıcı Kaydı

```http
POST /api/auth/register
Content-Type: application/json

{
  "username": "testuser",
  "email": "test@example.com",
  "password": "Test123!",
  "organizationId": 1
}
```

### Giriş Yapma ve Token Alma

```http
POST /api/auth/login
Content-Type: application/json

{
  "username": "testuser",
  "password": "Test123!"
}
```

### Yeni Organizasyon Oluşturma

```http
POST /api/organizations
Content-Type: application/json
Authorization: Bearer {token}

{
  "name": "Yeni Organizasyon",
  "description": "Organizasyon açıklaması",
  "activeStatus": 1
}
```

### Ürün Oluşturma

```http
POST /api/products
Content-Type: application/json
Authorization: Bearer {token}

{
  "name": "Örnek Ürün",
  "description": "Ürün açıklaması",
  "organizationId": 1
}
```

## Mimari Detaylar

### CQRS Yapısı

Proje, Command ve Query işlemlerini ayıran CQRS pattern'ini uygulamaktadır:

- **Command**: Veri değiştiren işlemler (Create, Update, Delete)
- **Query**: Veri okuyan işlemler (Get, List)

Her özellik (feature) kendi klasöründe ve aşağıdaki yapıda organize edilmiştir:
```
Features/
├── EntityName/
│   ├── Commands/
│   │   ├── Create/
│   │   │   ├── CreateEntityCommand.cs
│   │   │   └── CreateEntityCommandHandler.cs
│   │   ├── Update/
│   │   └── Delete/
│   └── Queries/
│       ├── List/
│       │   ├── GetAllEntitiesQuery.cs
│       │   └── GetAllEntitiesQueryHandler.cs
│       └── Detail/
```

### İzin Sistemi

Sistem, kaynak ve işlem bazlı detaylı bir izin yönetim mekanizmasına sahiptir:

- **ResourceType**: Kullanıcı, organizasyon, ürün gibi kaynak türleri
- **ActionType**: Create, Read, Update, Delete gibi işlem türleri
- **Permission**: Belirli bir kaynağa belirli bir işlemi yapma yetkisi

## Katkıda Bulunma

1. Bu repo'yu fork edin
2. Feature branch'i oluşturun (`git checkout -b feature/amazing-feature`)
3. Değişikliklerinizi commit edin (`git commit -m 'Add some amazing feature'`)
4. Branch'e push edin (`git push origin feature/amazing-feature`)
5. Pull Request açın

## Lisans

Bu proje MIT lisansı altında lisanslanmıştır. Daha fazla bilgi için `LICENSE` dosyasına bakın.

## İletişim

Proje sorumlusu - Uğur Gönül - efeugurgonul@gmail.com

Proje linki: [https://github.com/your-username/MicroservicesDemo](https://github.com/your-username/MicroservicesDemo)
