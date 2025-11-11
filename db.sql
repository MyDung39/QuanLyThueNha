USE master;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = N'QLTN')
BEGIN
    ALTER DATABASE QLTN SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE QLTN;
END
GO

CREATE DATABASE QLTN;
GO

USE QLTN;
GO

-- Bảng Người Dùng (User)
CREATE TABLE NguoiDung (
    MaNguoiDung VARCHAR(20) PRIMARY KEY,
    TenDangNhap VARCHAR(50) UNIQUE NOT NULL, --email làm tên đăng nhập
    MatKhau VARCHAR(20) NOT NULL,
    TenTaiKhoan VARCHAR(20), --Bsung
    SoDienThoai VARCHAR(20),
    PhuongThucDN NVARCHAR(10) NOT NULL DEFAULT N'MatKhau' CHECK (PhuongThucDN IN (N'MatKhau', N'OTP')),
    TrangThai NVARCHAR(50) NOT NULL DEFAULT N'Hoạt động' CHECK (TrangThai IN (N'Hoạt động', N'Bị khóa')),
    NgayTao DATETIME NOT NULL DEFAULT GETDATE(),
    NgaySaoLuu DATETIME NULL,
    NgayCapNhat DATETIME NOT NULL DEFAULT GETDATE()
);
GO

-- Bảng Nhà (House)
CREATE TABLE Nha (
    MaNha VARCHAR(20) PRIMARY KEY,
    MaNguoiDung VARCHAR(20) NOT NULL FOREIGN KEY (MaNguoiDung) REFERENCES NguoiDung(MaNguoiDung),
    DiaChi NVARCHAR(255) NOT NULL,
    TongSoPhong INT NOT NULL DEFAULT 10 CHECK (TongSoPhong >= 5 AND TongSoPhong <= 10),
    TongSoPhongHienTai INT NOT NULL DEFAULT 0 CHECK (TongSoPhongHienTai >= 0),
    GhiChu NVARCHAR(MAX) NULL,
    NgayTao DATETIME NOT NULL DEFAULT GETDATE(),
    NgayCapNhat DATETIME NOT NULL DEFAULT GETDATE()
);
GO

-- Bảng Phòng (Room)
CREATE TABLE Phong (
    MaPhong VARCHAR(20) PRIMARY KEY,
    MaNha VARCHAR(20) NOT NULL FOREIGN KEY (MaNha) REFERENCES Nha(MaNha),
    LoaiPhong NVARCHAR(100) NOT NULL DEFAULT N'Phòng trống' CHECK (LoaiPhong IN (N'Phòng trống', N'Phòng có đồ cơ bản')),
    DienTich DECIMAL(10,2) NULL CHECK (DienTich IS NULL OR DienTich > 0),
    GiaThue DECIMAL(15,2) NOT NULL CHECK (GiaThue >= 0),
    TrangThai NVARCHAR(50) NOT NULL DEFAULT N'Trống' CHECK (TrangThai IN (N'Trống', N'Đang thuê', N'Dự kiến', N'Bảo trì')),
    SoNguoiHienTai INT NOT NULL DEFAULT 0 CHECK (SoNguoiHienTai >= 0 AND SoNguoiHienTai <= 4),
    GhiChu NVARCHAR(MAX) NULL,
    NgayTao DATETIME NOT NULL DEFAULT GETDATE(),
    NgayCapNhat DATETIME NOT NULL DEFAULT GETDATE()
);
GO

-- Bảng Người Thuê (Tenant) - CHỈ CHỨA THÔNG TIN CÁ NHÂN
CREATE TABLE NguoiThue (
    MaNguoiThue VARCHAR(20) PRIMARY KEY,
    HoTen NVARCHAR(100) NOT NULL,
    SoDienThoai VARCHAR(20) NULL,
    Email VARCHAR(100) NULL,
    SoGiayTo VARCHAR(50) NULL, --CCCD/CMND, Hộ chiếu,...
    NgayTao DATETIME NOT NULL DEFAULT GETDATE(),
    NgayCapNhat DATETIME NOT NULL DEFAULT GETDATE()
    -- ĐÃ XÓA MaPhong, NgayBatDauThue, TrangThaiThue, NgayDonVao, NgayDonRa, VaiTro
);
GO

