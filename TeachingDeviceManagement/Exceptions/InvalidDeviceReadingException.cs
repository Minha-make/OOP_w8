/****************/
// MSSV: 202418890
// Họ và tên: Nguyễn Minh Hà
/****************/

namespace TeachingDeviceManagement.Exceptions;

/// <summary>
/// Ngoại lệ phát sinh khi thông số đọc từ thiết bị (như pin, chất lượng tín hiệu) nằm ngoài khoảng 0-100
/// </summary>
public class InvalidDeviceReadingException : Exception
{
    /// <summary>
    /// Mã thiết bị xảy ra ngoại lệ
    /// </summary>
    public string DeviceId { get; }

    /// <summary>
    /// Giá trị đọc không hợp lệ
    /// </summary>
    public int InvalidValue { get; }

    /// <summary>
    /// Khởi tạo ngoại lệ với mã thiết bị và giá trị sai
    /// </summary>
    /// <param name="deviceId">Mã thiết bị</param>
    /// <param name="invalidValue">Giá trị không hợp lệ</param>
    public InvalidDeviceReadingException(string deviceId, int invalidValue)
        : base($"Giá trị đọc không hợp lệ '{invalidValue}' đối với thiết bị '{deviceId}'. Giá trị hợp lệ phải từ 0 đến 100.")
    {
        if (string.IsNullOrWhiteSpace(deviceId))
        {
            throw new ArgumentException("Mã thiết bị không được để trống hoặc chỉ chứa khoảng trắng.", nameof(deviceId));
        }

        DeviceId = deviceId;
        InvalidValue = invalidValue;
    }

    /// <summary>
    /// Khởi tạo ngoại lệ với mã thiết bị, giá trị sai và thông báo tùy biến
    /// </summary>
    /// <param name="deviceId">Mã thiết bị</param>
    /// <param name="invalidValue">Giá trị không hợp lệ</param>
    /// <param name="message">Thông báo lỗi</param>
    public InvalidDeviceReadingException(string deviceId, int invalidValue, string message)
        : base(message)
    {
        if (string.IsNullOrWhiteSpace(deviceId))
        {
            throw new ArgumentException("Mã thiết bị không được để trống hoặc chỉ chứa khoảng trắng.", nameof(deviceId));
        }

        DeviceId = deviceId;
        InvalidValue = invalidValue;
    }

    /// <summary>
    /// Khởi tạo ngoại lệ với mã thiết bị, giá trị sai, thông báo lỗi và ngoại lệ gốc
    /// </summary>
    /// <param name="deviceId">Mã thiết bị</param>
    /// <param name="invalidValue">Giá trị không hợp lệ</param>
    /// <param name="message">Thông báo lỗi</param>
    /// <param name="innerException">Ngoại lệ gốc gây ra lỗi</param>
    public InvalidDeviceReadingException(string deviceId, int invalidValue, string message, Exception innerException)
        : base(message, innerException)
    {
        if (string.IsNullOrWhiteSpace(deviceId))
        {
            throw new ArgumentException("Mã thiết bị không được để trống hoặc chỉ chứa khoảng trắng.", nameof(deviceId));
        }

        DeviceId = deviceId;
        InvalidValue = invalidValue;
    }
}
