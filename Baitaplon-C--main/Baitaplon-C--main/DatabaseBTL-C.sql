IF OBJECT_ID('ChiTietHoaDon', 'U') IS NOT NULL DROP TABLE ChiTietHoaDon;
IF OBJECT_ID('HoaDon', 'U') IS NOT NULL DROP TABLE HoaDon;
IF OBJECT_ID('ChiSoDienNuoc', 'U') IS NOT NULL DROP TABLE ChiSoDienNuoc;
IF OBJECT_ID('SuCo', 'U') IS NOT NULL DROP TABLE SuCo;
IF OBJECT_ID('PhuongTien', 'U') IS NOT NULL DROP TABLE PhuongTien;
IF OBJECT_ID('HopDong', 'U') IS NOT NULL DROP TABLE HopDong;
-- Xóa khóa ngoại vòng tròn ở TaiSan trước khi xóa bảng
IF OBJECT_ID('TaiSan', 'U') IS NOT NULL ALTER TABLE Phongtro DROP CONSTRAINT FK_Phongtro_TaiSan;
IF OBJECT_ID('TaiSan', 'U') IS NOT NULL DROP TABLE TaiSan;
IF OBJECT_ID('Phongtro', 'U') IS NOT NULL DROP TABLE Phongtro;
IF OBJECT_ID('KhachThue', 'U') IS NOT NULL DROP TABLE KhachThue;
IF OBJECT_ID('Dichvu', 'U') IS NOT NULL DROP TABLE Dichvu;
IF OBJECT_ID('LoaiPhong', 'U') IS NOT NULL DROP TABLE LoaiPhong;
IF OBJECT_ID('TaiKhoan', 'U') IS NOT NULL DROP TABLE TaiKhoan;