-- Bảng Hợp Đồng (Contract)
CREATE TABLE HopDong (
    MaHopDong VARCHAR(20) PRIMARY KEY,
    MaPhong VARCHAR(20) NOT NULL FOREIGN KEY (MaPhong) REFERENCES Phong(MaPhong),
    -- ĐÃ XÓA MaNguoiThue
    ChuNha VARCHAR(20) NOT NULL FOREIGN KEY (ChuNha) REFERENCES NguoiDung(MaNguoiDung),
    TienCoc DECIMAL(15,2) NOT NULL DEFAULT 0 CHECK (TienCoc >= 0),
    NgayBatDau DATE NOT NULL,
    ThoiHan INT NOT NULL DEFAULT 12, -- Bsung để làm HĐ, tính theo tháng
    NgayKetThuc AS (DATEADD(month, ThoiHan, NgayBatDau)) PERSISTED,
    FileDinhKem AS 'mau-hop-dong-thue-nha-o.docx',
    TrangThai NVARCHAR(50) NOT NULL DEFAULT N'Hiệu lực' CHECK (TrangThai IN (N'Hiệu lực', N'Hết hạn')),
    GhiChu NVARCHAR(MAX) NULL,
    NgayTao DATETIME NOT NULL DEFAULT GETDATE(),
    NgayCapNhat DATETIME NOT NULL DEFAULT GETDATE()
);
GO

-- Bảng Chi tiết người thuê trong hợp đồng (Contract_Tennant Detail) --> ĐƯỢC THÊM MỚI
CREATE TABLE HopDong_NguoiThue (
    MaHopDong VARCHAR(20) NOT NULL FOREIGN KEY (MaHopDong) REFERENCES HopDong(MaHopDong) ON DELETE CASCADE,
    MaNguoiThue VARCHAR(20) NOT NULL FOREIGN KEY (MaNguoiThue) REFERENCES NguoiThue(MaNguoiThue) ON DELETE CASCADE,
    VaiTro NVARCHAR(20) NOT NULL CHECK (VaiTro IN (N'Chủ hợp đồng', N'Người ở cùng')),
    TrangThaiThue NVARCHAR(50) NOT NULL DEFAULT N'Đang ở' CHECK (TrangThaiThue IN (N'Đang ở', N'Đã trả phòng', N'Lịch hẹn trả')),
    NgayDonVao DATE NULL,
    NgayDonRa DATE NULL,
    NgayBatDauThue DATE NULL,
    PRIMARY KEY (MaHopDong, MaNguoiThue),
    CONSTRAINT CK_HopDongNguoiThue_NgayDonRa CHECK (NgayDonRa IS NULL OR NgayDonRa >= NgayDonVao)
);
GO

-- Bảng Thông Báo Hạn Hợp Đồng (Contract_Notification)
CREATE TABLE ThongBaoHan (
    MaThongBao VARCHAR(20) PRIMARY KEY,
    MaHopDong VARCHAR(20) NOT NULL FOREIGN KEY (MaHopDong) REFERENCES HopDong(MaHopDong),
    NoiDung NVARCHAR(500) NOT NULL,
    NgayThongBao DATE NOT NULL,
    TrangThai NVARCHAR(50) NOT NULL DEFAULT N'Chưa thông báo' CHECK (TrangThai IN (N'Chưa thông báo', N'Đã thông báo')),
    NgayTao DATETIME NOT NULL DEFAULT GETDATE()
);
GO

