# Food Ordering REST API - Burger Bangor 🍔

Homework 5, Lanjut dari homwork 4 (console app), sekarang diubah jadi REST API pakai ASP.NET Core + SQL Server. Tema: **BURGER**.

Yang dikelola ada 2 hal: **kategori** (Burger, Paket, Minuman) dan **menu** (Item dari kategorinya).

## environment
- .NET 10 (ASP.NET Core Web API)
- SQL Server, diakses pakai **ADO.NET (`Microsoft.Data.SqlClient`) dengan query SQL**.
- Scalar buat dokumentasi

## how to run it
1. Pastiin SQL Server udah nyala.
2. Buka `Database/schema.sql` di SSMS, terus run. Script ini bikin database `FoodOrderingDb`, tabel, constraint, index, dan data awal.
3. Cek connection string di `appsettings.json`, sesuaikan sama SQL Server-mu. Kalau pakai LocalDB:
   `Server=localhost;Database=FoodOrderingDb;User Id=sa;Password=P@$$w0rd;Trusted_Connection=True;`
4. Jalanin: `dotnet run`
5. Buka Scalar di `https://localhost:5163/scalar` (port-nya lihat di terminal)

## Struktur folder
```
Controllers/    -> nerima request, mengembalikan response
Services/       -> logika bisnis
Repositories/   -> query SQL ke database
Dtos/           -> bentuk data request & response + validasi
Models/         -> class Category dan MenuItem
Data/           -> SqlConnectionFactory (bikin koneksi)
Database/       -> schema.sql (tabel, constraint, index, seed)
Exceptions/     -> NotFoundException, ConflictException
Middleware/     -> nangkep exception jadi response yang rapi
```
Alurnya: Controller -> Service -> Repository -> Database.

## Endpoint

### Categories
| Method | URL | Keterangan |
|---|---|---|
| GET | `/api/categories` | semua kategori |
| GET | `/api/categories/{id}` | satu kategori |
| POST | `/api/categories` | tambah kategori |
| PUT | `/api/categories/{id}` | ubah kategori |
| DELETE | `/api/categories/{id}` | hapus kategori |

### Menus
| Method | URL | Keterangan |
|---|---|---|
| GET | `/api/menus` | semua menu |
| GET | `/api/menus/{id}` | satu menu |
| POST | `/api/menus` | tambah menu |
| PUT | `/api/menus/{id}` | ubah menu |
| DELETE | `/api/menus/{id}` | hapus menu |

Query opsional buat `GET /api/menus` (seacrhing dan sorting di Homework 4):
- `keyword` -> cari berdasarkan nama
- `categoryId` -> filter kategori
- `sort` -> `price_asc` (termurah) atau `price_desc` (termahal)

Contoh: `/api/menus?keyword=burger&categoryId=1&sort=price_asc`

Contoh body POST `/api/menus`:
```json
{
  "name": "Mushroom Burger",
  "price": 32000,
  "stock": 10,
  "categoryId": 1
}
```

## Validation (di DTO)
- Nama wajib diisi, kategori 2-50 karakter, menu 2-100 karakter
- Harga harus > 0
- Stok ga boleh negatif
- `categoryId` harus ada

Kalau validasinya gagal -> 400 Bad Request.

## Constraint, Index, Relationship (di `Database/schema.sql`)
**Constraint**
- Primary key di kedua tabel
- `UQ_Categories_Name` & `UQ_MenuItems_Name` -> nama ga boleh kembar
- `CK_MenuItems_Price` -> Price > 0
- `CK_MenuItems_Stock` -> Stock >= 0
- Kolom nama, harga, stok, kategori semuanya `NOT NULL`

**Index**
- Unique index otomatis dari constraint unique nama
- `IX_MenuItems_CategoryId` (buat filter kategori & JOIN)
- `IX_MenuItems_Price` (buat sort harga)

**Relationship**
- One-to-many: 1 `Category` punya banyak `MenuItem`
- Foreign key `FK_MenuItems_Categories` di `MenuItems.CategoryId`, `ON DELETE NO ACTION` -> kategori yang masih punya menu ga bisa dihapus

## Soal query
Semua query pakai parameter (`@Name`, `@Id`, dst), bukan gabung string dari input user, biar aman dari SQL injection. Satu-satunya bagian yang dinamis itu `ORDER BY` buat sort, dan itu dipilih dari pilihan yang udah ditentukan (`price_asc` / `price_desc`).

## Status code
| Kode | Kapan |
|---|---|
| 200 | berhasil |
| 201 | berhasil buat data baru |
| 204 | no content |
| 400 | validasi gagal |
| 404 | data ga ketemu |
| 409 | konflik |
| 500 | error server |
