# Room Management System - Plumeria Family

Đây là một ứng dụng Desktop được phát triển bằng WPF (.NET 8) nhằm mục đích cung cấp một giải pháp quản lý phòng cho thuê thông minh và đơn giản. Dự án được xây dựng theo kiến trúc đa tầng (N-Layer) để đảm bảo tính dễ bảo trì, mở rộng và kiểm thử.

## 🚀 Công nghệ sử dụng

-   **Ngôn ngữ:** C#
-   **Framework:** .NET 8
-   **Giao diện người dùng (UI):** Windows Presentation Foundation (WPF)
-   **Kiến trúc:**
    -   Kiến trúc đa tầng (N-Layer Architecture)
    -   Mô hình MVVM (Model-View-ViewModel) cho tầng giao diện.

## 📂 Cấu trúc dự án

Dự án được chia thành 4 tầng chính, mỗi tầng là một project riêng biệt trong Solution, có vai trò và trách nhiệm rõ ràng:
```
/RoomManagementSystem
│
├─── RoomManagementSystem.Presentation/ (Tầng Giao diện - UI Layer)
├─── RoomManagementSystem.BusinessLayer/ (Tầng Nghiệp vụ - Business Logic Layer)
├─── RoomManagementSystem.DataLayer/ (Tầng Truy cập Dữ liệu - Data Access Layer)
└─── RoomManagementSystem.Utilities/ (Tầng Tiện ích - Utilities Layer)
```
### 1. `RoomManagementSystem.Presentation`

Đây là tầng giao diện người dùng (UI), là điểm tương tác chính với người dùng.

-   **Nền tảng:** WPF Application.
-   **Kiến trúc:** Tuân thủ nghiêm ngặt theo mô hình **MVVM**.
    -   **`/Views`**: Chứa các file XAML định nghĩa giao diện (Windows, Pages, UserControls). Code-behind (`.xaml.cs`) được giữ ở mức tối thiểu, chỉ xử lý các tác vụ liên quan trực tiếp đến UI mà không thể thực hiện bằng MVVM.
    -   **`/ViewModels`**: Chứa logic trình bày và trạng thái cho các View tương ứng. Đây là nơi xử lý các lệnh (Commands), tương tác người dùng và chuẩn bị dữ liệu để hiển thị thông qua Data Binding.
    -   **`/Resources`**: Chứa các tài nguyên dùng chung cho toàn bộ ứng dụng, tương tự như CSS trong web.
        -   **`/Images`**: Lưu trữ các icon, hình ảnh.
        -   **`/Styles`**: Chứa các `ResourceDictionary` định nghĩa Style cho các control (Button, TextBox, v.v.), màu sắc, font chữ để đảm bảo tính nhất quán.
-   **Vai trò:** Hiển thị dữ liệu từ ViewModel và gửi các hành động của người dùng về ViewModel để xử lý. **Không chứa logic nghiệp vụ.**

### 2. `RoomManagementSystem.BusinessLayer`

Tầng này là "bộ não" của ứng dụng, chứa toàn bộ logic nghiệp vụ cốt lõi.

-   **Nền tảng:** Class Library.
-   **Vai trò:**
    -   Thực thi các quy tắc nghiệp vụ (ví dụ: kiểm tra tính hợp lệ của dữ liệu, tính toán hóa đơn, xử lý quy trình đặt phòng).
    -   Đóng vai trò trung gian, điều phối hoạt động giữa `Presentation Layer` và `Data Layer`.
    -   Tầng này **hoàn toàn độc lập** với công nghệ giao diện (WPF) và công nghệ truy cập dữ liệu (ví dụ: Entity Framework).

### 3. `RoomManagementSystem.DataLayer`

Tầng này chịu trách nhiệm cho mọi tương tác với cơ sở dữ liệu.