-- Bảng Dịch Vụ (Service)
CREATE TABLE DichVu (
    MaDichVu VARCHAR(20) PRIMARY KEY,
    TenDichVu NVARCHAR(100) NOT NULL,
    DVT NVARCHAR(50) NOT NULL,
    DonGia DECIMAL(15,2) NOT NULL CHECK (DonGia >= 0),
    NguonThuThap NVARCHAR(200) NULL,
    NgayTao DATETIME NOT NULL DEFAULT GETDATE(),
    NgayCapNhat DATETIME NOT NULL DEFAULT GETDATE()
);
GO

-- Bảng Cài Đặt Phí (Fee Configuration)
CREATE TABLE CaiDatPhi (
    MaCaiDat VARCHAR(20) PRIMARY KEY,
    MaDichVu VARCHAR(20) NOT NULL FOREIGN KEY (MaDichVu) REFERENCES DichVu(MaDichVu),
    GiaMoi DECIMAL(15,2) NOT NULL CHECK (GiaMoi >= 0),
    NgayApDung DATE NOT NULL,
    GhiChu NVARCHAR(MAX) NULL,
    NgayTao DATETIME NOT NULL DEFAULT GETDATE(),
    NgayCapNhat DATETIME NOT NULL DEFAULT GETDATE()
);
GO

-- Bảng Chỉ Số Điện (Electricity Meter)
CREATE TABLE ChiSoDien (
    MaChiSoDien VARCHAR(20) PRIMARY KEY,
    MaDichVu VARCHAR(20) NOT NULL FOREIGN KEY (MaDichVu) REFERENCES DichVu(MaDichVu),
	MaPhong VARCHAR(20) NOT NULL FOREIGN KEY (MaPhong) REFERENCES Phong(MaPhong),
    DonGia DECIMAL(15,2) NOT NULL CHECK (DonGia >= 0),
    NgayGhiThangTruoc DATE NULL,
    ChiSoThangTruoc DECIMAL(18,2) NOT NULL DEFAULT 0 CHECK (ChiSoThangTruoc >= 0),
    NgayGhiThangNay DATE NULL,
    ChiSoThangNay DECIMAL(18,2) NOT NULL DEFAULT 0,
    MucTieuThu AS (CASE WHEN ChiSoThangNay - ChiSoThangTruoc < 0 THEN 0 ELSE (ChiSoThangNay - ChiSoThangTruoc) END) PERSISTED,
    ThanhTien AS (CASE WHEN ChiSoThangNay - ChiSoThangTruoc < 0 THEN 0 ELSE (ChiSoThangNay - ChiSoThangTruoc) * DonGia END) PERSISTED,
    NguonThuThap NVARCHAR(200) NULL,
    NgayTao DATETIME NOT NULL DEFAULT GETDATE(),
    NgayCapNhat DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT CK_ChiSoDien_ThangNay CHECK (ChiSoThangNay >= 0 AND ChiSoThangNay >= ChiSoThangTruoc)
);
GO

-- Bảng Chỉ Số Nước (Water Meter)
CREATE TABLE ChiSoNuoc (
    MaChiSoNuoc VARCHAR(20) PRIMARY KEY,
    MaDichVu VARCHAR(20) NOT NULL FOREIGN KEY (MaDichVu) REFERENCES DichVu(MaDichVu),
	MaPhong VARCHAR(20) NOT NULL FOREIGN KEY (MaPhong) REFERENCES Phong(MaPhong),
    LoaiDongHo NVARCHAR(100) NOT NULL CHECK (LoaiDongHo IN (N'Thủy Cục', N'Toilet', N'Giếng Trời')),
    NgayGhiThangTruoc DATE NULL,
    ChiSoThangTruoc DECIMAL(18,2) NOT NULL DEFAULT 0 CHECK (ChiSoThangTruoc >= 0),
    NgayGhiThangNay DATE NULL,
    ChiSoThangNay DECIMAL(18,2) NOT NULL DEFAULT 0,
    MucTieuThu AS (CASE WHEN ChiSoThangNay - ChiSoThangTruoc < 0 THEN 0 ELSE (ChiSoThangNay - ChiSoThangTruoc) END) PERSISTED,
    NguonThuThap NVARCHAR(200) NULL,
    NgayTao DATETIME NOT NULL DEFAULT GETDATE(),
    NgayCapNhat DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT CK_ChiSoNuoc_ThangNay CHECK (ChiSoThangNay >= 0 AND ChiSoThangNay >= ChiSoThangTruoc)
);
GO

