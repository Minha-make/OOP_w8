/****************/
// MSSV: 202418890
// Họ và tên: Nguyễn Minh Hà
/****************/

using TeachingDeviceManagement.Devices;
using TeachingDeviceManagement.Enums;
using TeachingDeviceManagement.Events;
using TeachingDeviceManagement.Exceptions;
using Xunit;

namespace TeachingDeviceManagement.Tests;

public class ProjectorRemoteTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(15)]
    [InlineData(19)]
    public void UpdateBatteryLevel_ShouldRaiseWarning_WhenBatteryIsBelow20(int batteryLevel)
    {
        // Arrange
        var projector = new ProjectorRemote("PRJ-01", "Máy chiếu Hội trường");
        DeviceAlertEventArgs? alertArgs = null;
        projector.AlertRaised += (sender, e) => alertArgs = e;

        // Act
        projector.UpdateBatteryLevel(batteryLevel);

        // Assert
        Assert.Equal(batteryLevel, projector.BatteryLevel);
        Assert.NotNull(alertArgs);
        Assert.Equal(AlertSeverity.Warning, alertArgs.Severity);
        Assert.Equal("PRJ-01", alertArgs.DeviceId);
    }

    [Fact]
    public void UpdateBatteryLevel_ShouldRaiseCritical_WhenBatteryIsZero()
    {
        // Arrange
        var projector = new ProjectorRemote("PRJ-01", "Máy chiếu Hội trường");
        DeviceAlertEventArgs? alertArgs = null;
        projector.AlertRaised += (sender, e) => alertArgs = e;

        // Act
        projector.UpdateBatteryLevel(0);

        // Assert
        Assert.Equal(0, projector.BatteryLevel);
        Assert.NotNull(alertArgs);
        Assert.Equal(AlertSeverity.Critical, alertArgs.Severity);
    }

    [Theory]
    [InlineData(20)]
    [InlineData(21)]
    [InlineData(50)]
    [InlineData(100)]
    public void UpdateBatteryLevel_ShouldNotRaiseAlert_WhenBatteryIsInSafeZone(int safeLevel)
    {
        // Arrange
        var projector = new ProjectorRemote("PRJ-01", "Máy chiếu Hội trường");
        bool alertRaised = false;
        projector.AlertRaised += (sender, e) => alertRaised = true;

        // Act
        projector.UpdateBatteryLevel(safeLevel);

        // Assert
        Assert.False(alertRaised);
        Assert.Equal(safeLevel, projector.BatteryLevel);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-10)]
    [InlineData(101)]
    [InlineData(150)]
    public void UpdateBatteryLevel_ShouldThrowInvalidDeviceReadingException_WhenValueIsOutOfRange(int invalidValue)
    {
        // Arrange
        var projector = new ProjectorRemote("PRJ-01", "Máy chiếu Hội trường");

        // Act & Assert
        var ex = Assert.Throws<InvalidDeviceReadingException>(() => projector.UpdateBatteryLevel(invalidValue));
        Assert.Equal("PRJ-01", ex.DeviceId);
        Assert.Equal(invalidValue, ex.InvalidValue);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void Constructor_InitialBatteryOutOfRange_ShouldThrowException(int invalidBattery)
    {
        Assert.Throws<InvalidDeviceReadingException>(() => new ProjectorRemote("PRJ-01", "Máy chiếu", invalidBattery));
    }

    [Fact]
    public void UpdateBatteryLevel_RepeatedUpdatesInWarningZone_ShouldNotRaiseDuplicateAlert()
    {
        // Arrange
        var projector = new ProjectorRemote("PRJ-01", "Máy chiếu");
        int alertCount = 0;
        projector.AlertRaised += (sender, e) => alertCount++;

        // Act: Lần 1 chuyển vào vùng Warning (15%) -> phát cảnh báo
        projector.UpdateBatteryLevel(15);
        Assert.Equal(1, alertCount);

        // Act: Lần 2 tiếp tục ở trong vùng Warning (10%) -> không phát lặp
        projector.UpdateBatteryLevel(10);
        Assert.Equal(1, alertCount);

        // Act: Sạc pin về vùng an toàn (80%) -> không phát cảnh báo
        projector.UpdateBatteryLevel(80);
        Assert.Equal(1, alertCount);

        // Act: Pin tụt lại vùng Warning (18%) -> phát cảnh báo mới
        projector.UpdateBatteryLevel(18);
        Assert.Equal(2, alertCount);
    }
}
