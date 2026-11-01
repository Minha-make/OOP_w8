/****************/
// MSSV: 202418890
// Họ và tên: Nguyễn Minh Hà
/****************/

using TeachingDeviceManagement.Devices;
using TeachingDeviceManagement.Enums;
using TeachingDeviceManagement.Events;
using Xunit;

namespace TeachingDeviceManagement.Tests;

/// <summary>
/// Lớp thiết bị cụ thể dùng cho việc kiểm thử TeachingDevice
/// </summary>
internal class TestDevice : TeachingDevice
{
    public TestDevice(string id, string name) : base(id, name)
    {
    }

    public void TriggerAlert(string message, AlertSeverity severity)
    {
        RaiseAlert(message, severity);
    }
}

public class TeachingDeviceTests
{
    [Fact]
    public void ChangeStatus_ShouldUpdateStatusAndRaiseEvent_WhenStatusIsDifferent()
    {
        // Arrange
        var device = new TestDevice("DEV-01", "Thiết bị thử nghiệm");
        DeviceStatusChangedEventArgs? receivedArgs = null;
        device.StatusChanged += (sender, e) => receivedArgs = e;

        // Act
        device.ChangeStatus(DeviceStatus.Borrowed);

        // Assert
        Assert.Equal(DeviceStatus.Borrowed, device.Status);
        Assert.NotNull(receivedArgs);
        Assert.Equal("DEV-01", receivedArgs.DeviceId);
        Assert.Equal(DeviceStatus.Available, receivedArgs.OldStatus);
        Assert.Equal(DeviceStatus.Borrowed, receivedArgs.NewStatus);
    }

    [Fact]
    public void ChangeStatus_ShouldNotRaiseEvent_WhenStatusIsSame()
    {
        // Arrange
        var device = new TestDevice("DEV-01", "Thiết bị thử nghiệm");
        bool eventFired = false;
        device.StatusChanged += (sender, e) => eventFired = true;

        // Act: Đổi sang cùng trạng thái Available hiện tại
        device.ChangeStatus(DeviceStatus.Available);

        // Assert
        Assert.False(eventFired);
        Assert.Equal(DeviceStatus.Available, device.Status);
    }

    [Fact]
    public void ChangeStatus_InvalidEnumValue_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var device = new TestDevice("DEV-01", "Thiết bị thử nghiệm");

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => device.ChangeStatus((DeviceStatus)999));
    }

    [Theory]
    [InlineData(null, "Tên hợp lệ")]
    [InlineData("", "Tên hợp lệ")]
    [InlineData("   ", "Tên hợp lệ")]
    [InlineData("ID-01", null)]
    [InlineData("ID-01", "")]
    [InlineData("ID-01", "   ")]
    public void Constructor_NullOrWhitespaceIdOrName_ShouldThrowArgumentException(string? id, string? name)
    {
        Assert.Throws<ArgumentException>(() => new TestDevice(id!, name!));
    }

    [Fact]
    public void EventStability_WhenOneSubscriberThrows_RemainingSubscribersShouldStillExecute()
    {
        // Arrange
        var device = new TestDevice("DEV-01", "Thiết bị thử nghiệm");
        bool secondSubscriberRan = false;

        device.AlertRaised += (sender, e) => throw new InvalidOperationException("Lỗi từ subscriber 1");
        device.AlertRaised += (sender, e) => secondSubscriberRan = true;

        // Act & Assert
        var aggEx = Assert.Throws<AggregateException>(() => device.TriggerAlert("Cảnh báo test", AlertSeverity.Warning));
        
        // Subscriber thứ 2 vẫn phải được chạy dù subscriber thứ 1 ném exception
        Assert.True(secondSubscriberRan);
        Assert.Single(aggEx.InnerExceptions);
        Assert.IsType<InvalidOperationException>(aggEx.InnerExceptions[0]);
    }
}