-- Bảng Chi Tiết Đồng Hồ Nước (Water Meter Detail)
CREATE TABLE ChiTietDongHoNuoc (
    MaChiSoNuoc VARCHAR(20) NOT NULL PRIMARY KEY FOREIGN KEY (MaChiSoNuoc) REFERENCES ChiSoNuoc(MaChiSoNuoc),
    DonGia DECIMAL(15,2) NOT NULL CHECK (DonGia >= 0),
    MucTieuThu DECIMAL(18,2) NOT NULL DEFAULT 0 CHECK (MucTieuThu >= 0),
    ThanhTien AS (MucTieuThu * DonGia) PERSISTED,
    NgayTao DATETIME NOT NULL DEFAULT GETDATE(),
    NgayCapNhat DATETIME NOT NULL DEFAULT GETDATE()
);
GO

-- Bảng Hóa Đơn (Invoice)
CREATE TABLE HoaDon (
    MaHoaDon VARCHAR(20) PRIMARY KEY,
    MaPhong VARCHAR(20) NOT NULL FOREIGN KEY (MaPhong) REFERENCES Phong(MaPhong),
    ThoiKy VARCHAR(20) NOT NULL,
    NgayTao DATETIME NOT NULL DEFAULT GETDATE(),
    NgayCapNhat DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT UK_HoaDon_MaPhong_ThoiKy UNIQUE (MaPhong, ThoiKy)
);
GO

-- Bảng Chi Tiết Hóa Đơn (Invoice Detail)
CREATE TABLE ChiTietHoaDon (
    MaHoaDon VARCHAR(20) NOT NULL FOREIGN KEY (MaHoaDon) REFERENCES HoaDon(MaHoaDon) ON DELETE CASCADE,
    MaDichVu VARCHAR(20) NOT NULL FOREIGN KEY (MaDichVu) REFERENCES DichVu(MaDichVu) ON DELETE CASCADE,
    DVT NVARCHAR(50) NOT NULL,
    DonGia DECIMAL(15,2) NOT NULL CHECK (DonGia >= 0),
    SoLuong DECIMAL(18,2) NOT NULL DEFAULT 0 CHECK (SoLuong >= 0),
    ThanhTien AS (DonGia * SoLuong) PERSISTED,
    NgayTao DATETIME NOT NULL DEFAULT GETDATE(),
    NgayCapNhat DATETIME NOT NULL DEFAULT GETDATE()
	PRIMARY KEY (MaHoaDon, MaDichVu),
);
GO

-- Bảng Thông Báo Phí (Fee Notification)
CREATE TABLE ThongBaoPhi (
    MaThongBaoPhi VARCHAR(20) PRIMARY KEY,
    MaPhong VARCHAR(20) NOT NULL FOREIGN KEY (MaPhong) REFERENCES Phong(MaPhong),
    ThoiKy VARCHAR(20) NOT NULL,
    TongTien DECIMAL(18,2) NOT NULL CHECK (TongTien >= 0),
    FileDinhKem VARCHAR(255) NULL,
    NgayGui DATE NULL,
    TrangThai NVARCHAR(50) NOT NULL DEFAULT N'Chưa gửi' CHECK (TrangThai IN (N'Chưa gửi', N'Đã gửi')),
    NgayTao DATETIME NOT NULL DEFAULT GETDATE(),
    NgayCapNhat DATETIME NOT NULL DEFAULT GETDATE()
);
GO

