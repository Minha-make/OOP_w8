/****************/
// MSSV: 202418890
// Họ và tên: Nguyễn Minh Hà
/****************/

using System.Text;
using TeachingDeviceManagement.Devices;
using TeachingDeviceManagement.Enums;
using TeachingDeviceManagement.Events;
using TeachingDeviceManagement.Exceptions;
using TeachingDeviceManagement.Services;

namespace TeachingDeviceManagement;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        Console.WriteLine("======================================================================");
        Console.WriteLine("   BÀI LAB: DELEGATE & EVENT - HỆ THỐNG QUẢN LÝ THIẾT BỊ PHÒNG DẠY");
        Console.WriteLine("   MSSV: 202418890 - Họ và tên: Nguyễn Minh Hà");
        Console.WriteLine("======================================================================\n");

        // 1. Khởi tạo 3 thiết bị
        Console.WriteLine("--- 1. KHỞI TẠO CÁC THIẾT BỊ ---");
        var projector = new ProjectorRemote("PRJ-01", "Điều khiển máy chiếu Giảng đường A", initialBattery: 100);
        var airConditioner = new AirConditionerRemote("AC-01", "Điều khiển điều hòa P.302", assignedRoom: "Phòng 302");
        var microphone = new WirelessMicrophone("MIC-01", "Micro không dây Hội trường B", initialBattery: 100, initialSignal: 100);

        var devices = new List<TeachingDevice> { projector, airConditioner, microphone };
        Console.WriteLine($"-> Đã tạo {devices.Count} thiết bị: {projector.Name}, {airConditioner.Name}, {microphone.Name}.\n");

        // 2. Khởi tạo các subscriber
        Console.WriteLine("--- 2. KHỞI TẠO CÁC DỊCH VỤ SUBSCRIBER ---");
        var consoleNotifier = new ConsoleNotificationService();
        var auditLogger = new DeviceAuditLogger();
        var maintenanceService = new MaintenanceService();

        // FilteredAlertSubscriber: chỉ xử lý cảnh báo mức Critical
        var criticalOnlySubscriber = new FilteredAlertSubscriber(
            alert => alert.Severity == AlertSeverity.Critical,
            alert => Console.WriteLine($"   [FILTERED - CHỈ CRITICAL] Phát hiện sự cố khẩn: {alert.DeviceId} - {alert.Message}")
        );

        // 3. Sử dụng Coordinator để đăng ký sự kiện và chống đăng ký trùng
        Console.WriteLine("--- 3. ĐĂNG KÝ SỰ KIỆN QUA COORDINATOR (CHỐNG ĐĂNG KÝ TRÙNG) ---");
        var coordinator = new DeviceMonitoringCoordinator(
            devices,
            consoleNotifier,
            auditLogger,
            maintenanceService);

        coordinator.Subscribe();
        coordinator.Subscribe(); // Gọi lần 2 để kiểm tra tính Idempotent (không bị nhân đôi handler)

        // Đăng ký FilteredAlertSubscriber thêm cho từng thiết bị
        foreach (var device in devices)
        {
            device.AlertRaised += criticalOnlySubscriber.HandleAlert;
        }
        Console.WriteLine("-> Đã đăng ký lắng nghe sự kiện thành công (đảm bảo không đăng ký trùng).\n");

        // 4. Demo thay đổi trạng thái (StatusChanged)
        Console.WriteLine("--- 4. DEMO SỰ KIỆN THAY ĐỔI TRẠNG THÁI (ChangeStatus) ---");
        Console.WriteLine("-> Thay đổi PRJ-01 sang Borrowed:");
        projector.ChangeStatus(DeviceStatus.Borrowed);

        Console.WriteLine("-> Thử chuyển tiếp PRJ-01 sang Borrowed (cùng trạng thái):");
        projector.ChangeStatus(DeviceStatus.Borrowed);
        Console.WriteLine("   (Không phát sinh event do trạng thái không đổi)");

        Console.WriteLine("-> Thay đổi MIC-01 sang Borrowed:");
        microphone.ChangeStatus(DeviceStatus.Borrowed);
        Console.WriteLine();

        // 5. Demo cảnh báo mức Warning (AlertRaised)
        Console.WriteLine("--- 5. DEMO CẢNH BÁO MỨC WARNING (AlertRaised) ---");
        Console.WriteLine("-> Cập nhật pin máy chiếu xuống 15% (<20%):");
        projector.UpdateBatteryLevel(15);

        Console.WriteLine("-> Cập nhật tiếp pin máy chiếu xuống 12% (cùng vùng Warning - kiểm tra chống lặp):");
        projector.UpdateBatteryLevel(12);
        Console.WriteLine("   (Không phát sinh cảnh báo lặp khi vẫn nằm trong vùng cảnh báo)");

        Console.WriteLine("-> Cập nhật pin micro xuống 25% (<30%):");
        microphone.UpdateBatteryLevel(25);

        Console.WriteLine("-> Cập nhật chất lượng tín hiệu micro xuống 35% (<40%):");
        microphone.UpdateSignalQuality(35);
        Console.WriteLine();

        // 6. Demo cảnh báo mức Critical (AlertRaised)
        Console.WriteLine("--- 6. DEMO CẢNH BÁO MỨC CRITICAL (AlertRaised) ---");
        Console.WriteLine("-> AC-01 mất phản hồi (true -> false):");
        airConditioner.UpdateResponseState(false);

        Console.WriteLine("-> AC-01 tiếp tục không phản hồi (false -> false):");
        airConditioner.UpdateResponseState(false);
        Console.WriteLine("   (Không phát sinh cảnh báo lặp lại khi vẫn không phản hồi)");

        Console.WriteLine("-> PRJ-01 pin cạn kiệt về 0%:");
        projector.UpdateBatteryLevel(0);

        Console.WriteLine("-> MIC-01 mất sóng hoàn toàn (0%):");
        microphone.UpdateSignalQuality(0);
        Console.WriteLine();

        // 7. Demo kiểm soát tính ổn định của Event khi Subscriber gặp ngoại lệ
        Console.WriteLine("--- 7. DEMO TÍNH ỔN ĐỊNH CỦA EVENT KHI CÓ SUBSCRIBER LỖI ---");
        bool fallbackSubscriberRan = false;
        EventHandler<DeviceAlertEventArgs> faultyHandler = (sender, e) =>
            throw new InvalidOperationException("Lỗi giả lập từ subscriber không ổn định!");
        EventHandler<DeviceAlertEventArgs> resilientHandler = (sender, e) =>
        {
            fallbackSubscriberRan = true;
            Console.WriteLine("   [Resilient Subscriber] Vẫn nhận được cảnh báo thành công dù subscriber trước bị lỗi!");
        };

        projector.AlertRaised += faultyHandler;
        projector.AlertRaised += resilientHandler;

        try
        {
            // Thiết lập pin sạc lại rồi chuyển về Critical
            projector.UpdateBatteryLevel(80);
            projector.UpdateBatteryLevel(0);
        }
        catch (AggregateException ex)
        {
            Console.WriteLine($"   [Bắt AggregateException] Đã cách ly lỗi subscriber: {ex.InnerExceptions[0].Message}");
            Console.WriteLine($"   Subscriber phía sau có chạy không? -> {fallbackSubscriberRan} (Đảm bảo tính ổn định)");
        }
        finally
        {
            projector.AlertRaised -= faultyHandler;
            projector.AlertRaised -= resilientHandler;
        }
        Console.WriteLine();

        // 8. Demo ngoại lệ InvalidDeviceReadingException
        Console.WriteLine("--- 8. DEMO BẮT NGOẠI LỆ ĐỌC THÔNG SỐ NGOÀI KHOẢNG 0-100 ---");
        try
        {
            Console.WriteLine("-> Cố gắng cập nhật pin micro thành 120%:");
            microphone.UpdateBatteryLevel(120);
        }
        catch (InvalidDeviceReadingException ex)
        {
            Console.WriteLine($"   [Bắt ngoại lệ thành công] Thiết bị: {ex.DeviceId}, Giá trị sai: {ex.InvalidValue}");
            Console.WriteLine($"   Thông báo lỗi: {ex.Message}");
        }
        Console.WriteLine();

        // 9. Kiểm tra dữ liệu thu thập từ các Subscriber
        Console.WriteLine("--- 9. KIỂM TRA DỮ LIỆU ĐƯỢC CÁC DỊCH VỤ THU THẬP ---");
        Console.WriteLine($"[AuditLogger] Tổng số lần thay đổi trạng thái: {auditLogger.Entries.Count}");
        foreach (var entry in auditLogger.Entries)
        {
            Console.WriteLine($"   + {entry}");
        }

        Console.WriteLine($"\n[MaintenanceService] Tổng số yêu cầu bảo trì đã tạo: {maintenanceService.Requests.Count}");
        foreach (var req in maintenanceService.Requests)
        {
            Console.WriteLine($"   + {req}");
        }
        Console.WriteLine();

        // 10. Demo Hủy đăng ký (Unsubscribe) và chứng minh không nhận event
        Console.WriteLine("--- 10. DEMO HỦY ĐĂNG KÝ (Unsubscribe) VÀ CHỨNG MINH NGỪNG NHẬN EVENT ---");
        int logCountBefore = auditLogger.Entries.Count;
        int reqCountBefore = maintenanceService.Requests.Count;

        coordinator.Unsubscribe();
        foreach (var device in devices)
        {
            device.AlertRaised -= criticalOnlySubscriber.HandleAlert;
        }
        Console.WriteLine("-> Đã gọi Unsubscribe() và gỡ bỏ tất cả subscriber.");

        Console.WriteLine("-> Thay đổi trạng thái PRJ-01 sang UnderMaintenance và sạc pin lên 50% rồi về 0%:");
        projector.ChangeStatus(DeviceStatus.UnderMaintenance);
        projector.UpdateBatteryLevel(50);
        projector.UpdateBatteryLevel(0);

        int logCountAfter = auditLogger.Entries.Count;
        int reqCountAfter = maintenanceService.Requests.Count;

        Console.WriteLine($"   Log entries trước: {logCountBefore}, sau: {logCountAfter} (Không đổi => Chứng minh thành công)");
        Console.WriteLine($"   Yêu cầu bảo trì trước: {reqCountBefore}, sau: {reqCountAfter} (Không đổi => Chứng minh thành công)");
        Console.WriteLine("   Màn hình console cũng không in thông báo mới nào từ các service.");

        Console.WriteLine("\n======================================================================");
        Console.WriteLine("   KẾT THÚC CHƯƠNG TRÌNH DEMO THÀNH CÔNG");
        Console.WriteLine("======================================================================");
    }
}