use  [Baitaplon-C#]
-- 1. Bảng Tài Khoản
CREATE TABLE TaiKhoan 
(
    Tendangnhap nvarchar(50) primary key,
    Matkhau nvarchar(50) not null,
    Hoten nvarchar(50) not null
);

-- 2. Bảng Loại Phòng
CREATE TABLE LoaiPhong
(
    Maloaiphong INT PRIMARY KEY IDENTITY(1,1),
    Tenloai NVARCHAR(50) NOT NULL,
    Dongia DECIMAL(18,0) NOT NULL,
    CONSTRAINT CK_LoaiPhong_Dongia CHECK (Dongia >= 0)
);

-- 3. Bảng Dịch Vụ
CREATE TABLE Dichvu
(
    MaDV nvarchar(50) NOT NULL PRIMARY KEY, -- Đã thêm PRIMARY KEY
    TenDV nvarchar(50) not null,
    Donvitinh nvarchar(50),
    Dongia decimal(18,0) not null check (Dongia >= 0)
);

-- 4. Bảng Khách Thuê
CREATE TABLE KhachThue
(
    Makhach int primary key identity(1,1),
    Hoten nvarchar(100) not null,
    CCCD varchar(20) unique,
    SDT varchar(15),
    Gioitinh nvarchar(10), -- Đã thêm Cột Giới Tính
    Ngaysinh date,
    Quequan nvarchar(100)
);

-- 5. Bảng Phòng Trọ (Tạo bảng trước, thêm khóa ngoại sau để tránh lỗi vòng tròn)
CREATE TABLE Phongtro
(
    Maphong varchar(10) primary key,
    Tenphong nvarchar(50) not null,
    Dientich int,
    Trangthai nvarchar(20) not null default N'Trống',
    Maloaiphong int,
    Makhach int, -- Đã thêm cột Mã Khách
    Mats int,    -- Đã thêm cột Mã Tài Sản
    foreign key (Maloaiphong) references LoaiPhong(Maloaiphong),
    foreign key (Makhach) references KhachThue(Makhach)
);

-- 6. Bảng Tài Sản
CREATE TABLE TaiSan
(
    Mats int primary key identity(1,1),
    Tents nvarchar(100) not null,
    Soluong int not null check (Soluong >= 0),
    Tinhtrang nvarchar(50),
    Maphong varchar(10),
    foreign key (Maphong) references Phongtro(Maphong) on delete set null
);

-- 7. Bảng Hợp Đồng
CREATE TABLE HopDong
(
    MaHD int primary key identity(1,1),
    Maphong varchar(10) not null,
    Makhach int not null,
    Ngaylap date not null,
    Ngaybatdau date not null,
    Ngayketthuc date,
    Tiencoc decimal(18,0) not null check (Tiencoc >= 0),
    Giathuethang decimal(18,0) not null,
    Trangthai nvarchar(20) not null default N'Hiệu lực',
    foreign key(Maphong) references PhongTro(Maphong),
    foreign key(Makhach) references KhachThue(Makhach)
);

-- 8. Bảng Chỉ Số Điện Nước
CREATE TABLE ChiSoDienNuoc
(
    ID int primary key identity(1,1),
    Maphong varchar(10) not null,
    Thang int not null check (Thang between 1 and 12),
    Nam int not null,
    Chisodiencu int not null check (Chisodiencu >= 0),
    Chisodienmoi int not null,
    Chisonuoccu int not null check(Chisonuoccu >= 0),
    Chisonuocmoi int not null,
    unique(Maphong, Thang, Nam),
    foreign key(Maphong) references PhongTro(Maphong),
    CONSTRAINT CK_ChiSoDien_HopLe CHECK (Chisodienmoi >= Chisodiencu),
    CONSTRAINT CK_ChiSoNuoc_HopLe CHECK (Chisonuocmoi >= Chisonuoccu)
);
select * from ChiSoDienNuoc
-- 9. Bảng Hóa Đơn
CREATE TABLE HoaDon
(
    MaHD int primary key identity(1,1),
    MaHopdong int,
    Thang int not null check (Thang between 1 and 12),
    Nam int not null,
    Ngaylap date default getdate(),
    Tongtien decimal(18,0) not null check (Tongtien >= 0 ),
    Trangthai nvarchar(20) not null default N'Chưa trả',
    foreign key (MaHopdong) references HopDong(MaHD)
);

-- 10. Bảng Chi Tiết Hóa Đơn
CREATE TABLE ChiTietHoaDon
(
    MaCT int primary key identity(1,1),
    MaHoaDon int not null,
    MaDV nvarchar(50), -- Đã thêm cột Mã Dịch Vụ
    Tenkhoanmuc nvarchar(100) not null,
    Donvitinh nvarchar(20),
    Soluong decimal(10,2) not null,
    Dongia decimal(18,0) not null,  
    Thanhtien AS (CAST(Soluong * Dongia AS decimal(18,0))), 
    foreign key (MaHoaDon) references HoaDon(MaHD),
    foreign key (MaDV) references Dichvu(MaDV) -- Đã thêm Khóa Ngoại Dịch Vụ
);

-- 11. CẬP NHẬT KHÓA NGOẠI CHO BẢNG PHÒNG TRỌ (Mats)
-- Phải làm bước này cuối cùng vì khi tạo bảng Phongtro, bảng TaiSan chưa tồn tại
ALTER TABLE Phongtro
ADD CONSTRAINT FK_Phongtro_TaiSan
FOREIGN KEY (Mats) REFERENCES TaiSan(Mats);

--
INSERT INTO LoaiPhong (Tenloai, Dongia) VALUES
( N'Phòng có gác', 2500000),
( N'Phòng có gác xép, ban công', 2800000),
( N'Phòng không gác, không ban công', 2200000);

--
INSERT INTO KhachThue (Hoten, CCCD, SDT, Ngaysinh, Gioitinh, Quequan) VALUES
(N'Nguyễn Văn Nam', '001201000123', '0987654321', '2001-03-12', N'Nam', N'Hà Nội'),
(N'Hoàng Minh Hải', '001305027490', '0973930673', '2005-10-02', N'Nam', N'Hà Nội'),
(N'Trần Thị Lan', '001202000456', '0978123456', '2002-06-25', N'Nữ', N'Nam Định'),
(N'Lê Minh Hoàng', '001200000789', '0963456789', '2000-11-10', N'Nam', N'Thái Bình'),
(N'Hoàng Thị Khánh Ngọc', '001200000742', '0963845932', '2003-01-12', N'Nữ', N'Thái Bình'),
(N'Phạm Ngọc Anh', '001201000987', '0955789123', '2001-08-18', N'Nữ', N'Hà Nam'),
(N'Đỗ Văn Tuấn', '001199000654', '0944321987', '1999-02-03', N'Nam', N'Bắc Giang'),
(N'Nguyễn Quang Minh', '001301048735', '0983374975', '2001-03-04', N'Nam', N'Bắc Ninh'),
(N'Nguyễn Thị Hương', '001202000321', '0936654111', '2002-09-20', N'Nữ', N'Thanh Hóa'),
(N'Vũ Đức Long', '001198000222', '0912888999', '1998-12-14', N'Nam', N'Hải Phòng'),
(N'Vũ Huyền Trang', '001103058637', '0973878920', '2004-06-13', N'Nữ', N'Hải Phòng'),
(N'Nguyễn Thùy Linh', '001305319876', '0399540737', '2005-05-31', N'Nữ', N'Hà Nội');


--
INSERT INTO Dichvu (MaDV, TenDV, Donvitinh, Dongia) VALUES
('DV01', N'Tiền điện', 'kWh', 3500),
('DV02', N'Tiền nước', 'm3', 30000),
('DV03', N'Internet', N'Tháng', 100000),
('DV04', N'Rác sinh hoạt', N'Tháng', 30000),
('DV05', N'Giữ xe', N'Tháng', 50000);

--
INSERT INTO Phongtro (Maphong, Tenphong, Dientich, Trangthai, Maloaiphong, Makhach, Mats) VALUES
-- Loại 1 (ID 1), Khách 1-3
('P101', N'Phòng 101', 30, N'Đang thuê', 1, 1, NULL),
('P102', N'Phòng 102', 30, N'Đang thuê', 1, 2, NULL),
('P103', N'Phòng 103', 30, N'Đang thuê', 1, 3, NULL),

-- Loại 2 (ID 2), Khách 4-6
('P201', N'Phòng 201', 30, N'Đang thuê', 2, 4, NULL),
('P202', N'Phòng 202', 30, N'Đang thuê', 2, 5, NULL),
('P203', N'Phòng 203', 30, N'Đang thuê', 2, 6, NULL),

-- Loại 3 (ID 3), Khách 7-12
('P301', N'Phòng 301', 22, N'Đang thuê', 3, 7, NULL),
('P302', N'Phòng 302', 22, N'Đang thuê', 3, 8, NULL),
('P303', N'Phòng 303', 22, N'Đang thuê', 3, 9, NULL),
('P401', N'Phòng 401', 22, N'Đang thuê', 3, 10, NULL),
('P402', N'Phòng 402', 22, N'Đang thuê', 3, 11, NULL),
('P403', N'Phòng 403', 22, N'Đang thuê', 3, 12, NULL);

--
INSERT INTO TaiSan (Tents, Soluong, Tinhtrang, Maphong) VALUES
-- Phòng P101
(N'Giường', 1, N'Tốt', 'P101'), (N'Tủ quần áo', 1, N'Tốt', 'P101'), (N'Bàn học', 1, N'Tốt', 'P101'), (N'Quạt treo tường', 1, N'Tốt', 'P101'), (N'Bóng đèn', 2, N'Tốt', 'P101'),

-- Phòng P102
(N'Giường', 1, N'Tốt', 'P102'), (N'Tủ quần áo', 1, N'Tốt', 'P102'), (N'Bàn học', 1, N'Tốt', 'P102'), (N'Quạt treo tường', 1, N'Tốt', 'P102'), (N'Bóng đèn', 2, N'Tốt', 'P102'),

-- Phòng P103
(N'Giường', 1, N'Tốt', 'P103'), (N'Tủ quần áo', 1, N'Tốt', 'P103'), (N'Bàn học', 1, N'Tốt', 'P103'), (N'Quạt treo tường', 1, N'Tốt', 'P103'), (N'Bóng đèn', 2, N'Tốt', 'P103'),

-- Phòng P201 (Có ban công)
(N'Giường', 1, N'Tốt', 'P201'), (N'Tủ quần áo', 1, N'Tốt', 'P201'), (N'Bàn học', 1, N'Tốt', 'P201'), (N'Quạt treo tường', 1, N'Tốt', 'P201'), (N'Bóng đèn', 2, N'Tốt', 'P201'), (N'Cửa sổ / ban công', 1, N'Tốt', 'P201'),

-- Phòng P202
(N'Giường', 1, N'Tốt', 'P202'), (N'Tủ quần áo', 1, N'Tốt', 'P202'), (N'Bàn học', 1, N'Tốt', 'P202'), (N'Quạt treo tường', 1, N'Tốt', 'P202'), (N'Bóng đèn', 2, N'Tốt', 'P202'), (N'Cửa sổ / ban công', 1, N'Tốt', 'P202'),

-- Phòng P203
(N'Giường', 1, N'Tốt', 'P203'), (N'Tủ quần áo', 1, N'Tốt', 'P203'), (N'Bàn học', 1, N'Tốt', 'P203'), (N'Quạt treo tường', 1, N'Tốt', 'P203'), (N'Bóng đèn', 2, N'Tốt', 'P203'), (N'Cửa sổ / ban công', 1, N'Tốt', 'P203'),

-- Các phòng tầng 3, 4 (Giống tầng 1)
(N'Giường', 1, N'Tốt', 'P301'), (N'Tủ quần áo', 1, N'Tốt', 'P301'), (N'Bàn học', 1, N'Tốt', 'P301'), (N'Quạt treo tường', 1, N'Tốt', 'P301'), (N'Bóng đèn', 2, N'Tốt', 'P301'),
(N'Giường', 1, N'Tốt', 'P302'), (N'Tủ quần áo', 1, N'Tốt', 'P302'), (N'Bàn học', 1, N'Tốt', 'P302'), (N'Quạt treo tường', 1, N'Tốt', 'P302'), (N'Bóng đèn', 2, N'Tốt', 'P302'),
(N'Giường', 1, N'Tốt', 'P303'), (N'Tủ quần áo', 1, N'Tốt', 'P303'), (N'Bàn học', 1, N'Tốt', 'P303'), (N'Quạt treo tường', 1, N'Tốt', 'P303'), (N'Bóng đèn', 2, N'Tốt', 'P303'),

(N'Giường', 1, N'Tốt', 'P401'), (N'Tủ quần áo', 1, N'Tốt', 'P401'), (N'Bàn học', 1, N'Tốt', 'P401'), (N'Quạt treo tường', 1, N'Tốt', 'P401'), (N'Bóng đèn', 2, N'Tốt', 'P401'),
(N'Giường', 1, N'Tốt', 'P402'), (N'Tủ quần áo', 1, N'Tốt', 'P402'), (N'Bàn học', 1, N'Tốt', 'P402'), (N'Quạt treo tường', 1, N'Tốt', 'P402'), (N'Bóng đèn', 2, N'Tốt', 'P402'),
(N'Giường', 1, N'Tốt', 'P403'), (N'Tủ quần áo', 1, N'Tốt', 'P403'), (N'Bàn học', 1, N'Tốt', 'P403'), (N'Quạt treo tường', 1, N'Tốt', 'P403'), (N'Bóng đèn', 2, N'Tốt', 'P403');

--
INSERT INTO HopDong (Maphong, Makhach, Ngaylap, Ngaybatdau, Ngayketthuc, Tiencoc, Giathuethang, Trangthai) VALUES
('P101', 1, '2025-01-01', '2025-01-01', '2025-12-31', 2500000, 2500000, N'Hiệu lực'),
('P102', 2, '2025-01-05', '2025-01-05', '2026-01-04', 2500000, 2500000, N'Hiệu lực'),
('P103', 3, '2025-01-10', '2025-01-10', '2026-01-09', 2500000, 2500000, N'Hiệu lực'),

('P201', 4, '2025-02-01', '2025-02-01', '2026-01-31', 2800000, 2800000, N'Hiệu lực'),
('P202', 5, '2025-02-03', '2025-02-03', '2026-02-02', 2800000, 2800000, N'Hiệu lực'),
('P203', 6, '2025-02-05', '2025-02-05', '2026-02-04', 2800000, 2800000, N'Hiệu lực'),

('P301', 7, '2025-03-01', '2025-03-01', '2026-02-28', 2200000, 2200000, N'Hiệu lực'),
('P302', 8, '2025-03-05', '2025-03-05', '2026-03-04', 2200000, 2200000, N'Hiệu lực'),
('P303', 9, '2025-03-10', '2025-03-10', '2026-03-09', 2200000, 2200000, N'Hiệu lực'),

('P401', 10, '2025-04-01', '2025-04-01', '2026-03-31', 2200000, 2200000, N'Hiệu lực'),
('P402', 11, '2025-04-05', '2025-04-05', '2026-04-04', 2200000, 2200000, N'Hiệu lực'),
('P403', 12, '2025-04-10', '2025-04-10', '2026-04-09', 2200000, 2200000, N'Hiệu lực');

--
INSERT INTO ChiSoDienNuoc (Maphong, Thang, Nam, Chisodiencu, Chisodienmoi, Chisonuoccu, Chisonuocmoi) VALUES
('P101', 4, 2025, 1200, 1250, 400, 405), -- Dùng 50 điện, 5 nước
('P102', 4, 2025, 1150, 1210, 390, 398), -- Dùng 60 điện, 8 nước
('P103', 4, 2025, 1300, 1345, 410, 414), -- Dùng 45 điện, 4 nước
('P201', 4, 2025, 200, 280, 50, 56),     -- Dùng 80 điện, 6 nước
('P202', 4, 2025, 180, 250, 45, 50),     -- Dùng 70 điện, 5 nước
('P203', 4, 2025, 220, 300, 60, 68),     -- Dùng 80 điện, 8 nước
('P301', 4, 2025, 50, 100, 10, 15),      -- Dùng 50 điện, 5 nước
('P302', 4, 2025, 40, 85, 12, 16),       -- Dùng 45 điện, 4 nước
('P303', 4, 2025, 60, 120, 15, 22),      -- Dùng 60 điện, 7 nước
('P401', 4, 2025, 0, 40, 0, 3),          -- Mới vào: Dùng 40 điện, 3 nước
('P402', 4, 2025, 0, 35, 0, 2),          -- Mới vào: Dùng 35 điện, 2 nước
('P403', 4, 2025, 0, 30, 0, 2);          -- Mới vào: Dùng 30 điện, 2 nước

--
INSERT INTO HoaDon (MaHopdong, Thang, Nam, Ngaylap, Tongtien, Trangthai) VALUES
(1, 4, 2025, '2025-04-30', 3005000, N'Chưa trả'), -- P101
(2, 4, 2025, '2025-04-30', 3130000, N'Chưa trả'), -- P102
(3, 4, 2025, '2025-04-30', 2957500, N'Chưa trả'), -- P103
(4, 4, 2025, '2025-04-30', 3440000, N'Chưa trả'), -- P201
(5, 4, 2025, '2025-04-30', 3375000, N'Chưa trả'), -- P202
(6, 4, 2025, '2025-04-30', 3500000, N'Chưa trả'), -- P203
(7, 4, 2025, '2025-04-30', 2705000, N'Chưa trả'), -- P301
(8, 4, 2025, '2025-04-30', 2657500, N'Chưa trả'), -- P302
(9, 4, 2025, '2025-04-30', 2800000, N'Chưa trả'), -- P303
(10, 4, 2025, '2025-04-30', 2610000, N'Chưa trả'), -- P401
(11, 4, 2025, '2025-04-30', 2562500, N'Chưa trả'), -- P402
(12, 4, 2025, '2025-04-30', 2545000, N'Chưa trả'); -- P403

--
-- === CHI TIẾT HÓA ĐƠN SỐ 1 (Phòng P101) ===
INSERT INTO ChiTietHoaDon (MaHoaDon, MaDV, Tenkhoanmuc, Donvitinh, Soluong, Dongia) VALUES
(1, NULL, N'Tiền thuê phòng P101', N'Tháng', 1, 2500000),     -- Giá phòng
(1, 'DV01', N'Tiền điện T4', 'kWh', 50, 3500),               -- 1250 - 1200 = 50 số
(1, 'DV02', N'Tiền nước T4', 'm3', 5, 30000),                -- 405 - 400 = 5 khối
(1, 'DV03', N'Internet', N'Tháng', 1, 100000),               -- Mạng
(1, 'DV04', N'Rác sinh hoạt', N'Tháng', 1, 30000),           -- Rác
(1, 'DV05', N'Giữ xe', N'Tháng', 1, 50000);                  -- Xe

-- === CHI TIẾT HÓA ĐƠN SỐ 2 (Phòng P102) ===
INSERT INTO ChiTietHoaDon (MaHoaDon, MaDV, Tenkhoanmuc, Donvitinh, Soluong, Dongia) VALUES
(2, NULL, N'Tiền thuê phòng P102', N'Tháng', 1, 2500000),
(2, 'DV01', N'Tiền điện T4', 'kWh', 60, 3500),               -- 60 số
(2, 'DV02', N'Tiền nước T4', 'm3', 8, 30000),                -- 8 khối
(2, 'DV03', N'Internet', N'Tháng', 1, 100000),
(2, 'DV04', N'Rác sinh hoạt', N'Tháng', 1, 30000),
(2, 'DV05', N'Giữ xe', N'Tháng', 1, 50000);

-- === CHI TIẾT HÓA ĐƠN SỐ 3 (Phòng P103) ===
INSERT INTO ChiTietHoaDon (MaHoaDon, MaDV, Tenkhoanmuc, Donvitinh, Soluong, Dongia) VALUES
(3, NULL, N'Tiền thuê phòng P103', N'Tháng', 1, 2500000),
(3, 'DV01', N'Tiền điện T4', 'kWh', 45, 3500),               -- 45 số
(3, 'DV02', N'Tiền nước T4', 'm3', 4, 30000),                -- 4 khối
(3, 'DV03', N'Internet', N'Tháng', 1, 100000),
(3, 'DV04', N'Rác sinh hoạt', N'Tháng', 1, 30000),
(3, 'DV05', N'Giữ xe', N'Tháng', 1, 50000);

select * from Hoadon 
-- Đổi tên cột trong bảng HopDong
EXEC sp_rename 'HopDong.Trangthai', 'Trangthaihopdong', 'COLUMN';

-- Đổi tên cột trong bảng Phongtro
EXEC sp_rename 'Phongtro.Trangthai', 'Trangthaiphongtro', 'COLUMN';

-- 12. Bảng Phương Tiện (Phiên bản liên kết chặt chẽ)
CREATE TABLE PhuongTien
(
    MaPT INT PRIMARY KEY IDENTITY(1,1),
    Makhach INT NOT NULL,                 -- Liên kết chặt với bảng KhachThue
    Loaixe NVARCHAR(50) NOT NULL,         -- Xe máy, Xe đạp, Oto...
    Hieuxe NVARCHAR(50),                  -- Vision, Airblade...
    Bienso NVARCHAR(20),                  -- Biển số
    Mauxe NVARCHAR(30),                   -- Màu xe
    Ngaydangky DATE DEFAULT GETDATE(),    -- Ngày bắt đầu gửi xe
    
    -- RÀNG BUỘC KHÓA NGOẠI: Xóa khách là xóa luôn xe của khách đó
    CONSTRAINT FK_PhuongTien_KhachThue 
    FOREIGN KEY (Makhach) REFERENCES KhachThue(Makhach) ON DELETE CASCADE
);

-- Thêm dữ liệu bảng Phương Tiện
-- Lưu ý: Makhach phải trùng khớp với bảng KhachThue và PhongTro đang có

INSERT INTO PhuongTien (Makhach, Loaixe, Hieuxe, Bienso, Mauxe, Ngaydangky) VALUES
-- 1. Phòng P101 (Khách: Nguyễn Văn Nam - Vào ở 01/01/2025)
(1, N'Xe máy', N'Honda Wave Alpha', '29H1-123.45', N'Trắng', '2025-01-01'),

-- 2. Phòng P102 (Khách: Hoàng Minh Hải - Vào ở 05/01/2025)
(2, N'Xe máy', N'Yamaha Exciter', '29B1-567.89', N'Xanh GP', '2025-01-05'),

-- 3. Phòng P103 (Khách: Trần Thị Lan - Vào ở 10/01/2025)
(3, N'Xe máy', N'Honda Vision', '18B2-999.99', N'Đỏ đô', '2025-01-11'),

-- 4. Phòng P201 (Khách: Lê Minh Hoàng - Vào ở 01/02/2025)
(4, N'Xe máy', N'Honda AirBlade', '17B1-111.22', N'Đen nhám', '2025-02-01'),

-- 5. Phòng P202 (Khách: Hoàng Thị Khánh Ngọc - Vào ở 03/02/2025)
(5, N'Xe máy điện', N'Vinfast Klara', '29MĐ-123.56', N'Trắng ngọc', '2025-02-03'),

-- 6. Phòng P203 (Khách: Phạm Ngọc Anh - Vào ở 05/02/2025)
(6, N'Xe máy', N'Honda Lead', '90H1-888.66', N'Vàng kem', '2025-02-05'),

-- 7. Phòng P301 (Khách: Đỗ Văn Tuấn - Vào ở 01/03/2025)
(7, N'Xe máy', N'Yamaha Sirius', '98B1-456.78', N'Đen đỏ', '2025-03-01'),

-- 8. Phòng P302 (Khách: Nguyễn Quang Minh - Vào ở 05/03/2025)
(8, N'Xe máy', N'Honda Winner X', '99G1-234.56', N'Cam đen', '2025-03-05'),

-- 9. Phòng P303 (Khách: Nguyễn Thị Hương - Vào ở 10/03/2025)
-- Khách này không có xe (Đi bộ hoặc đi xe bus), ta không insert dòng này.

-- 10. Phòng P401 (Khách: Vũ Đức Long - Vào ở 01/04/2025)
(10, N'Xe máy', N'Honda SH 150i', '15B1-678.90', N'Xám xi măng', '2025-04-01'),

-- 11. Phòng P402 (Khách: Vũ Huyền Trang - Vào ở 05/04/2025)
(11, N'Xe đạp điện', N'Pega Cap A', '15MĐ-001.02', N'Xanh dương', '2025-04-05'),

-- 12. Phòng P403 (Khách: Nguyễn Thùy Linh - Vào ở 10/04/2025)
(12, N'Xe đạp', N'Martin 107', NULL, N'Bạc', '2025-04-10'); 
-- Xe đạp thường không có biển số nên để NULL
select * from KhachThue
SELECT * FROM PhuongTien pt
JOIN KhachThue k ON pt.Makhach = k.Makhach

CREATE TABLE SuCo
(
    MaSC VARCHAR(20) PRIMARY KEY, -- Đổi sang VARCHAR để bạn đặt mã theo ý muốn (ví dụ: SC001)
    Mats INT NOT NULL,
    Maphong VARCHAR(10) NOT NULL,
    NgayBao DATE,
    MoTaSuCo NVARCHAR(255),
    TrangThaiXuLy NVARCHAR(50),
    ChiPhiSuaDuKien DECIMAL(18,0),
    
    CONSTRAINT FK_SuCo_TaiSan FOREIGN KEY (Mats) REFERENCES TaiSan(Mats),
    CONSTRAINT FK_SuCo_Phongtro FOREIGN KEY (Maphong) REFERENCES Phongtro(Maphong)
);
-- Lưu ý: Đảm bảo các Mats (4, 24, 5, 10) và Maphong (P101, P202, P301) đã tồn tại trong bảng gốc
INSERT INTO SuCo (MaSC, Mats, Maphong, NgayBao, MoTaSuCo, TrangThaiXuLy, ChiPhiSuaDuKien) 
VALUES
('SC001', 4, 'P101', '2025-12-05', N'Quạt trần kêu to khi chạy số lớn', N'Đã hoàn thành', 120000),
('SC002', 24, 'P202', '2025-12-15', N'Vòi hoa sen bị rò rỉ nước', N'Đang sửa', 80000),
('SC003', 5, 'P101', '2025-12-20', N'Ổ cắm điện cạnh giường bị lỏng', N'Chờ xử lý', 45000),
('SC004', 10, 'P301', '2025-12-28', N'Điều hòa không mát, cần nạp gas', N'Chờ xử lý', 550000),
('SC005', 4, 'P101', '2025-12-29', N'Thay dây điện quạt bị hỏng', N'Đã hoàn thành', 30000);

INSERT INTO ChiTietHoaDon (MaHoaDon, MaDV, Tenkhoanmuc, Donvitinh, Soluong, Dongia) VALUES
(4, NULL, N'Tiền thuê phòng P201', N'Tháng', 1, 2800000),
(4, 'DV01', N'Tiền điện T4', 'kWh', 80, 3500), -- 280 - 200 = 80 số điện
(4, 'DV02', N'Tiền nước T4', 'm3', 6, 30000),  -- 56 - 50 = 6 khối nước
(4, 'DV03', N'Internet', N'Tháng', 1, 100000),
(4, 'DV04', N'Rác sinh hoạt', N'Tháng', 1, 30000),
(4, 'DV05', N'Giữ xe', N'Tháng', 1, 50000);

-- 1. Dữ liệu cho Hóa đơn 5 (Phòng P202)
INSERT INTO ChiTietHoaDon (MaHoaDon, MaDV, Tenkhoanmuc, Donvitinh, Soluong, Dongia) VALUES
(5, NULL, N'Tiền thuê phòng P202', N'Tháng', 1, 2800000),
(5, 'DV01', N'Tiền điện T4', 'kWh', 70, 3500), -- 250 - 180 = 70 số
(5, 'DV02', N'Tiền nước T4', 'm3', 5, 30000),   -- 50 - 45 = 5 khối
(5, 'DV03', N'Internet', N'Tháng', 1, 100000),
(5, 'DV04', N'Rác sinh hoạt', N'Tháng', 1, 30000),
(5, 'DV05', N'Giữ xe', N'Tháng', 1, 50000);

-- 2. Dữ liệu cho Hóa đơn 6 (Phòng P203)
INSERT INTO ChiTietHoaDon (MaHoaDon, MaDV, Tenkhoanmuc, Donvitinh, Soluong, Dongia) VALUES
(6, NULL, N'Tiền thuê phòng P203', N'Tháng', 1, 2800000),
(6, 'DV01', N'Tiền điện T4', 'kWh', 80, 3500), -- 300 - 220 = 80 số
(6, 'DV02', N'Tiền nước T4', 'm3', 8, 30000),   -- 68 - 60 = 8 khối
(6, 'DV03', N'Internet', N'Tháng', 1, 100000),
(6, 'DV04', N'Rác sinh hoạt', N'Tháng', 1, 30000),
(6, 'DV05', N'Giữ xe', N'Tháng', 1, 50000);

-- 3. Dữ liệu cho Hóa đơn 7 (Phòng P301)
INSERT INTO ChiTietHoaDon (MaHoaDon, MaDV, Tenkhoanmuc, Donvitinh, Soluong, Dongia) VALUES
(7, NULL, N'Tiền thuê phòng P301', N'Tháng', 1, 2200000),
(7, 'DV01', N'Tiền điện T4', 'kWh', 50, 3500), -- 100 - 50 = 50 số
(7, 'DV02', N'Tiền nước T4', 'm3', 5, 30000),   -- 15 - 10 = 5 khối
(7, 'DV03', N'Internet', N'Tháng', 1, 100000),
(7, 'DV04', N'Rác sinh hoạt', N'Tháng', 1, 30000),
(7, 'DV05', N'Giữ xe', N'Tháng', 1, 50000);

-- 4. Dữ liệu cho Hóa đơn 8 (Phòng P302)
INSERT INTO ChiTietHoaDon (MaHoaDon, MaDV, Tenkhoanmuc, Donvitinh, Soluong, Dongia) VALUES
(8, NULL, N'Tiền thuê phòng P302', N'Tháng', 1, 2200000),
(8, 'DV01', N'Tiền điện T4', 'kWh', 45, 3500), -- 85 - 40 = 45 số
(8, 'DV02', N'Tiền nước T4', 'm3', 4, 30000),   -- 16 - 12 = 4 khối
(8, 'DV03', N'Internet', N'Tháng', 1, 100000),
(8, 'DV04', N'Rác sinh hoạt', N'Tháng', 1, 30000),
(8, 'DV05', N'Giữ xe', N'Tháng', 1, 50000);

-- 5. Dữ liệu cho Hóa đơn 9 (Phòng P303)
INSERT INTO ChiTietHoaDon (MaHoaDon, MaDV, Tenkhoanmuc, Donvitinh, Soluong, Dongia) VALUES
(9, NULL, N'Tiền thuê phòng P303', N'Tháng', 1, 2200000),
(9, 'DV01', N'Tiền điện T4', 'kWh', 60, 3500), -- 120 - 60 = 60 số
(9, 'DV02', N'Tiền nước T4', 'm3', 7, 30000),   -- 22 - 15 = 7 khối
(9, 'DV03', N'Internet', N'Tháng', 1, 100000),
(9, 'DV04', N'Rác sinh hoạt', N'Tháng', 1, 30000),
(9, 'DV05', N'Giữ xe', N'Tháng', 1, 50000);

-- 6. Dữ liệu cho Hóa đơn 10 (Phòng P401)
INSERT INTO ChiTietHoaDon (MaHoaDon, MaDV, Tenkhoanmuc, Donvitinh, Soluong, Dongia) VALUES
(10, NULL, N'Tiền thuê phòng P401', N'Tháng', 1, 2200000),
(10, 'DV01', N'Tiền điện T4', 'kWh', 40, 3500), -- 40 - 0 = 40 số
(10, 'DV02', N'Tiền nước T4', 'm3', 3, 30000),  -- 3 - 0 = 3 khối
(10, 'DV03', N'Internet', N'Tháng', 1, 100000),
(10, 'DV04', N'Rác sinh hoạt', N'Tháng', 1, 30000),
(10, 'DV05', N'Giữ xe', N'Tháng', 1, 50000);

-- 7. Dữ liệu cho Hóa đơn 11 (Phòng P402)
INSERT INTO ChiTietHoaDon (MaHoaDon, MaDV, Tenkhoanmuc, Donvitinh, Soluong, Dongia) VALUES
(11, NULL, N'Tiền thuê phòng P402', N'Tháng', 1, 2200000),
(11, 'DV01', N'Tiền điện T4', 'kWh', 35, 3500), -- 35 - 0 = 35 số
(11, 'DV02', N'Tiền nước T4', 'm3', 2, 30000),  -- 2 - 0 = 2 khối
(11, 'DV03', N'Internet', N'Tháng', 1, 100000),
(11, 'DV04', N'Rác sinh hoạt', N'Tháng', 1, 30000),
(11, 'DV05', N'Giữ xe', N'Tháng', 1, 50000);

-- 8. Dữ liệu cho Hóa đơn 12 (Phòng P403)
INSERT INTO ChiTietHoaDon (MaHoaDon, MaDV, Tenkhoanmuc, Donvitinh, Soluong, Dongia) VALUES
(12, NULL, N'Tiền thuê phòng P403', N'Tháng', 1, 2200000),
(12, 'DV01', N'Tiền điện T4', 'kWh', 30, 3500), -- 30 - 0 = 30 số
(12, 'DV02', N'Tiền nước T4', 'm3', 2, 30000),  -- 2 - 0 = 2 khối
(12, 'DV03', N'Internet', N'Tháng', 1, 100000),
(12, 'DV04', N'Rác sinh hoạt', N'Tháng', 1, 30000),
(12, 'DV05', N'Giữ xe', N'Tháng', 1, 50000);

ALTER TABLE ChiSoDienNuoc
ADD SoDienSuDung AS (Chisodienmoi - Chisodiencu);

ALTER TABLE ChiSoDienNuoc
ADD SoNuocSuDung AS (Chisonuocmoi - Chisonuoccu);


select * from ChiSoDienNuoc
SELECT 
    h.MaHD, 
    p.Tenphong, 
    k.Hoten, 
    (CAST(h.Thang AS VARCHAR) + '/' + CAST(h.Nam AS VARCHAR)) AS KyThanhToan,
    h.Ngaylap, 
    h.Tongtien, 
    h.Trangthai
FROM HoaDon h
JOIN HopDong hd ON h.MaHopdong = hd.MaHD
JOIN Phongtro p ON hd.Maphong = p.Maphong
JOIN KhachThue k ON hd.Makhach = k.Makhach
ORDER BY h.Nam DESC, h.Thang DESC;

CREATE VIEW View_LichSuThue AS
SELECT 
    h.MaHD, h.Maphong, h.Makhach, k.Hoten, k.CCCD, k.SDT, 
    h.Ngaybatdau, h.Ngayketthuc, h.Tiencoc, h.Giathuethang, h.Trangthaihopdong
FROM HopDong h
JOIN KhachThue k ON h.Makhach = k.Makhach;

ALTER VIEW View_LichSuThue AS
SELECT 
    h.MaHD, 
    h.Maphong, 
    h.Makhach, 
    k.Hoten, 
    k.CCCD, 
    h.Ngaybatdau, 
    h.Ngayketthuc, 
    h.Tiencoc, 
    h.Giathuethang, 
    h.Trangthaihopdong,
    h.Ngaylap,     -- Cột thiếu 1
    k.SDT,         -- Chỉnh vị trí SDT ở đây để không bị nhảy ra cuối
    k.Gioitinh,    -- Cột thiếu 2
    k.Ngaysinh,    -- Cột thiếu 3
    k.Quequan      -- Cột thiếu 4
FROM HopDong h
JOIN KhachThue k ON h.Makhach = k.Makhach;