-- Bảng Thanh Toán (Payment)
CREATE TABLE [dbo].[ThanhToan] (
    [MaThanhToan]         VARCHAR (20)    NOT NULL,
    [MaPhong]             VARCHAR (20)    NOT NULL,
    [MaHoaDon]            VARCHAR (20)    NULL,
    [MaHopDong]           VARCHAR (20)    NULL,
    [MaThongBaoPhi]       VARCHAR (20)    NULL,
    [TongCongNo]          DECIMAL (18, 2) NOT NULL,
    [SoTienDaThanhToan]   DECIMAL (18, 2) DEFAULT ((0)) NOT NULL,
    [SoTienConLai]        AS              ([TongCongNo]-[SoTienDaThanhToan]),
    [NgayHanThanhToan]    DATE            NULL,
    [PhuongThucThanhToan] NVARCHAR (50)   DEFAULT (N'Tiền mặt') NOT NULL,
    [TrangThai]           NVARCHAR (50)   DEFAULT (N'Chưa trả') NOT NULL,
    [NgayTao]             DATETIME        DEFAULT (getdate()) NOT NULL,
    [NgayCapNhat]         DATETIME        DEFAULT (getdate()) NOT NULL,
    PRIMARY KEY CLUSTERED ([MaThanhToan] ASC),
    FOREIGN KEY ([MaPhong]) REFERENCES [dbo].[Phong] ([MaPhong]),
    FOREIGN KEY ([MaHoaDon]) REFERENCES [dbo].[HoaDon] ([MaHoaDon]),
    FOREIGN KEY ([MaHopDong]) REFERENCES [dbo].[HopDong] ([MaHopDong]),
    FOREIGN KEY ([MaThongBaoPhi]) REFERENCES [dbo].[ThongBaoPhi] ([MaThongBaoPhi]),
    CHECK ([TongCongNo]>=(0)),
    CHECK ([SoTienDaThanhToan]>=(0)),
    CHECK ([PhuongThucThanhToan]=N'Chuyển khoản' OR [PhuongThucThanhToan]=N'Tiền mặt'),
    CHECK ([TrangThai]=N'Trả một phần' OR [TrangThai]=N'Chưa trả' OR [TrangThai]=N'Đã trả')
);

-- Bảng Xe Máy (Vehicle)
CREATE TABLE XeMay (
    MaXe VARCHAR(20) PRIMARY KEY,
    MaNguoiThue VARCHAR(20) NOT NULL FOREIGN KEY (MaNguoiThue) REFERENCES NguoiThue(MaNguoiThue),
    MaPhong VARCHAR(20) NOT NULL FOREIGN KEY (MaPhong) REFERENCES Phong(MaPhong),
    BienSo VARCHAR(50) UNIQUE NOT NULL,
    PhiGuiXe DECIMAL(18,2) NOT NULL DEFAULT 0 CHECK (PhiGuiXe >= 0),
    NgayTao DATETIME NOT NULL DEFAULT GETDATE(),
    NgayCapNhat DATETIME NOT NULL DEFAULT GETDATE()
);
GO

-- Bảng Thú Cưng (Pet)
CREATE TABLE ThuCung (
    MaThuCung VARCHAR(20) PRIMARY KEY,
    MaPhong VARCHAR(20) NOT NULL FOREIGN KEY (MaPhong) REFERENCES Phong(MaPhong),
    MaNguoiThue VARCHAR(20) NOT NULL FOREIGN KEY (MaNguoiThue) REFERENCES NguoiThue(MaNguoiThue),
    Loai NVARCHAR(50) NOT NULL,
    SoLuong INT NOT NULL DEFAULT 0 CHECK (SoLuong >= 0),
    GhiChu NVARCHAR(MAX) NULL,
    NgayTao DATETIME NOT NULL DEFAULT GETDATE(),
    NgayCapNhat DATETIME NOT NULL DEFAULT GETDATE()
);
GO

