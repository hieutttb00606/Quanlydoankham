# Hệ Thống Quản Lý Đoàn Khám Sức Khỏe

Hệ thống quản lý toàn diện dành cho các đoàn khám sức khỏe, bao gồm quản lý nhân sự, vật tư, hợp đồng và báo cáo.

## 📂 Gộp Cấu Trúc Dự Án

- **`/QuanLyDoanKham`**: Backend Web API (.NET 8.0)
- **`/QuanLyDoanKham.Web`**: Frontend Vue.js (Vite + TailwindCSS)
- **`/Database`**: Chứa các scripts SQL và tài liệu database.

## 🗄️ Khởi tạo Database

### Bước 1: Tạo Database & Migration
```bash
cd QuanLyDoanKham/QuanLyDoanKham.API
dotnet ef database update
```

### Bước 2: Thêm dữ liệu mẫu (Optional)
Chạy các script SQL theo thứ tự trong SQL Server Management Studio:
1. `Database/Scripts/03_SEED_SAMPLE_DATA.sql` - Dữ liệu mẫu (Công ty, Hợp đồng, Nhân viên, v.v.)
2. `Database/Scripts/04_CREATE_CUSTOMER_USERS.sql` - Tài khoản Customer (vingroup, fpt)

**Hoặc chạy nhanh bằng lệnh:**
```bash
sqlcmd -S . -d QuanLyDoanKham -i "Database/Scripts/03_SEED_SAMPLE_DATA.sql"
sqlcmd -S . -d QuanLyDoanKham -i "Database/Scripts/04_CREATE_CUSTOMER_USERS.sql"
```

## 🔐 Thông Tin Đăng Nhập

Xem chi tiết trong file: [`LOGIN_INFO.md`](LOGIN_INFO.md)

**Tài khoản Admin:**
- Username: `admin`
- Password: `admin123`

**Tài khoản Customer (sau khi chạy script):**
- VinGroup: `vingroup` / `vingroup123`
- FPT: `fpt` / `fpt123`

## 🚀 Khởi chạy ứng dụng

Bạn có thể sử dụng workflow có sẵn để chạy toàn bộ hệ thống bằng một câu lệnh:
```bash
/run-dev
```

Hoặc chạy thủ công:

### 1. Backend (API)
```bash
cd QuanLyDoanKham/QuanLyDoanKham.API
dotnet run
```

### 2. Frontend (Web)
```bash
cd QuanLyDoanKham.Web
npm install
npm run dev
```

## 🛠 Tính Năng Chính

### 👨‍⚕️ Quản lý Nhân sự (Mới)
- Quản lý hồ sơ chi tiết: CMND, Mã số thuế, Tài khoản ngân hàng.
- Tự động chuẩn hóa tên không dấu để tìm kiếm.
- Phân loại nhân sự nội bộ/thuê ngoài.
- Hệ thống tính lương theo buổi khám (1 củ còn 500k).

### 📦 Quản lý Vật tư
- Theo dõi tồn kho vật tư tiêu hao và tài sản cố định.
- Xuất vật tư cho đoàn khám và quản lý vật tư hoàn trả.
- Tự động cập nhật tồn kho khi nhập/xuất/trả.

### 📜 Hợp đồng & Đoàn khám
- Quản lý hợp đồng khám với các công ty.
- Phân chia đoàn khám theo ngày.
- Giao nhiệm vụ cho nhân sự và cấp phát vật tư theo từng đoàn.

### 📊 Báo cáo & Thống kê
- Tổng hợp chi phí nhân sự và vật tư cho từng đoàn khám.
- Báo cáo doanh thu và vật tư tiêu hao.

## 📝 Lưu Ý
- Backend yêu cầu SQL Server. Cấu hình chuỗi kết nối trong `appsettings.json`.
- Hệ thống sử dụng JWT để bảo mật API.

---
**Phát triển bởi Antigravity IDE - 2026**