-   **Nền tảng:** Class Library.
-   **Vai trò:**
    -   Thực hiện các thao tác CRUD (Create, Read, Update, Delete) với database.
    -   Trừu tượng hóa việc truy cập dữ liệu, giúp các tầng trên không cần quan tâm đến loại cơ sở dữ liệu đang sử dụng (SQL Server, SQLite, v.v.).
    -   Có thể chứa các đối tượng Model (POCO classes) đại diện cho các bảng trong database.

### 4. `RoomManagementSystem.Utilities`

Đây là tầng chứa các mã nguồn có thể tái sử dụng trên toàn bộ dự án.

-   **Nền tảng:** Class Library.
-   **Vai trò:**
    -   Cung cấp các lớp tiện ích (Helpers), phương thức mở rộng (Extension Methods), hoặc các chức năng chung như logging, mã hóa, v.v.
    -   Giúp tránh lặp lại code ở các tầng khác.

## ✨ Tình hình phát triển hiện tại (Tầng Giao diện)

Tầng `Presentation` đã hoàn thành các công việc nền tảng quan trọng sau:

1.  **Màn hình Đăng nhập (`LoginWindow.xaml`):**
    -   Giao diện đã được xây dựng hoàn chỉnh, **chính xác 100%** theo bản thiết kế chi tiết từ Figma (dữ liệu SVG).
    -   Bố cục, màu sắc, font chữ, kích thước, và các hiệu ứng gradient đều được tinh chỉnh để đạt độ chính xác cao nhất.

2.  **Hệ thống Styling tập trung:**
    -   Đã thiết lập cấu trúc `ResourceDictionary` trong thư mục `/Resources` để quản lý Style.
    -   Các Style cho `Button`, `TextBox`, `PasswordBox` đã được định nghĩa và có thể tái sử dụng.
    -   Các control input (TextBox, PasswordBox) hỗ trợ hiển thị icon và placeholder.

3.  **Kiến trúc MVVM:**
    -   Đã thiết lập cấu trúc thư mục cho `Views` và `ViewModels`.
    -   Màn hình Login đã có `LoginViewModel.cs` tương ứng (hiện tại đang trống, sẵn sàng để thêm logic).

4.  **Hỗ trợ màn hình Độ phân giải cao (High-DPI):**
    -   Dự án đã được cấu hình là **DPI-Aware** thông qua file `app.manifest`.
    -   Sử dụng `UseLayoutRounding="True"` để đảm bảo giao diện luôn hiển thị sắc nét trên các màn hình có tỷ lệ scaling khác nhau (125%, 150%...).

5.  **Chức năng cơ bản của cửa sổ:**
    -   Cửa sổ không có viền (`WindowStyle="None"`, `AllowsTransparency="True"`).
    -   Đã cài đặt chức năng kéo thả cửa sổ và các nút Minimize, Maximize, Close.

## 🚀 Bắt đầu

Để chạy dự án trên máy của bạn, hãy làm theo các bước sau:

1.  **Prerequisites:**
    -   Visual Studio 2022 (với .NET desktop development workload).
    -   .NET 8 SDK.
2.  **Clone repository:**
    ```bash
    git clone https://github.com/MyDung39/QuanLyThueNha.git
    ```
3.  Mở file `RoomManagementSystem.sln` bằng Visual Studio.
4.  Chuột phải vào project `RoomManagementSystem.Presentation` và chọn **Set as Startup Project**.
5.  Nhấn **F5** để biên dịch và chạy ứng dụng.

## 🚀 Dữ liệu tài khoản Google và Google Forms lấy chỉ số điện/nước
1.  **Tài khoản Google:**
    - Email: pentanix79@gmail.com
    - Password: TH3cnpmtdtu
2.  **Google Forms (dùng email Pentanix):**
    - Edit form thu thập chỉ số: https://tinyurl.com/ye76s7y6
    - Form người thuê điền: https://tinyurl.com/6y8e4ups
    - Google Sheet lấy thông tin chỉ số: https://tinyurl.com/55ecmdez