-- Bảng Vân Tay (Fingerprint)
CREATE TABLE VanTay (
    MaVanTay VARCHAR(20) PRIMARY KEY,
    MaPhong VARCHAR(20) NOT NULL FOREIGN KEY (MaPhong) REFERENCES Phong(MaPhong),
    MaNguoiThue VARCHAR(20) NOT NULL FOREIGN KEY (MaNguoiThue) REFERENCES NguoiThue(MaNguoiThue),
    TrangThai NVARCHAR(50) NOT NULL DEFAULT N'Chưa cài' CHECK (TrangThai IN ( N'Chưa cài', N'Đã cài', N'Đã vô hiệu hóa')),
    NgayCaiDat DATE NOT NULL,
    NgayVoHieuHoa DATE NULL,
    NgayTao DATETIME NOT NULL DEFAULT GETDATE(),
    NgayCapNhat DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT CK_VanTay_NgayVoHieuHoa CHECK (NgayVoHieuHoa IS NULL OR NgayVoHieuHoa >= NgayCaiDat)
);
GO

-- Bảng Bảo Trì (Maintenance)
CREATE TABLE BaoTri (
    MaBaoTri VARCHAR(20) PRIMARY KEY,
    MaPhong VARCHAR(20) NOT NULL FOREIGN KEY (MaPhong) REFERENCES Phong(MaPhong),
    MaNguoiThue VARCHAR(20) NULL FOREIGN KEY (MaNguoiThue) REFERENCES NguoiThue(MaNguoiThue),
    MoTa NVARCHAR(MAX) NOT NULL,
    TrangThaiXuLy NVARCHAR(50) NOT NULL DEFAULT N'Chưa xử lý' CHECK (TrangThaiXuLy IN (N'Chưa xử lý', N'Đang xử lý', N'Hoàn tất')),
    NgayYeuCau DATE NOT NULL,
    NgayHoanThanh DATE NULL,
    ChiPhi DECIMAL(18,2) NOT NULL DEFAULT 0 CHECK (ChiPhi >= 0),
    NgayTao DATETIME NOT NULL DEFAULT GETDATE(),
    NgayCapNhat DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT CK_BaoTri_NgayHoanThanh CHECK (NgayHoanThanh IS NULL OR NgayHoanThanh >= NgayYeuCau)
);
GO

-- Bảng Báo Cáo Tài Chính (Financial Report)
CREATE TABLE BaoCaoTaiChinh (
    MaBaoCaoTaiChinh VARCHAR(20) PRIMARY KEY,
    ThoiGian DATE NOT NULL,
    TongDoanhThu DECIMAL(18,2) NOT NULL DEFAULT 0 CHECK (TongDoanhThu >= 0),
    TongChiPhi DECIMAL(18,2) NOT NULL DEFAULT 0 CHECK (TongChiPhi >= 0),
    LoiNhuan AS (ISNULL(TongDoanhThu,0) - ISNULL(TongChiPhi,0)) PERSISTED,
    FileXuat VARCHAR(255) NULL,
    NgayTao DATETIME NOT NULL DEFAULT GETDATE(),
    NgayCapNhat DATETIME NOT NULL DEFAULT GETDATE()
);
GO

-- Bảng Báo Cáo Trạng Thái (Status Report)
CREATE TABLE BaoCaoTrangThai (
    MaBaoCaoTrangThai VARCHAR(20) PRIMARY KEY,
    LoaiBaoCao NVARCHAR(100) NOT NULL,
    ThoiGian DATE NOT NULL,
    MaPhong VARCHAR(20) NULL FOREIGN KEY (MaPhong) REFERENCES Phong(MaPhong),
    MaNguoiThue VARCHAR(20) NULL FOREIGN KEY (MaNguoiThue) REFERENCES NguoiThue(MaNguoiThue),
    TrangThaiPhong NVARCHAR(50) NULL,
    CongNo DECIMAL(18,2) NULL CHECK (CongNo >= 0),
    TienThue DECIMAL(18,2) NULL CHECK (TienThue >= 0),
    TrangThaiThanhToan NVARCHAR(50) NULL,
    FileXuat VARCHAR(255) NULL,
    NgayTao DATETIME NOT NULL DEFAULT GETDATE(),
    NgayCapNhat DATETIME NOT NULL DEFAULT GETDATE()
);
GO




