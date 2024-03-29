﻿using CurrieTechnologies.Razor.SweetAlert2;

namespace QEBSPEditor.Services;

public class AlertDialogsService
{
    private SweetAlertService SweetAlert { get; }

    public AlertDialogsService(SweetAlertService sweetAlert)
    {
        SweetAlert = sweetAlert;
    }

    public async Task SuccessAsync(string body) => await SuccessAsync("Success", body);
    public async Task SuccessAsync(string title, string body)
    {
        await SweetAlert.FireAsync(title, body, SweetAlertIcon.Success);
    }

    public async Task FailureAsync(string body) => await FailureAsync("Failure", body);
    public async Task FailureAsync(string title, string body)
    {
        await SweetAlert.FireAsync(title, body, SweetAlertIcon.Error);
    }

    public async Task<bool> ConfirmAsync(string body) => await ConfirmAsync("Are you sure?", body);
    public async Task<bool> ConfirmAsync(string title, string body)
    {
        var result = await SweetAlert.FireAsync(new SweetAlertOptions()
        {
            TitleText = title,
            Text = body,
            Icon = SweetAlertIcon.Warning,
            ShowConfirmButton = true,
            ConfirmButtonText = "Yes",
            ShowDenyButton = true,
            DenyButtonText = "No"
        });

        return result.IsConfirmed;
    }

    public async Task WarningAsync(string body) => await WarningAsync("Warning", body);
    public async Task WarningAsync(string title, string body)
    {
        var result = await SweetAlert.FireAsync(new SweetAlertOptions()
        {
            TitleText = title,
            Text = body,
            Icon = SweetAlertIcon.Warning,
            ShowConfirmButton = true,
            ConfirmButtonText = "Ok",
        });
    }
}
