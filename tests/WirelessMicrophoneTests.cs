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

public class WirelessMicrophoneTests
{
    [Fact]
    public void UpdateBatteryLevel_ShouldRaiseWarning_WhenBelow30()
    {
        // Arrange
        var mic = new WirelessMicrophone("MIC-01", "Micro Giảng đường");
        DeviceAlertEventArgs? alertArgs = null;
        mic.AlertRaised += (sender, e) => alertArgs = e;

        // Act
        mic.UpdateBatteryLevel(25);

        // Assert
        Assert.Equal(25, mic.BatteryLevel);
        Assert.NotNull(alertArgs);
        Assert.Equal(AlertSeverity.Warning, alertArgs.Severity);
    }

    [Fact]
    public void UpdateBatteryLevel_ShouldRaiseCritical_WhenZero()
    {
        // Arrange
        var mic = new WirelessMicrophone("MIC-01", "Micro Giảng đường");
        DeviceAlertEventArgs? alertArgs = null;
        mic.AlertRaised += (sender, e) => alertArgs = e;

        // Act
        mic.UpdateBatteryLevel(0);

        // Assert
        Assert.Equal(0, mic.BatteryLevel);
        Assert.NotNull(alertArgs);
        Assert.Equal(AlertSeverity.Critical, alertArgs.Severity);
    }

    [Fact]
    public void UpdateSignalQuality_ShouldRaiseWarning_WhenBelow40()
    {
        // Arrange
        var mic = new WirelessMicrophone("MIC-01", "Micro Giảng đường");
        DeviceAlertEventArgs? alertArgs = null;
        mic.AlertRaised += (sender, e) => alertArgs = e;

        // Act
        mic.UpdateSignalQuality(30);

        // Assert
        Assert.Equal(30, mic.SignalQuality);
        Assert.NotNull(alertArgs);
        Assert.Equal(AlertSeverity.Warning, alertArgs.Severity);
    }

    [Fact]
    public void UpdateSignalQuality_ShouldRaiseCritical_WhenZero()
    {
        // Arrange
        var mic = new WirelessMicrophone("MIC-01", "Micro Giảng đường");
        DeviceAlertEventArgs? alertArgs = null;
        mic.AlertRaised += (sender, e) => alertArgs = e;

        // Act
        mic.UpdateSignalQuality(0);

        // Assert
        Assert.Equal(0, mic.SignalQuality);
        Assert.NotNull(alertArgs);
        Assert.Equal(AlertSeverity.Critical, alertArgs.Severity);
    }

    [Fact]
    public void UpdateBatteryLevel_ShouldThrowInvalidDeviceReadingException_WhenValueOutOfRange()
    {
        // Arrange
        var mic = new WirelessMicrophone("MIC-01", "Micro Giảng đường");

        // Act & Assert
        var ex = Assert.Throws<InvalidDeviceReadingException>(() => mic.UpdateBatteryLevel(-5));
        Assert.Equal("MIC-01", ex.DeviceId);
        Assert.Equal(-5, ex.InvalidValue);
    }
}