CREATE TABLE LichSuHopDong (
    MaLichSu INT IDENTITY(1,1) PRIMARY KEY,
    MaHopDong VARCHAR(20) NOT NULL FOREIGN KEY REFERENCES HopDong(MaHopDong) ON DELETE CASCADE,
    NgayThayDoi DATETIME NOT NULL DEFAULT GETDATE(),
    MaNguoiThayDoi VARCHAR(20) NOT NULL FOREIGN KEY REFERENCES NguoiDung(MaNguoiDung),
    HanhDong NVARCHAR(100) NOT NULL, -- Ví dụ: 'Tạo mới', 'Cập nhật', 'Gia hạn'
    NoiDungThayDoi NVARCHAR(MAX) NULL -- Mô tả chi tiết thay đổi
);
GO

-- DỮ LIỆU THÊM SẴN (ĐÃ CẬP NHẬT)
INSERT INTO NguoiDung (MaNguoiDung, TenDangNhap, MatKhau, TenTaiKhoan, SoDienThoai, PhuongThucDN, TrangThai)
VALUES ('ND001', 'nguyenthimydungntmd39@gmail.com', 'pass123', N'My Dung', NULL, N'MatKhau', N'Hoạt động');

INSERT INTO DichVu (MaDichVu, TenDichVu, DVT, DonGia)
VALUES
('DV1', N'Điện', N'kWh', 3500),
('DV2', N'Nước', N'm3', 15000),
('DV3', N'Internet', N'tháng', 50000),
('DV4', N'Rác', N'phòng/tháng', 30000), 
('DV5', N'Gửi xe máy', N'xe/tháng', 100000), 
('DV6', N'Phí trễ hạn', N'ngày', 20000),
('DV7', N'Phí giặt máy', N'người/tháng', 35000);
GO

INSERT INTO Nha (MaNha, MaNguoiDung, DiaChi, TongSoPhong, TongSoPhongHienTai, GhiChu)
VALUES ('NHA001', 'ND001', N'123 Lê Lợi, Q.1, TP.HCM', 7, 2, N'Nhà trọ 7 phòng, 2 đang thuê');
GO

INSERT INTO Phong (MaPhong, MaNha, LoaiPhong, DienTich, GiaThue, TrangThai, SoNguoiHienTai, GhiChu)
VALUES
('PHONG001', 'NHA001', N'Phòng trống', 20.0, 2500000, N'Đang thuê', 1, N'Có ban công'),
('PHONG002', 'NHA001', N'Phòng có đồ cơ bản', 30.0, 3500000, N'Đang thuê', 3, N'Phòng rộng rãi'),
('PHONG003', 'NHA001', N'Phòng trống', 18.0, 2300000, N'Trống', 0, N'Đang sơn lại');
GO

INSERT INTO NguoiThue (MaNguoiThue, HoTen, SoDienThoai, Email, SoGiayTo)
VALUES
('NT001', N'Nguyễn Vân Anh', '0909000111', 'vananh@gmail.com', '123456789'),
('NT002', N'Lê Thị Bích', '0909000222', 'lebich@gmail.com', '987654321'),
('NT003', N'Trần Ngọc Minh Châu', '0909000333', 'minhchau@gmail.com', '647812058'),
('NT004', N'Tạ Minh Ngọc Hân', '0567789995', 'ngochanta@gmail.com', '047751236');

GO

INSERT INTO HopDong (MaHopDong, MaPhong, ChuNha, TienCoc, NgayBatDau, ThoiHan, TrangThai)
VALUES
('HD001', 'PHONG001', 'ND001', 2500000, '2025-01-01', 12, N'Hiệu lực'),
('HD002', 'PHONG002', 'ND001', 3500000, '2025-02-01', 12, N'Hiệu lực');
GO

INSERT INTO HopDong_NguoiThue (MaHopDong, MaNguoiThue, VaiTro, TrangThaiThue, NgayDonVao, NgayBatDauThue)
VALUES
('HD001', 'NT001', N'Chủ hợp đồng', N'Đang ở', '2025-01-01', '2025-01-01'),
('HD002', 'NT002', N'Chủ hợp đồng', N'Đang ở', '2025-02-01', '2025-02-01'),
('HD002', 'NT003', N'Người ở cùng', N'Đang ở', '2025-02-01', '2025-02-01'),
('HD002', 'NT004', N'Người ở cùng', N'Đang ở', '2025-02-01', '2025-02-01');
GO


INSERT INTO BaoTri (MaBaoTri, MaPhong, MaNguoiThue, MoTa, TrangThaiXuLy, NgayYeuCau, NgayHoanThanh, ChiPhi) 
VALUES 
('BT001', 'PHONG001', 'NT001', N'Vòi nước bồn rửa mặt bị rò rỉ.', N'Hoàn tất', '2025-03-10', '2025-03-11', 150000),
('BT002', 'PHONG002', 'NT002', N'Bóng đèn chính của phòng bị cháy.', N'Đang xử lý', '2025-04-01', NULL, 0);
GO

INSERT INTO ThongBaoPhi (MaThongBaoPhi, MaPhong, ThoiKy, TongTien, FileDinhKem, NgayGui, TrangThai)
VALUES
('TBP001', 'PHONG001', '10/2025', 2500000 + (80*3500) + (15*15000) + 50000, 'TBP001.pdf', '2025-10-02', N'Đã gửi'),
('TBP002', 'PHONG002', '10/2025', 3500000 + (90*3500) + (20*15000) + 50000, 'TBP002.pdf', '2025-10-02', N'Đã gửi');
GO

INSERT INTO HoaDon (MaHoaDon, MaPhong, ThoiKy)
VALUES
('HDN001', 'PHONG001', '10/2025'),
('HDN002', 'PHONG002', '10/2025');
GO

INSERT INTO ThanhToan 
(MaThanhToan, MaPhong, MaHoaDon, MaHopDong, MaThongBaoPhi, TongCongNo, SoTienDaThanhToan, NgayHanThanhToan, PhuongThucThanhToan, TrangThai)
VALUES
('TT001', 'PHONG001', 'HDN001', 'HD001', 'TBP001', 
  2500000 + (80 * 3500) + (15 * 15000) + 50000, 
  2500000 + (80 * 3500) + (15 * 15000) + 50000,   -- Đã trả đủ
  '2025-10-10', N'Chuyển khoản', N'Đã trả'),

('TT002', 'PHONG002', 'HDN002', 'HD002', 'TBP002', 
  3500000 + (90 * 3500) + (20 * 15000) + 50000, 
  0,                                              -- Chưa trả đồng nào
  '2025-10-10', N'Tiền mặt', N'Chưa trả');
GO

INSERT INTO ChiTietHoaDon (MaHoaDon, MaDichVu, DVT, DonGia, SoLuong)
VALUES
-- Chi tiết cho hóa đơn HDN001 (Phòng PHONG001)
-- Tiền thuê phòng lấy tự động từ bảng phòng
('HDN001', 'DV1', N'kWh', 3500, 80),      -- Tiền điện
('HDN001', 'DV2', N'm3', 15000, 15),      -- Tiền nước

-- Chi tiết cho hóa đơn HDN002 (Phòng PHONG002)
-- Tiền thuê phòng lấy tự động từ bảng phòng
('HDN002', 'DV1', N'kWh', 3500, 90),      -- Tiền điện
('HDN002', 'DV5', N'xe/tháng', 100000, 2);  -- Tiền gửi xe
GO

-- Thêm tài khoản đăng nhập cho My Dung
--INSERT INTO NguoiDung (MaNguoiDung, TenDangNhap, MatKhau, TenTaiKhoan, SoDienThoai, PhuongThucDN, TrangThai)
--VALUES ('ND002', 'nguyenthimydungntmd39@gmail.com', 'pass123', N'My Dung', NULL, N'MatKhau', N'Hoạt động');
--GO


